export interface Project {
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
