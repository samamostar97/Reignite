export interface Product {
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

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface ProductQueryFilter {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
  orderBy?: string;
}
