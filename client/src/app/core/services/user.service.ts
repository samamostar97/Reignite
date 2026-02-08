import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../models/common.model';
import {
  UserResponse,
  UserQueryFilter,
  CreateUserRequest,
  UpdateUserRequest,
  UserAddressResponse,
  CreateUserAddressRequest,
  UpdateUserAddressRequest,
  WishlistResponse
} from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/users`;

  getUsers(filter?: UserQueryFilter): Observable<PagedResult<UserResponse>> {
    let params = new HttpParams();

    if (filter) {
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
      if (filter.search) params = params.set('search', filter.search);
      if (filter.orderBy) params = params.set('orderBy', filter.orderBy);
    }

    return this.http.get<PagedResult<UserResponse>>(this.apiUrl, { params });
  }

  getUserById(id: number): Observable<UserResponse> {
    return this.http.get<UserResponse>(`${this.apiUrl}/${id}`);
  }

  createUser(data: CreateUserRequest, image?: File): Observable<UserResponse> {
    const formData = new FormData();
    formData.append('firstName', data.firstName);
    formData.append('lastName', data.lastName);
    formData.append('username', data.username);
    formData.append('email', data.email);
    formData.append('phoneNumber', data.phoneNumber);
    formData.append('password', data.password);
    if (image) formData.append('image', image);
    return this.http.post<UserResponse>(this.apiUrl, formData);
  }

  updateUser(id: number, data: UpdateUserRequest): Observable<UserResponse> {
    return this.http.put<UserResponse>(`${this.apiUrl}/${id}`, data);
  }

  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  uploadUserImage(id: number, file: File): Observable<{ fileUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ fileUrl: string }>(`${environment.apiUrl}/uploads/users/${id}`, formData);
  }

  deleteUserImage(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/uploads/users/${id}`);
  }

  // User Address methods
  getUserAddress(userId: number): Observable<UserAddressResponse> {
    return this.http.get<UserAddressResponse>(`${this.apiUrl}/${userId}/address`);
  }

  createUserAddress(userId: number, data: CreateUserAddressRequest): Observable<UserAddressResponse> {
    return this.http.post<UserAddressResponse>(`${this.apiUrl}/${userId}/address`, data);
  }

  updateUserAddress(userId: number, data: UpdateUserAddressRequest): Observable<UserAddressResponse> {
    return this.http.put<UserAddressResponse>(`${this.apiUrl}/${userId}/address`, data);
  }

  deleteUserAddress(userId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${userId}/address`);
  }

  // User Wishlist (read-only)
  getUserWishlist(userId: number): Observable<WishlistResponse> {
    return this.http.get<WishlistResponse>(`${this.apiUrl}/${userId}/wishlist`);
  }
}
