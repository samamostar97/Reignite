import { PaginationRequest } from './common.model';

// Review Response DTO - matches backend ProjectReviewResponse
export interface ProjectReviewResponse {
  id: number;
  userId: number;
  userName: string;
  rating: number;
  comment?: string;
  createdAt: string;
}

// Response DTO - matches backend ProjectResponse
export interface ProjectResponse {
  id: number;
  title: string;
  description?: string;
  imageUrl?: string;
  hoursSpent?: number;
  userId: number;
  userName: string;
  hobbyId: number;
  hobbyName: string;
  productId?: number;
  productName?: string;
  createdAt: string;
  averageRating: number;
  reviewCount: number;
  reviews: ProjectReviewResponse[];
}

// Query filter - extends PaginationRequest, matches backend ProjectQueryFilter
export interface ProjectQueryFilter extends PaginationRequest {
  search?: string;
  orderBy?: string;
  hobbyId?: number;
}

// Request DTOs - match backend
export interface CreateProjectRequest {
  title: string;
  description?: string;
  imageUrl?: string;
  hoursSpent?: number;
  userId: number;
  hobbyId: number;
  productId?: number;
}

export interface UpdateProjectRequest {
  title?: string;
  description?: string;
  imageUrl?: string;
  hoursSpent?: number;
  userId?: number;
  hobbyId?: number;
  productId?: number;
}
