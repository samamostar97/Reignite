import { PaginationRequest } from './common.model';

// Response DTO - matches backend ProductCategoryResponse
export interface ProductCategoryResponse {
  id: number;
  name: string;
}

// Query filter - extends PaginationRequest, matches backend ProductCategoryQueryFilter
export interface ProductCategoryQueryFilter extends PaginationRequest {
  search?: string;
  orderBy?: string;
}

// Request DTOs - match backend
export interface CreateProductCategoryRequest {
  name: string;
}

export interface UpdateProductCategoryRequest {
  name?: string;
}
