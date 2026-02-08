export interface FaqResponse {
  id: number;
  question: string;
  answer: string;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateFaqRequest {
  question: string;
  answer: string;
}

export interface UpdateFaqRequest {
  question: string;
  answer: string;
}

export interface FaqQueryFilter {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
  orderBy?: string;
}
