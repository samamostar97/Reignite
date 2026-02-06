import { PaginationRequest } from './common.model';

export interface ProductReviewResponse {
  id: number;
  userId: number;
  userName: string;
  userProfileImageUrl?: string;
  productId: number;
  productName: string;
  rating: number;
  comment?: string;
  createdAt: string;
}

export interface CreateProductReviewRequest {
  productId: number;
  rating: number;
  comment?: string;
}

export interface UpdateProductReviewRequest {
  rating: number;
  comment?: string;
}

export interface ProductReviewQueryFilter extends PaginationRequest {
  productId?: number;
  userId?: number;
  minRating?: number;
  maxRating?: number;
  orderBy?: string;
}
