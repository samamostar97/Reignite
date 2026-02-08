import { PaginationRequest } from './common.model';

export interface CouponResponse {
  id: number;
  code: string;
  discountType: string;
  discountValue: number;
  minimumOrderAmount?: number;
  expiryDate?: string;
  maxUses?: number;
  timesUsed: number;
  isActive: boolean;
  isExpired: boolean;
  isMaxedOut: boolean;
}

export interface CreateCouponRequest {
  code: string;
  discountType: string;
  discountValue: number;
  minimumOrderAmount?: number;
  expiryDate?: string;
  maxUses?: number;
  isActive: boolean;
}

export interface UpdateCouponRequest {
  code: string;
  discountType: string;
  discountValue: number;
  minimumOrderAmount?: number;
  expiryDate?: string;
  maxUses?: number;
  isActive: boolean;
}

export interface CouponQueryFilter extends PaginationRequest {
  search?: string;
  isActive?: boolean;
  isExpired?: boolean;
  orderBy?: string;
}
