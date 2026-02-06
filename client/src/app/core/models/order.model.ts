import { PaginationRequest } from './common.model';

export enum OrderStatus {
  Processing = 0,
  OnDelivery = 1
}

export interface OrderItemResponse {
  id: number;
  productId: number;
  productName: string;
  productImageUrl?: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface OrderResponse {
  id: number;
  userId: number;
  userName: string;
  userProfileImageUrl?: string;
  totalAmount: number;
  purchaseDate: string;
  status: OrderStatus;
  statusName: string;
  itemCount: number;
  items: OrderItemResponse[];
}

export interface OrderQueryFilter extends PaginationRequest {
  search?: string;
  orderBy?: string;
  userId?: number;
  status?: OrderStatus;
}

export interface CreateOrderItemRequest {
  productId: number;
  quantity: number;
}

export interface CreateOrderRequest {
  userId: number;
  items: CreateOrderItemRequest[];
}
