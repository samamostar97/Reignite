import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../models/common.model';
import {
  UserResponse,
  UpdateUserRequest,
  UserAddressResponse,
  CreateUserAddressRequest,
  UpdateUserAddressRequest,
  UserHobbyResponse,
  AddUserHobbyRequest,
  ChangePasswordRequest,
  WishlistResponse,
  WishlistItemResponse
} from '../models/user.model';
import { OrderResponse } from '../models/order.model';

@Injectable({ providedIn: 'root' })
export class ProfileService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/profile`;

  // Profile
  getProfile(): Observable<UserResponse> {
    return this.http.get<UserResponse>(this.apiUrl);
  }

  updateProfile(data: UpdateUserRequest): Observable<UserResponse> {
    return this.http.put<UserResponse>(this.apiUrl, data);
  }

  // Profile Image
  uploadImage(file: File): Observable<UserResponse> {
    const formData = new FormData();
    formData.append('image', file);
    return this.http.post<UserResponse>(`${this.apiUrl}/image`, formData);
  }

  deleteImage(): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/image`);
  }

  // Password
  changePassword(data: ChangePasswordRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/password`, data);
  }

  // Address
  getAddress(): Observable<UserAddressResponse> {
    return this.http.get<UserAddressResponse>(`${this.apiUrl}/address`);
  }

  createAddress(data: CreateUserAddressRequest): Observable<UserAddressResponse> {
    return this.http.post<UserAddressResponse>(`${this.apiUrl}/address`, data);
  }

  updateAddress(data: UpdateUserAddressRequest): Observable<UserAddressResponse> {
    return this.http.put<UserAddressResponse>(`${this.apiUrl}/address`, data);
  }

  deleteAddress(): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/address`);
  }

  // Orders
  getOrders(pageNumber = 1, pageSize = 10): Observable<PagedResult<OrderResponse>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PagedResult<OrderResponse>>(`${this.apiUrl}/orders`, { params });
  }

  // Hobbies
  getHobbies(): Observable<UserHobbyResponse[]> {
    return this.http.get<UserHobbyResponse[]>(`${this.apiUrl}/hobbies`);
  }

  addHobby(data: AddUserHobbyRequest): Observable<UserHobbyResponse> {
    return this.http.post<UserHobbyResponse>(`${this.apiUrl}/hobbies`, data);
  }

  deleteHobby(hobbyId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/hobbies/${hobbyId}`);
  }

  // Wishlist
  getWishlist(): Observable<WishlistResponse> {
    return this.http.get<WishlistResponse>(`${this.apiUrl}/wishlist`);
  }

  addToWishlist(productId: number): Observable<WishlistItemResponse> {
    return this.http.post<WishlistItemResponse>(`${this.apiUrl}/wishlist/${productId}`, {});
  }

  removeFromWishlist(productId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/wishlist/${productId}`);
  }
}
