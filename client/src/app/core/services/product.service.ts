import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../models/common.model';
import {
  ProductResponse,
  ProductQueryFilter,
  CreateProductRequest,
  UpdateProductRequest
} from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/products`;

  getProducts(filter?: ProductQueryFilter): Observable<PagedResult<ProductResponse>> {
    let params = new HttpParams();

    if (filter) {
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
      if (filter.search) params = params.set('search', filter.search);
      if (filter.orderBy) params = params.set('orderBy', filter.orderBy);
      if (filter.productCategoryId) params = params.set('productCategoryId', filter.productCategoryId.toString());
    }

    return this.http.get<PagedResult<ProductResponse>>(this.apiUrl, { params });
  }

  getProductById(id: number): Observable<ProductResponse> {
    return this.http.get<ProductResponse>(`${this.apiUrl}/${id}`);
  }

  createProduct(data: CreateProductRequest): Observable<ProductResponse> {
    return this.http.post<ProductResponse>(this.apiUrl, data);
  }

  updateProduct(id: number, data: UpdateProductRequest): Observable<ProductResponse> {
    return this.http.put<ProductResponse>(`${this.apiUrl}/${id}`, data);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  uploadProductImage(id: number, file: File): Observable<ProductResponse> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ProductResponse>(`${this.apiUrl}/${id}/image`, formData);
  }

  deleteProductImage(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}/image`);
  }

  getBestSelling(count: number = 5): Observable<ProductResponse[]> {
    return this.http.get<ProductResponse[]>(`${this.apiUrl}/best-selling`, {
      params: { count: count.toString() }
    });
  }
}
