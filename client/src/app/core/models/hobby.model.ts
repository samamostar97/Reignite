export interface HobbyResponse {
  id: number;
  name: string;
  description?: string;
}

export interface HobbyQueryFilter {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
  orderBy?: string;
}

export interface CreateHobbyRequest {
  name: string;
  description?: string;
}

export interface UpdateHobbyRequest {
  name: string;
  description?: string;
}
