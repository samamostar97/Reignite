using Reignite.Application.DTOs.Response;

namespace Reignite.Application.IServices
{
    public interface IReportService
    {
        Task<DashboardReportResponse> GetDashboardReportAsync(CancellationToken cancellationToken = default);
        Task<RevenueSummaryResponse> GetRevenueSummaryAsync(CancellationToken cancellationToken = default);
        Task<List<SalesChartDataPoint>> GetSalesChartAsync(int days = 30, CancellationToken cancellationToken = default);
        Task<List<TopProductResponse>> GetTopProductsAsync(int count = 5, CancellationToken cancellationToken = default);
        Task<List<RecentOrderResponse>> GetRecentOrdersAsync(int count = 10, CancellationToken cancellationToken = default);
        Task<UserGrowthResponse> GetUserGrowthAsync(int days = 30, CancellationToken cancellationToken = default);
        Task<RatingOverviewResponse> GetRatingOverviewAsync(CancellationToken cancellationToken = default);
    }
}
