// Base pagination request - matches backend PaginationRequest
export interface PaginationRequest {
  pageNumber?: number;  // default 1
  pageSize?: number;    // default 10
}

// Paged response - matches backend PagedResult<T>
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
}
