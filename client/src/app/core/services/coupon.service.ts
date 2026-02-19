import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../models/common.model';
import {
  CouponResponse,
  CouponQueryFilter,
  CreateCouponRequest,
  UpdateCouponRequest,
  ValidateCouponRequest
} from '../models/coupon.model';

@Injectable({
  providedIn: 'root'
})
export class CouponService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/coupons`;

  getCoupons(filter: CouponQueryFilter): Observable<PagedResult<CouponResponse>> {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber?.toString() || '1')
      .set('pageSize', filter.pageSize?.toString() || '10');

    if (filter.search) params = params.set('search', filter.search);
    if (filter.orderBy) params = params.set('orderBy', filter.orderBy);
    if (filter.isActive !== undefined) params = params.set('isActive', filter.isActive.toString());
    if (filter.isExpired !== undefined) params = params.set('isExpired', filter.isExpired.toString());

    return this.http.get<PagedResult<CouponResponse>>(this.apiUrl, { params });
  }

  getCouponById(id: number): Observable<CouponResponse> {
    return this.http.get<CouponResponse>(`${this.apiUrl}/${id}`);
  }

  createCoupon(request: CreateCouponRequest): Observable<CouponResponse> {
    return this.http.post<CouponResponse>(this.apiUrl, request);
  }

  updateCoupon(id: number, request: UpdateCouponRequest): Observable<CouponResponse> {
    return this.http.put<CouponResponse>(`${this.apiUrl}/${id}`, request);
  }

  deleteCoupon(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  validateCoupon(request: ValidateCouponRequest): Observable<CouponResponse> {
    return this.http.post<CouponResponse>(`${this.apiUrl}/validate`, request);
  }
}
