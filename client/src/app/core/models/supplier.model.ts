import { PaginationRequest } from './common.model';

// Response DTO - matches backend SupplierResponse
export interface SupplierResponse {
  id: number;
  name: string;
  website?: string;
}

// Query filter - extends PaginationRequest, matches backend SupplierQueryFilter
export interface SupplierQueryFilter extends PaginationRequest {
  search?: string;
  orderBy?: string;
}

// Request DTOs - match backend
export interface CreateSupplierRequest {
  name: string;
  website?: string;
}

export interface UpdateSupplierRequest {
  name: string;
  website?: string;
}
