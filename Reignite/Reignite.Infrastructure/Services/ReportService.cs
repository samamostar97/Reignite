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

        public async Task<DashboardReportResponse> GetDashboardReportAsync(CancellationToken cancellationToken = default)
        {
            var revenueSummary = await GetRevenueSummaryAsync(cancellationToken);
            var salesChart = await GetSalesChartAsync(30, cancellationToken);
            var topProducts = await GetTopProductsAsync(5, cancellationToken);
            var recentOrders = await GetRecentOrdersAsync(10, cancellationToken);
            var userGrowth = await GetUserGrowthAsync(30, cancellationToken);
            var ratingOverview = await GetRatingOverviewAsync(cancellationToken);

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

        public async Task<RevenueSummaryResponse> GetRevenueSummaryAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var weekStart = now.Date.AddDays(-(int)now.DayOfWeek);
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var lastMonthStart = monthStart.AddMonths(-1);
            var lastMonthEnd = monthStart.AddDays(-1);

            var query = _orderRepository.AsQueryable().AsNoTracking();

            // Execute aggregations in the database
            var totalRevenue = await query.SumAsync(o => o.TotalAmount, cancellationToken);
            var totalOrders = await query.CountAsync(cancellationToken);
            var todayRevenue = await query.Where(o => o.PurchaseDate >= todayStart).SumAsync(o => o.TotalAmount, cancellationToken);
            var todayOrders = await query.Where(o => o.PurchaseDate >= todayStart).CountAsync(cancellationToken);
            var weekRevenue = await query.Where(o => o.PurchaseDate >= weekStart).SumAsync(o => o.TotalAmount, cancellationToken);
            var monthRevenue = await query.Where(o => o.PurchaseDate >= monthStart).SumAsync(o => o.TotalAmount, cancellationToken);
            var lastMonthRevenue = await query
                .Where(o => o.PurchaseDate >= lastMonthStart && o.PurchaseDate <= lastMonthEnd)
                .SumAsync(o => o.TotalAmount, cancellationToken);

            var revenueChangePercent = lastMonthRevenue > 0
                ? ((monthRevenue - lastMonthRevenue) / lastMonthRevenue) * 100
                : (monthRevenue > 0 ? 100 : 0);

            return new RevenueSummaryResponse
            {
                TotalRevenue = totalRevenue,
                TodayRevenue = todayRevenue,
                WeekRevenue = weekRevenue,
                MonthRevenue = monthRevenue,
                TotalOrders = totalOrders,
                TodayOrders = todayOrders,
                AverageOrderValue = totalOrders > 0 ? totalRevenue / totalOrders : 0,
                RevenueChangePercent = revenueChangePercent
            };
        }

        public async Task<List<SalesChartDataPoint>> GetSalesChartAsync(int days = 30, CancellationToken cancellationToken = default)
        {
            var startDate = DateTime.UtcNow.Date.AddDays(-days + 1);

            // Group by date in database
            var dailyData = await _orderRepository.AsQueryable()
                .AsNoTracking()
                .Where(o => o.PurchaseDate >= startDate)
                .GroupBy(o => o.PurchaseDate.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(o => o.TotalAmount), OrderCount = g.Count() })
                .ToListAsync(cancellationToken);

            var lookup = dailyData.ToDictionary(d => d.Date, d => d);

            var result = new List<SalesChartDataPoint>();
            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                result.Add(new SalesChartDataPoint
                {
                    Date = date,
                    Revenue = lookup.TryGetValue(date, out var data) ? data.Revenue : 0,
                    OrderCount = lookup.TryGetValue(date, out var dataCount) ? dataCount.OrderCount : 0
                });
            }

            return result;
        }

        public async Task<List<TopProductResponse>> GetTopProductsAsync(int count = 5, CancellationToken cancellationToken = default)
        {
            // Do aggregation in database, then fetch product details
            var topProductIds = await _orderItemRepository.AsQueryable()
                .AsNoTracking()
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    QuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Take(count)
                .ToListAsync(cancellationToken);

            var productIds = topProductIds.Select(x => x.ProductId).ToList();

            var products = await _productRepository.AsQueryable()
                .AsNoTracking()
                .Include(p => p.ProductCategory)
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            var productLookup = products.ToDictionary(p => p.Id);

            return topProductIds.Select(tp => new TopProductResponse
            {
                ProductId = tp.ProductId,
                ProductName = productLookup.TryGetValue(tp.ProductId, out var p) ? p.Name : "Nepoznat proizvod",
                ProductImageUrl = productLookup.TryGetValue(tp.ProductId, out var p2) ? p2.ProductImageUrl : null,
                CategoryName = productLookup.TryGetValue(tp.ProductId, out var p3) ? p3.ProductCategory?.Name ?? "Nepoznata kategorija" : "Nepoznata kategorija",
                QuantitySold = tp.QuantitySold,
                TotalRevenue = tp.TotalRevenue
            }).ToList();
        }

        public async Task<List<RecentOrderResponse>> GetRecentOrdersAsync(int count = 10, CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.AsQueryable()
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.PurchaseDate)
                .Take(count)
                .ToListAsync(cancellationToken);

            return orders.Select(o => new RecentOrderResponse
            {
                OrderId = o.Id,
                CustomerName = o.User != null ? $"{o.User.FirstName} {o.User.LastName}" : "Nepoznat korisnik",
                CustomerImageUrl = o.User?.ProfileImageUrl,
                TotalAmount = o.TotalAmount,
                PurchaseDate = o.PurchaseDate,
                Status = o.Status.ToString(),
                ItemCount = o.OrderItems?.Sum(oi => oi.Quantity) ?? 0
            }).ToList();
        }

        public async Task<UserGrowthResponse> GetUserGrowthAsync(int days = 30, CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var weekStart = now.Date.AddDays(-(int)now.DayOfWeek);
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var startDate = now.Date.AddDays(-days + 1);
            var lastMonthStart = monthStart.AddMonths(-1);
            var lastMonthEnd = monthStart.AddDays(-1);

            var query = _userRepository.AsQueryable().AsNoTracking();

            // Execute counts in database
            var totalUsers = await query.CountAsync(cancellationToken);
            var newUsersToday = await query.CountAsync(u => u.CreatedAt >= todayStart, cancellationToken);
            var newUsersThisWeek = await query.CountAsync(u => u.CreatedAt >= weekStart, cancellationToken);
            var newUsersThisMonth = await query.CountAsync(u => u.CreatedAt >= monthStart, cancellationToken);
            var lastMonthUsers = await query.CountAsync(u => u.CreatedAt >= lastMonthStart && u.CreatedAt <= lastMonthEnd, cancellationToken);
            var usersBeforeStartDate = await query.CountAsync(u => u.CreatedAt < startDate, cancellationToken);

            var growthPercent = lastMonthUsers > 0
                ? ((double)(newUsersThisMonth - lastMonthUsers) / lastMonthUsers) * 100
                : (newUsersThisMonth > 0 ? 100 : 0);

            // Get daily counts grouped in database
            var dailyCounts = await query
                .Where(u => u.CreatedAt >= startDate)
                .GroupBy(u => u.CreatedAt.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var dailyLookup = dailyCounts.ToDictionary(d => d.Date, d => d.Count);

            var growthChart = new List<UserGrowthDataPoint>();
            var runningTotal = usersBeforeStartDate;

            for (int i = 0; i < days; i++)
            {
                var date = startDate.AddDays(i);
                var newUsers = dailyLookup.TryGetValue(date, out var count) ? count : 0;
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

        public async Task<RatingOverviewResponse> GetRatingOverviewAsync(CancellationToken cancellationToken = default)
        {
            var productQuery = _productReviewRepository.AsQueryable().AsNoTracking();
            var projectQuery = _projectReviewRepository.AsQueryable().AsNoTracking();

            // Execute aggregations in database
            var totalProductReviews = await productQuery.CountAsync(cancellationToken);
            var avgProductRating = totalProductReviews > 0 ? await productQuery.AverageAsync(r => r.Rating, cancellationToken) : 0;

            var totalProjectReviews = await projectQuery.CountAsync(cancellationToken);
            var avgProjectRating = totalProjectReviews > 0 ? await projectQuery.AverageAsync(r => r.Rating, cancellationToken) : 0;

            // Get rating distributions with single queries
            var productRatingDist = await productQuery
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var projectRatingDist = await projectQuery
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var productDistLookup = productRatingDist.ToDictionary(r => r.Rating, r => r.Count);
            var projectDistLookup = projectRatingDist.ToDictionary(r => r.Rating, r => r.Count);

            return new RatingOverviewResponse
            {
                AverageProductRating = Math.Round(avgProductRating, 1),
                TotalProductReviews = totalProductReviews,
                AverageProjectRating = Math.Round(avgProjectRating, 1),
                TotalProjectReviews = totalProjectReviews,
                ProductRatingDistribution = new RatingDistribution
                {
                    OneStar = productDistLookup.GetValueOrDefault(1, 0),
                    TwoStar = productDistLookup.GetValueOrDefault(2, 0),
                    ThreeStar = productDistLookup.GetValueOrDefault(3, 0),
                    FourStar = productDistLookup.GetValueOrDefault(4, 0),
                    FiveStar = productDistLookup.GetValueOrDefault(5, 0)
                },
                ProjectRatingDistribution = new RatingDistribution
                {
                    OneStar = projectDistLookup.GetValueOrDefault(1, 0),
                    TwoStar = projectDistLookup.GetValueOrDefault(2, 0),
                    ThreeStar = projectDistLookup.GetValueOrDefault(3, 0),
                    FourStar = projectDistLookup.GetValueOrDefault(4, 0),
                    FiveStar = projectDistLookup.GetValueOrDefault(5, 0)
                }
            };
        }
    }
}
