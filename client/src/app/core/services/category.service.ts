import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../models/common.model';
import {
  ProductCategoryResponse,
  ProductCategoryQueryFilter,
  CreateProductCategoryRequest,
  UpdateProductCategoryRequest
} from '../models/category.model';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/product-categories`;

  getCategories(filter?: ProductCategoryQueryFilter): Observable<PagedResult<ProductCategoryResponse>> {
    let params = new HttpParams();

    if (filter) {
      if (filter.pageNumber) params = params.set('pageNumber', filter.pageNumber.toString());
      if (filter.pageSize) params = params.set('pageSize', filter.pageSize.toString());
      if (filter.search) params = params.set('search', filter.search);
      if (filter.orderBy) params = params.set('orderBy', filter.orderBy);
    }

    return this.http.get<PagedResult<ProductCategoryResponse>>(this.apiUrl, { params });
  }

  getCategoryById(id: number): Observable<ProductCategoryResponse> {
    return this.http.get<ProductCategoryResponse>(`${this.apiUrl}/${id}`);
  }

  createCategory(data: CreateProductCategoryRequest): Observable<ProductCategoryResponse> {
    return this.http.post<ProductCategoryResponse>(this.apiUrl, data);
  }

  updateCategory(id: number, data: UpdateProductCategoryRequest): Observable<ProductCategoryResponse> {
    return this.http.put<ProductCategoryResponse>(`${this.apiUrl}/${id}`, data);
  }

  deleteCategory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
