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

export interface UserAddressResponse {
  id: number;
  userId: number;
  street: string;
  city: string;
  postalCode: string;
  country: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateUserAddressRequest {
  street: string;
  city: string;
  postalCode: string;
  country: string;
}

export interface UpdateUserAddressRequest {
  street: string;
  city: string;
  postalCode: string;
  country: string;
}

export interface WishlistResponse {
  id: number;
  userId: number;
  createdAt: string;
  items: WishlistItemResponse[];
}

export interface WishlistItemResponse {
  id: number;
  wishlistId: number;
  productId: number;
  productName: string;
  productPrice: number;
  productImageUrl: string | null;
  addedAt: string;
}

export enum SkillLevel {
  Beginner = 0,
  Intermediate = 1,
  Advanced = 2
}

export interface UserHobbyResponse {
  id: number;
  userId: number;
  hobbyId: number;
  hobbyName: string;
  skillLevel: SkillLevel;
  bio: string | null;
}

export interface AddUserHobbyRequest {
  hobbyId: number;
  skillLevel: SkillLevel;
  bio?: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}
