namespace Reignite.Application.IServices
{
    public interface IPdfReportService
    {
        Task<byte[]> GenerateOrdersReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<byte[]> GenerateRevenueReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    }
}
