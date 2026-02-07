import { HttpInterceptorFn, HttpErrorResponse, HttpRequest, HttpHandlerFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError, BehaviorSubject, filter, take } from 'rxjs';
import { ApiErrorResponse } from '../models/api.model';
import { AuthService } from '../services/auth.service';

// Track if we're currently refreshing to prevent multiple refresh attempts
let isRefreshing = false;
const refreshTokenSubject = new BehaviorSubject<string | null>(null);

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // Handle 401 with token refresh
      if (error.status === 401 && !req.url.includes('/auth/')) {
        return handle401Error(req, next, authService, router);
      }

      // Handle other errors
      let errorMessage = 'Došlo je do greške. Pokušajte ponovo.';

      if (error.error instanceof ErrorEvent) {
        errorMessage = 'Greška u mreži. Provjerite internet konekciju.';
      } else {
        const apiError = error.error as ApiErrorResponse;

        switch (error.status) {
          case 400:
            errorMessage = apiError?.error || 'Neispravan zahtjev.';
            break;
          case 403:
            errorMessage = apiError?.error || 'Nemate dozvolu za ovu akciju.';
            break;
          case 404:
            errorMessage = apiError?.error || 'Resurs nije pronađen.';
            break;
          case 409:
            errorMessage = apiError?.error || 'Konflikt - resurs već postoji.';
            break;
          case 500:
            errorMessage = 'Greška na serveru. Pokušajte kasnije.';
            break;
          default:
            errorMessage = apiError?.error || 'Neočekivana greška.';
        }
      }

      return throwError(() => ({
        ...error,
        message: errorMessage,
        originalError: error.error
      }));
    })
  );
};

function handle401Error(
  request: HttpRequest<unknown>,
  next: HttpHandlerFn,
  authService: AuthService,
  router: Router
) {
  if (!isRefreshing) {
    isRefreshing = true;
    refreshTokenSubject.next(null);

    const refreshToken = localStorage.getItem('refreshToken');
    if (!refreshToken) {
      return logoutAndRedirect(router);
    }

    return authService.refreshToken().pipe(
      switchMap((response) => {
        isRefreshing = false;
        refreshTokenSubject.next(response.accessToken);

        // Retry the original request with the new token
        return next(addTokenToRequest(request, response.accessToken));
      }),
      catchError(() => {
        isRefreshing = false;
        refreshTokenSubject.next(null);
        return logoutAndRedirect(router);
      })
    );
  }

  // If already refreshing, wait for the new token and retry
  return refreshTokenSubject.pipe(
    filter((token) => token !== null),
    take(1),
    switchMap((token) => next(addTokenToRequest(request, token!)))
  );
}

function addTokenToRequest(request: HttpRequest<unknown>, token: string): HttpRequest<unknown> {
  return request.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  });
}

function logoutAndRedirect(router: Router) {
  localStorage.removeItem('accessToken');
  localStorage.removeItem('refreshToken');
  localStorage.removeItem('currentUser');
  router.navigate(['/login']);

  return throwError(() => ({
    status: 401,
    message: 'Sesija je istekla. Molimo prijavite se ponovo.'
  }));
}
