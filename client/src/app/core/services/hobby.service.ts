import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../models/common.model';
import {
  HobbyResponse,
  HobbyQueryFilter,
  CreateHobbyRequest,
  UpdateHobbyRequest
} from '../models/hobby.model';

@Injectable({
  providedIn: 'root'
})
export class HobbyService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/hobbies`;

  getHobbies(): Observable<HobbyResponse[]> {
    return this.http.get<HobbyResponse[]>(`${this.apiUrl}/all`);
  }

  getHobbiesPaged(filter?: HobbyQueryFilter): Observable<PagedResult<HobbyResponse>> {
    let params = new HttpParams();

    if (filter) {
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
      if (filter.search) params = params.set('search', filter.search);
      if (filter.orderBy) params = params.set('orderBy', filter.orderBy);
    }

    return this.http.get<PagedResult<HobbyResponse>>(this.apiUrl, { params });
  }

  getHobbyById(id: number): Observable<HobbyResponse> {
    return this.http.get<HobbyResponse>(`${this.apiUrl}/${id}`);
  }

  createHobby(data: CreateHobbyRequest): Observable<HobbyResponse> {
    return this.http.post<HobbyResponse>(this.apiUrl, data);
  }

  updateHobby(id: number, data: UpdateHobbyRequest): Observable<HobbyResponse> {
    return this.http.put<HobbyResponse>(`${this.apiUrl}/${id}`, data);
  }

  deleteHobby(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
