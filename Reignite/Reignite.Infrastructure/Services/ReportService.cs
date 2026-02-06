using Microsoft.EntityFrameworkCore;
using Reignite.Application.DTOs.Response;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IRepository<OrderItem, int> _orderItemRepository;
        private readonly IRepository<User, int> _userRepository;
        private readonly IRepository<Product, int> _productRepository;
        private readonly IRepository<ProductReview, int> _productReviewRepository;
        private readonly IRepository<ProjectReview, int> _projectReviewRepository;

        public ReportService(
            IRepository<Order, int> orderRepository,
            IRepository<OrderItem, int> orderItemRepository,
            IRepository<User, int> userRepository,
            IRepository<Product, int> productRepository,
            IRepository<ProductReview, int> productReviewRepository,
            IRepository<ProjectReview, int> projectReviewRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _productReviewRepository = productReviewRepository;
            _projectReviewRepository = projectReviewRepository;
        }

        public async Task<DashboardReportResponse> GetDashboardReportAsync()
        {
            var revenueSummary = await GetRevenueSummaryAsync();
            var salesChart = await GetSalesChartAsync(30);
            var topProducts = await GetTopProductsAsync(5);
            var recentOrders = await GetRecentOrdersAsync(10);
            var userGrowth = await GetUserGrowthAsync(30);
            var ratingOverview = await GetRatingOverviewAsync();

            return new DashboardReportResponse
            {
                RevenueSummary = revenueSummary,
                SalesChart = salesChart,
                TopProducts = topProducts,
                RecentOrders = recentOrders,
                UserGrowth = userGrowth,
                RatingOverview = ratingOverview
            };
        }

        public async Task<RevenueSummaryResponse> GetRevenueSummaryAsync()
        {
            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var weekStart = now.Date.AddDays(-(int)now.DayOfWeek);
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var lastMonthStart = monthStart.AddMonths(-1);
            var lastMonthEnd = monthStart.AddDays(-1);

            var orders = await _orderRepository.AsQueryable().ToListAsync();

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var todayRevenue = orders.Where(o => o.PurchaseDate >= todayStart).Sum(o => o.TotalAmount);
            var weekRevenue = orders.Where(o => o.PurchaseDate >= weekStart).Sum(o => o.TotalAmount);
            var monthRevenue = orders.Where(o => o.PurchaseDate >= monthStart).Sum(o => o.TotalAmount);

            var lastMonthRevenue = orders
                .Where(o => o.PurchaseDate >= lastMonthStart && o.PurchaseDate <= lastMonthEnd)
                .Sum(o => o.TotalAmount);

            var revenueChangePercent = lastMonthRevenue > 0
                ? ((monthRevenue - lastMonthRevenue) / lastMonthRevenue) * 100
                : (monthRevenue > 0 ? 100 : 0);

            return new RevenueSummaryResponse
            {
                TotalRevenue = totalRevenue,
                TodayRevenue = todayRevenue,
                WeekRevenue = weekRevenue,
                MonthRevenue = monthRevenue,
                TotalOrders = orders.Count,
                TodayOrders = orders.Count(o => o.PurchaseDate >= todayStart),
                AverageOrderValue = orders.Count > 0 ? totalRevenue / orders.Count : 0,
                RevenueChangePercent = revenueChangePercent
            };
        }

        public async Task<List<SalesChartDataPoint>> GetSalesChartAsync(int days = 30)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-days + 1);

            var orders = await _orderRepository.AsQueryable()
                .Where(o => o.PurchaseDate >= startDate)
                .ToListAsync();

            var result = new List<SalesChartDataPoint>();

            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                var dayOrders = orders.Where(o => o.PurchaseDate.Date == date).ToList();

                result.Add(new SalesChartDataPoint
                {
                    Date = date,
                    Revenue = dayOrders.Sum(o => o.TotalAmount),
                    OrderCount = dayOrders.Count
                });
            }

            return result;
        }

        public async Task<List<TopProductResponse>> GetTopProductsAsync(int count = 5)
        {
            var orderItems = await _orderItemRepository.AsQueryable()
                .Include(oi => oi.Product)
                    .ThenInclude(p => p.ProductCategory)
                .ToListAsync();

            var topProducts = orderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new TopProductResponse
                {
                    ProductId = g.Key,
                    ProductName = g.First().Product?.Name ?? "Nepoznat proizvod",
                    ProductImageUrl = g.First().Product?.ProductImageUrl,
                    CategoryName = g.First().Product?.ProductCategory?.Name ?? "Nepoznata kategorija",
                    QuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .OrderByDescending(p => p.TotalRevenue)
                .Take(count)
                .ToList();

            return topProducts;
        }

        public async Task<List<RecentOrderResponse>> GetRecentOrdersAsync(int count = 10)
        {
            var orders = await _orderRepository.AsQueryable()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.PurchaseDate)
                .Take(count)
                .ToListAsync();

            return orders.Select(o => new RecentOrderResponse
            {
                OrderId = o.Id,
                CustomerName = o.User != null ? $"{o.User.FirstName} {o.User.LastName}" : "Nepoznat korisnik",
                CustomerImageUrl = o.User?.ProfileImageUrl,
                TotalAmount = o.TotalAmount,
                PurchaseDate = o.PurchaseDate,
                Status = o.Status.ToString(),
                ItemCount = o.OrderItems?.Count ?? 0
            }).ToList();
        }

        public async Task<UserGrowthResponse> GetUserGrowthAsync(int days = 30)
        {
            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var weekStart = now.Date.AddDays(-(int)now.DayOfWeek);
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var startDate = now.Date.AddDays(-days + 1);
            var lastMonthStart = monthStart.AddMonths(-1);
            var lastMonthEnd = monthStart.AddDays(-1);

            var users = await _userRepository.AsQueryable().ToListAsync();

            var totalUsers = users.Count;
            var newUsersToday = users.Count(u => u.CreatedAt >= todayStart);
            var newUsersThisWeek = users.Count(u => u.CreatedAt >= weekStart);
            var newUsersThisMonth = users.Count(u => u.CreatedAt >= monthStart);

            var lastMonthUsers = users.Count(u => u.CreatedAt >= lastMonthStart && u.CreatedAt <= lastMonthEnd);
            var growthPercent = lastMonthUsers > 0
                ? ((double)(newUsersThisMonth - lastMonthUsers) / lastMonthUsers) * 100
                : (newUsersThisMonth > 0 ? 100 : 0);

            var growthChart = new List<UserGrowthDataPoint>();
            var runningTotal = users.Count(u => u.CreatedAt < startDate);

            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                var newUsers = users.Count(u => u.CreatedAt.Date == date);
                runningTotal += newUsers;

                growthChart.Add(new UserGrowthDataPoint
                {
                    Date = date,
                    NewUsers = newUsers,
                    TotalUsers = runningTotal
                });
            }

            return new UserGrowthResponse
            {
                TotalUsers = totalUsers,
                NewUsersToday = newUsersToday,
                NewUsersThisWeek = newUsersThisWeek,
                NewUsersThisMonth = newUsersThisMonth,
                GrowthPercent = growthPercent,
                GrowthChart = growthChart
            };
        }

        public async Task<RatingOverviewResponse> GetRatingOverviewAsync()
        {
            var productReviews = await _productReviewRepository.AsQueryable().ToListAsync();
            var projectReviews = await _projectReviewRepository.AsQueryable().ToListAsync();

            var avgProductRating = productReviews.Count > 0 ? productReviews.Average(r => r.Rating) : 0;
            var avgProjectRating = projectReviews.Count > 0 ? projectReviews.Average(r => r.Rating) : 0;

            return new RatingOverviewResponse
            {
                AverageProductRating = Math.Round(avgProductRating, 1),
                TotalProductReviews = productReviews.Count,
                AverageProjectRating = Math.Round(avgProjectRating, 1),
                TotalProjectReviews = projectReviews.Count,
                ProductRatingDistribution = new RatingDistribution
                {
                    OneStar = productReviews.Count(r => r.Rating == 1),
                    TwoStar = productReviews.Count(r => r.Rating == 2),
                    ThreeStar = productReviews.Count(r => r.Rating == 3),
                    FourStar = productReviews.Count(r => r.Rating == 4),
                    FiveStar = productReviews.Count(r => r.Rating == 5)
                },
                ProjectRatingDistribution = new RatingDistribution
                {
                    OneStar = projectReviews.Count(r => r.Rating == 1),
                    TwoStar = projectReviews.Count(r => r.Rating == 2),
                    ThreeStar = projectReviews.Count(r => r.Rating == 3),
                    FourStar = projectReviews.Count(r => r.Rating == 4),
                    FiveStar = projectReviews.Count(r => r.Rating == 5)
                }
            };
        }
    }
}
