import { PaginationRequest } from './common.model';

export enum ActivityType {
  ProductReview = 0,
  ProjectReview = 1,
  NewProject = 2
}

export interface ActivityResponse {
  id: number;
  type: ActivityType;
  title: string;
  description: string;
  rating?: number;
  userName: string;
  userProfileImageUrl?: string;
  targetName?: string;
  targetImageUrl?: string;
  targetId: number;
  createdAt: string;
}

export interface ActivityQueryFilter extends PaginationRequest {
  type?: ActivityType;
}
