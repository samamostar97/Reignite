import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../models/common.model';
import {
  CommunityUserResponse,
  PublicUserProfileResponse,
  ChatMessageResponse
} from '../models/community.model';

@Injectable({ providedIn: 'root' })
export class CommunityService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/community`;

  getUsers(pageNumber = 1, pageSize = 12, hobbyId?: number): Observable<PagedResult<CommunityUserResponse>> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    if (hobbyId) params = params.set('hobbyId', hobbyId.toString());
    return this.http.get<PagedResult<CommunityUserResponse>>(`${this.apiUrl}/users`, { params });
  }

  getUserProfile(userId: number): Observable<PublicUserProfileResponse> {
    return this.http.get<PublicUserProfileResponse>(`${this.apiUrl}/users/${userId}`);
  }

  getChatMessages(hobbyId: number, pageNumber = 1, pageSize = 50): Observable<PagedResult<ChatMessageResponse>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PagedResult<ChatMessageResponse>>(`${this.apiUrl}/chat/${hobbyId}/messages`, { params });
  }
}
