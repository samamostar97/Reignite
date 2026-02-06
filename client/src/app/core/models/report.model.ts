export interface DashboardReportResponse {
  revenueSummary: RevenueSummaryResponse;
  salesChart: SalesChartDataPoint[];
  topProducts: TopProductResponse[];
  recentOrders: RecentOrderResponse[];
  userGrowth: UserGrowthResponse;
  ratingOverview: RatingOverviewResponse;
}

export interface RevenueSummaryResponse {
  totalRevenue: number;
  todayRevenue: number;
  weekRevenue: number;
  monthRevenue: number;
  totalOrders: number;
  todayOrders: number;
  averageOrderValue: number;
  revenueChangePercent: number;
}

export interface SalesChartDataPoint {
  date: string;
  revenue: number;
  orderCount: number;
}

export interface TopProductResponse {
  productId: number;
  productName: string;
  productImageUrl?: string;
  categoryName: string;
  quantitySold: number;
  totalRevenue: number;
}

export interface RecentOrderResponse {
  orderId: number;
  customerName: string;
  customerImageUrl?: string;
  totalAmount: number;
  purchaseDate: string;
  status: string;
  itemCount: number;
}

export interface UserGrowthResponse {
  totalUsers: number;
  newUsersToday: number;
  newUsersThisWeek: number;
  newUsersThisMonth: number;
  growthPercent: number;
  growthChart: UserGrowthDataPoint[];
}

export interface UserGrowthDataPoint {
  date: string;
  newUsers: number;
  totalUsers: number;
}

export interface RatingOverviewResponse {
  averageProductRating: number;
  totalProductReviews: number;
  averageProjectRating: number;
  totalProjectReviews: number;
  productRatingDistribution: RatingDistribution;
  projectRatingDistribution: RatingDistribution;
}

export interface RatingDistribution {
  oneStar: number;
  twoStar: number;
  threeStar: number;
  fourStar: number;
  fiveStar: number;
}
