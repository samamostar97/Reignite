namespace Reignite.Application.DTOs.Response
{
    public class DashboardReportResponse
    {
        public RevenueSummaryResponse RevenueSummary { get; set; } = new();
        public List<SalesChartDataPoint> SalesChart { get; set; } = new();
        public List<TopProductResponse> TopProducts { get; set; } = new();
        public List<RecentOrderResponse> RecentOrders { get; set; } = new();
        public UserGrowthResponse UserGrowth { get; set; } = new();
        public RatingOverviewResponse RatingOverview { get; set; } = new();
    }

    public class RevenueSummaryResponse
    {
        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal WeekRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TodayOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public decimal RevenueChangePercent { get; set; }
    }

    public class SalesChartDataPoint
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class TopProductResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class RecentOrderResponse
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerImageUrl { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ItemCount { get; set; }
    }

    public class UserGrowthResponse
    {
        public int TotalUsers { get; set; }
        public int NewUsersToday { get; set; }
        public int NewUsersThisWeek { get; set; }
        public int NewUsersThisMonth { get; set; }
        public double GrowthPercent { get; set; }
        public List<UserGrowthDataPoint> GrowthChart { get; set; } = new();
    }

    public class UserGrowthDataPoint
    {
        public DateTime Date { get; set; }
        public int NewUsers { get; set; }
        public int TotalUsers { get; set; }
    }

    public class RatingOverviewResponse
    {
        public double AverageProductRating { get; set; }
        public int TotalProductReviews { get; set; }
        public double AverageProjectRating { get; set; }
        public int TotalProjectReviews { get; set; }
        public RatingDistribution ProductRatingDistribution { get; set; } = new();
        public RatingDistribution ProjectRatingDistribution { get; set; } = new();
    }

    public class RatingDistribution
    {
        public int OneStar { get; set; }
        public int TwoStar { get; set; }
        public int ThreeStar { get; set; }
        public int FourStar { get; set; }
        public int FiveStar { get; set; }
    }
}
