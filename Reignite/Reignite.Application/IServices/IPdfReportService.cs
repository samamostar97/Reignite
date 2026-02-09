namespace Reignite.Application.IServices
{
    public interface IPdfReportService
    {
        Task<byte[]> GenerateOrdersReportAsync(DateTime startDate, DateTime endDate);
        Task<byte[]> GenerateRevenueReportAsync(DateTime startDate, DateTime endDate);
    }
}
