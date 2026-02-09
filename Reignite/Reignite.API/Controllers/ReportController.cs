using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reignite.Application.DTOs.Response;
using Reignite.Application.IServices;

namespace Reignite.API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize(Roles = "Admin")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IPdfReportService _pdfReportService;

        public ReportController(IReportService reportService, IPdfReportService pdfReportService)
        {
            _reportService = reportService;
            _pdfReportService = pdfReportService;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<DashboardReportResponse>> GetDashboardReport()
        {
            var report = await _reportService.GetDashboardReportAsync();
            return Ok(report);
        }

        [HttpGet("revenue")]
        public async Task<ActionResult<RevenueSummaryResponse>> GetRevenueSummary()
        {
            var summary = await _reportService.GetRevenueSummaryAsync();
            return Ok(summary);
        }

        [HttpGet("sales-chart")]
        public async Task<ActionResult<List<SalesChartDataPoint>>> GetSalesChart([FromQuery] int days = 30)
        {
            var chart = await _reportService.GetSalesChartAsync(days);
            return Ok(chart);
        }

        [HttpGet("top-products")]
        public async Task<ActionResult<List<TopProductResponse>>> GetTopProducts([FromQuery] int count = 5)
        {
            var products = await _reportService.GetTopProductsAsync(count);
            return Ok(products);
        }

        [HttpGet("recent-orders")]
        public async Task<ActionResult<List<RecentOrderResponse>>> GetRecentOrders([FromQuery] int count = 10)
        {
            var orders = await _reportService.GetRecentOrdersAsync(count);
            return Ok(orders);
        }

        [HttpGet("user-growth")]
        public async Task<ActionResult<UserGrowthResponse>> GetUserGrowth([FromQuery] int days = 30)
        {
            var growth = await _reportService.GetUserGrowthAsync(days);
            return Ok(growth);
        }

        [HttpGet("ratings")]
        public async Task<ActionResult<RatingOverviewResponse>> GetRatingOverview()
        {
            var ratings = await _reportService.GetRatingOverviewAsync();
            return Ok(ratings);
        }

        [HttpGet("export/orders")]
        public async Task<IActionResult> ExportOrdersReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var pdf = await _pdfReportService.GenerateOrdersReportAsync(startDate, endDate.Date.AddDays(1).AddSeconds(-1));
            return File(pdf, "application/pdf", $"Reignite_Narudzbe_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf");
        }

        [HttpGet("export/revenue")]
        public async Task<IActionResult> ExportRevenueReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var pdf = await _pdfReportService.GenerateRevenueReportAsync(startDate, endDate.Date.AddDays(1).AddSeconds(-1));
            return File(pdf, "application/pdf", $"Reignite_Prihodi_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}.pdf");
        }
    }
}
