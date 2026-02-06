import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ProductReviewResponse,
  CreateProductReviewRequest,
  UpdateProductReviewRequest,
  ProductReviewQueryFilter
} from '../models/product-review.model';
import { PagedResult } from '../models/common.model';

@Injectable({
  providedIn: 'root'
})
export class ProductReviewService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/product-reviews`;

  getReviews(filter: ProductReviewQueryFilter): Observable<PagedResult<ProductReviewResponse>> {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber?.toString() || '1')
      .set('pageSize', filter.pageSize?.toString() || '10');

    if (filter.productId !== undefined) {
      params = params.set('productId', filter.productId.toString());
    }
    if (filter.userId !== undefined) {
      params = params.set('userId', filter.userId.toString());
    }
    if (filter.minRating !== undefined) {
      params = params.set('minRating', filter.minRating.toString());
    }
    if (filter.maxRating !== undefined) {
      params = params.set('maxRating', filter.maxRating.toString());
    }
    if (filter.orderBy) {
      params = params.set('orderBy', filter.orderBy);
    }

    return this.http.get<PagedResult<ProductReviewResponse>>(this.apiUrl, { params });
  }

  getReviewById(id: number): Observable<ProductReviewResponse> {
    return this.http.get<ProductReviewResponse>(`${this.apiUrl}/${id}`);
  }

  getByProductId(productId: number, pageNumber = 1, pageSize = 10): Observable<PagedResult<ProductReviewResponse>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PagedResult<ProductReviewResponse>>(`${this.apiUrl}/product/${productId}`, { params });
  }

  getAverageRating(productId: number): Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/product/${productId}/average-rating`);
  }

  hasUserReviewed(productId: number): Observable<boolean> {
    return this.http.get<boolean>(`${this.apiUrl}/product/${productId}/has-reviewed`);
  }

  createReview(request: CreateProductReviewRequest): Observable<ProductReviewResponse> {
    return this.http.post<ProductReviewResponse>(this.apiUrl, request);
  }

  updateReview(id: number, request: UpdateProductReviewRequest): Observable<ProductReviewResponse> {
    return this.http.put<ProductReviewResponse>(`${this.apiUrl}/${id}`, request);
  }

  deleteReview(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
