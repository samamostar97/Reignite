import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of, tap } from 'rxjs';
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

  // Cache for the default product list (first page, no filters)
  private cachedProducts = signal<PagedResult<ProductResponse> | null>(null);
  private cacheKey = '';

  getProducts(filter?: ProductQueryFilter): Observable<PagedResult<ProductResponse>> {
    let params = new HttpParams();
    const currentKey = JSON.stringify(filter || {});

    if (filter) {
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
      if (filter.search) params = params.set('search', filter.search);
      if (filter.orderBy) params = params.set('orderBy', filter.orderBy);
      if (filter.productCategoryId) params = params.set('productCategoryId', filter.productCategoryId.toString());
    }

    // Return cached data if available and filter matches
    const cached = this.cachedProducts();
    if (cached && this.cacheKey === currentKey) {
      return of(cached);
    }

    return this.http.get<PagedResult<ProductResponse>>(this.apiUrl, { params }).pipe(
      tap(result => {
        this.cachedProducts.set(result);
        this.cacheKey = currentKey;
      })
    );
  }

  // Preload products (called from dashboard)
  preloadProducts(): void {
    if (!this.cachedProducts()) {
      this.getProducts({ pageNumber: 1, pageSize: 10 }).subscribe();
    }
  }

  // Invalidate cache (after create/update/delete)
  invalidateCache(): void {
    this.cachedProducts.set(null);
    this.cacheKey = '';
  }

  getProductById(id: number): Observable<ProductResponse> {
    return this.http.get<ProductResponse>(`${this.apiUrl}/${id}`);
  }

  createProduct(data: CreateProductRequest, image?: File): Observable<ProductResponse> {
    const formData = new FormData();
    formData.append('name', data.name);
    if (data.description) formData.append('description', data.description);
    formData.append('price', data.price.toString());
    formData.append('productCategoryId', data.productCategoryId.toString());
    formData.append('supplierId', data.supplierId.toString());
    if (image) formData.append('image', image);
    return this.http.post<ProductResponse>(this.apiUrl, formData).pipe(
      tap(() => this.invalidateCache())
    );
  }

  updateProduct(id: number, data: UpdateProductRequest): Observable<ProductResponse> {
    return this.http.put<ProductResponse>(`${this.apiUrl}/${id}`, data).pipe(
      tap(() => this.invalidateCache())
    );
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => this.invalidateCache())
    );
  }

  uploadProductImage(id: number, file: File): Observable<{ fileUrl: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ fileUrl: string }>(`${environment.apiUrl}/uploads/products/${id}`, formData);
  }

  deleteProductImage(id: number): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/uploads/products/${id}`);
  }

  getBestSelling(count: number = 5): Observable<ProductResponse[]> {
    return this.http.get<ProductResponse[]>(`${this.apiUrl}/best-selling`, {
      params: { count: count.toString() }
    });
  }
}
