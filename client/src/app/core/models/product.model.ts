import { PaginationRequest } from './common.model';

// Response DTO - matches backend ProductResponse
export interface ProductResponse {
  id: number;
  name: string;
  price: number;
  description?: string;
  productCategoryId: number;
  productCategoryName: string;
  supplierId: number;
  supplierName: string;
  productImageUrl?: string;
}

// Query filter - extends PaginationRequest, matches backend ProductQueryFilter
export interface ProductQueryFilter extends PaginationRequest {
  search?: string;
  orderBy?: string;
  productCategoryId?: number;
}

// Request DTOs - match backend
export interface CreateProductRequest {
  name: string;
  price: number;
  description?: string;
  productCategoryId: number;
  supplierId: number;
  productImageUrl?: string;
}

export interface UpdateProductRequest {
  name?: string;
  price?: number;
  description?: string;
  productCategoryId?: number;
  supplierId?: number;
  productImageUrl?: string;
}
