using Reignite.Application.DTOs.Response;

namespace Reignite.Application.IServices
{
    public interface IReportService
    {
        Task<DashboardReportResponse> GetDashboardReportAsync();
        Task<RevenueSummaryResponse> GetRevenueSummaryAsync();
        Task<List<SalesChartDataPoint>> GetSalesChartAsync(int days = 30);
        Task<List<TopProductResponse>> GetTopProductsAsync(int count = 5);
        Task<List<RecentOrderResponse>> GetRecentOrdersAsync(int count = 10);
        Task<UserGrowthResponse> GetUserGrowthAsync(int days = 30);
        Task<RatingOverviewResponse> GetRatingOverviewAsync();
    }
}
