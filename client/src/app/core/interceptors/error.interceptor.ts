import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { ApiErrorResponse } from '../models/api.model';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Došlo je do greške. Pokušajte ponovo.';

      if (error.error instanceof ErrorEvent) {
        // Client-side error (network issues, etc.)
        errorMessage = 'Greška u mreži. Provjerite internet konekciju.';
      } else {
        // Server-side error
        const apiError = error.error as ApiErrorResponse;

        switch (error.status) {
          case 400:
            errorMessage = apiError?.error || 'Neispravan zahtjev.';
            break;
          case 401:
            errorMessage = apiError?.error || 'Neautorizovan pristup.';
            // Optionally redirect to login
            localStorage.removeItem('accessToken');
            localStorage.removeItem('refreshToken');
            localStorage.removeItem('currentUser');
            router.navigate(['/login']);
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

      // Return a new error with the formatted message
      return throwError(() => ({
        ...error,
        message: errorMessage,
        originalError: error.error
      }));
    })
  );
};
