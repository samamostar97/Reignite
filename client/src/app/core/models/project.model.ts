import { PaginationRequest } from './common.model';

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
}

// Query filter - extends PaginationRequest, matches backend ProjectQueryFilter
export interface ProjectQueryFilter extends PaginationRequest {
  search?: string;
  orderBy?: string;
  userId?: number;
  hobbyId?: number;
  productId?: number;
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
