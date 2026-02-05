import { PaginationRequest } from './common.model';

export enum UserRole {
  AppUser = 0,
  Admin = 1
}

export interface UserResponse {
  id: number;
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  phoneNumber: string;
  role: UserRole;
  profileImageUrl: string | null;
  createdAt: string;
  orderCount: number;
  projectCount: number;
}

export interface UserQueryFilter extends PaginationRequest {
  search?: string;
  orderBy?: string;
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  password: string;
  phoneNumber: string;
}

export interface UpdateUserRequest {
  firstName?: string;
  lastName?: string;
  username?: string;
  email?: string;
  phoneNumber?: string;
  profileImageUrl?: string;
}
