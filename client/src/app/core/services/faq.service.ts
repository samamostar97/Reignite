import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { FaqResponse, CreateFaqRequest, UpdateFaqRequest, FaqQueryFilter } from '../models/faq.model';
import { PagedResult } from '../models/common.model';

@Injectable({
  providedIn: 'root'
})
export class FaqService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/faqs`;

  getAll(filter?: FaqQueryFilter): Observable<PagedResult<FaqResponse>> {
    let params = new HttpParams();

    if (filter) {
      if (filter.pageNumber !== undefined) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize !== undefined) params = params.set('pageSize', filter.pageSize.toString());
      if (filter.search) params = params.set('search', filter.search);
      if (filter.orderBy) params = params.set('orderBy', filter.orderBy);
    }

    return this.http.get<PagedResult<FaqResponse>>(this.apiUrl, { params });
  }

  getById(id: number): Observable<FaqResponse> {
    return this.http.get<FaqResponse>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateFaqRequest): Observable<FaqResponse> {
    return this.http.post<FaqResponse>(this.apiUrl, request);
  }

  update(id: number, request: UpdateFaqRequest): Observable<FaqResponse> {
    return this.http.put<FaqResponse>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
