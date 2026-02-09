using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.Infrastructure.Services
{
    public class PdfReportService : IPdfReportService
    {
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IRepository<OrderItem, int> _orderItemRepository;

        private static readonly string EmberHex = "#FF6B35";
        private static readonly string DarkBg = "#1A1410";

        public PdfReportService(
            IRepository<Order, int> orderRepository,
            IRepository<OrderItem, int> orderItemRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<byte[]> GenerateOrdersReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.AsQueryable()
                .AsNoTracking()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.PurchaseDate >= startDate && o.PurchaseDate <= endDate)
                .OrderByDescending(o => o.PurchaseDate)
                .ToListAsync(cancellationToken);

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var avgOrderValue = orders.Count > 0 ? totalRevenue / orders.Count : 0;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // Header
                    page.Header().Element(header =>
                    {
                        header.Background(EmberHex).Padding(16).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("REIGNITE").FontSize(20).Bold().FontColor(Colors.White);
                                col.Item().Text("Izvještaj o Narudžbama").FontSize(12).FontColor(Colors.White);
                            });
                            row.ConstantItem(180).AlignRight().Column(col =>
                            {
                                col.Item().Text($"Period: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}")
                                    .FontSize(9).FontColor(Colors.White);
                                col.Item().Text($"Generirano: {DateTime.Now:dd.MM.yyyy HH:mm}")
                                    .FontSize(9).FontColor(Colors.White);
                            });
                        });
                    });

                    // Content
                    page.Content().PaddingVertical(16).Column(col =>
                    {
                        // Summary cards
                        col.Item().Row(row =>
                        {
                            SummaryCard(row.RelativeItem(), "Ukupno Narudžbi", orders.Count.ToString());
                            row.ConstantItem(12);
                            SummaryCard(row.RelativeItem(), "Ukupan Prihod", $"{totalRevenue:N2} KM");
                            row.ConstantItem(12);
                            SummaryCard(row.RelativeItem(), "Prosječna Vrijednost", $"{avgOrderValue:N2} KM");
                        });

                        col.Item().PaddingTop(20).Text("Detalji Narudžbi")
                            .FontSize(14).Bold().FontColor(DarkBg);

                        col.Item().PaddingTop(8);

                        // Orders table
                        if (orders.Count > 0)
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(40);   // ID
                                    columns.RelativeColumn(2);    // Customer
                                    columns.RelativeColumn(1.2f); // Date
                                    columns.RelativeColumn(1);    // Status
                                    columns.RelativeColumn(1);    // Items
                                    columns.RelativeColumn(1.2f); // Total
                                });

                                // Header row
                                table.Header(header =>
                                {
                                    TableHeaderCell(header.Cell(), "#");
                                    TableHeaderCell(header.Cell(), "Kupac");
                                    TableHeaderCell(header.Cell(), "Datum");
                                    TableHeaderCell(header.Cell(), "Status");
                                    TableHeaderCell(header.Cell(), "Stavke");
                                    TableHeaderCell(header.Cell(), "Ukupno");
                                });

                                // Data rows
                                foreach (var order in orders)
                                {
                                    var customerName = order.User != null
                                        ? $"{order.User.FirstName} {order.User.LastName}"
                                        : "Nepoznat";
                                    var statusName = GetStatusName(order.Status.ToString());
                                    var itemCount = order.OrderItems?.Sum(oi => oi.Quantity) ?? 0;

                                    TableCell(table.Cell(), order.Id.ToString());
                                    TableCell(table.Cell(), customerName);
                                    TableCell(table.Cell(), order.PurchaseDate.ToString("dd.MM.yyyy"));
                                    TableCell(table.Cell(), statusName);
                                    TableCell(table.Cell(), itemCount.ToString());
                                    TableCell(table.Cell(), $"{order.TotalAmount:N2} KM");
                                }
                            });
                        }
                        else
                        {
                            col.Item().PaddingTop(20).AlignCenter()
                                .Text("Nema narudžbi u odabranom periodu.")
                                .FontSize(11).FontColor(Colors.Grey.Medium);
                        }
                    });

                    // Footer
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Stranica ");
                        text.CurrentPageNumber();
                        text.Span(" od ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        public async Task<byte[]> GenerateRevenueReportAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var orders = await _orderRepository.AsQueryable()
                .AsNoTracking()
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.ProductCategory)
                .Where(o => o.PurchaseDate >= startDate && o.PurchaseDate <= endDate)
                .OrderBy(o => o.PurchaseDate)
                .ToListAsync(cancellationToken);

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;

            // Daily revenue
            var dailyRevenue = orders
                .GroupBy(o => o.PurchaseDate.Date)
                .Select(g => new { Date = g.Key, Revenue = g.Sum(o => o.TotalAmount), Orders = g.Count() })
                .OrderBy(d => d.Date)
                .ToList();

            // Category breakdown
            var categoryRevenue = orders
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => oi.Product?.ProductCategory?.Name ?? "Nepoznato")
                .Select(g => new { Category = g.Key, Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice), Quantity = g.Sum(oi => oi.Quantity) })
                .OrderByDescending(c => c.Revenue)
                .ToList();

            // Top products
            var topProducts = orders
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => new { oi.ProductId, Name = oi.Product?.Name ?? "Nepoznat" })
                .Select(g => new { g.Key.Name, Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice), Quantity = g.Sum(oi => oi.Quantity) })
                .OrderByDescending(p => p.Revenue)
                .Take(10)
                .ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    // Header
                    page.Header().Element(header =>
                    {
                        header.Background(EmberHex).Padding(16).Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("REIGNITE").FontSize(20).Bold().FontColor(Colors.White);
                                col.Item().Text("Izvještaj o Prihodima").FontSize(12).FontColor(Colors.White);
                            });
                            row.ConstantItem(180).AlignRight().Column(col =>
                            {
                                col.Item().Text($"Period: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}")
                                    .FontSize(9).FontColor(Colors.White);
                                col.Item().Text($"Generirano: {DateTime.Now:dd.MM.yyyy HH:mm}")
                                    .FontSize(9).FontColor(Colors.White);
                            });
                        });
                    });

                    // Content
                    page.Content().PaddingVertical(16).Column(col =>
                    {
                        // KPI summary
                        col.Item().Row(row =>
                        {
                            SummaryCard(row.RelativeItem(), "Ukupan Prihod", $"{totalRevenue:N2} KM");
                            row.ConstantItem(12);
                            SummaryCard(row.RelativeItem(), "Ukupno Narudžbi", totalOrders.ToString());
                            row.ConstantItem(12);
                            var avg = totalOrders > 0 ? totalRevenue / totalOrders : 0;
                            SummaryCard(row.RelativeItem(), "Prosječna Narudžba", $"{avg:N2} KM");
                        });

                        // Daily revenue table
                        col.Item().PaddingTop(20).Text("Dnevni Prihod")
                            .FontSize(14).Bold().FontColor(DarkBg);
                        col.Item().PaddingTop(8);

                        if (dailyRevenue.Count > 0)
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(2);  // Date
                                    columns.RelativeColumn(1);  // Orders
                                    columns.RelativeColumn(2);  // Revenue
                                });

                                table.Header(header =>
                                {
                                    TableHeaderCell(header.Cell(), "Datum");
                                    TableHeaderCell(header.Cell(), "Narudžbe");
                                    TableHeaderCell(header.Cell(), "Prihod");
                                });

                                foreach (var day in dailyRevenue)
                                {
                                    TableCell(table.Cell(), day.Date.ToString("dd.MM.yyyy"));
                                    TableCell(table.Cell(), day.Orders.ToString());
                                    TableCell(table.Cell(), $"{day.Revenue:N2} KM");
                                }
                            });
                        }

                        // Category breakdown
                        col.Item().PaddingTop(20).Text("Prihod po Kategorijama")
                            .FontSize(14).Bold().FontColor(DarkBg);
                        col.Item().PaddingTop(8);

                        if (categoryRevenue.Count > 0)
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);  // Category
                                    columns.RelativeColumn(1);  // Quantity
                                    columns.RelativeColumn(2);  // Revenue
                                });

                                table.Header(header =>
                                {
                                    TableHeaderCell(header.Cell(), "Kategorija");
                                    TableHeaderCell(header.Cell(), "Prodano");
                                    TableHeaderCell(header.Cell(), "Prihod");
                                });

                                foreach (var cat in categoryRevenue)
                                {
                                    TableCell(table.Cell(), cat.Category);
                                    TableCell(table.Cell(), cat.Quantity.ToString());
                                    TableCell(table.Cell(), $"{cat.Revenue:N2} KM");
                                }
                            });
                        }

                        // Top products
                        col.Item().PaddingTop(20).Text("Top 10 Proizvoda")
                            .FontSize(14).Bold().FontColor(DarkBg);
                        col.Item().PaddingTop(8);

                        if (topProducts.Count > 0)
                        {
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(30);  // #
                                    columns.RelativeColumn(3);   // Product
                                    columns.RelativeColumn(1);   // Quantity
                                    columns.RelativeColumn(2);   // Revenue
                                });

                                table.Header(header =>
                                {
                                    TableHeaderCell(header.Cell(), "#");
                                    TableHeaderCell(header.Cell(), "Proizvod");
                                    TableHeaderCell(header.Cell(), "Prodano");
                                    TableHeaderCell(header.Cell(), "Prihod");
                                });

                                var rank = 1;
                                foreach (var product in topProducts)
                                {
                                    TableCell(table.Cell(), rank.ToString());
                                    TableCell(table.Cell(), product.Name);
                                    TableCell(table.Cell(), product.Quantity.ToString());
                                    TableCell(table.Cell(), $"{product.Revenue:N2} KM");
                                    rank++;
                                }
                            });
                        }
                    });

                    // Footer
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Stranica ");
                        text.CurrentPageNumber();
                        text.Span(" od ");
                        text.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static void SummaryCard(IContainer container, string label, string value)
        {
            container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(12).Column(col =>
            {
                col.Item().Text(label).FontSize(9).FontColor(Colors.Grey.Medium);
                col.Item().PaddingTop(4).Text(value).FontSize(16).Bold().FontColor(DarkBg);
            });
        }

        private static void TableHeaderCell(IContainer container, string text)
        {
            container.Background(DarkBg).Padding(8)
                .Text(text).FontSize(9).Bold().FontColor(Colors.White);
        }

        private static void TableCell(IContainer container, string text)
        {
            container.BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(8)
                .Text(text).FontSize(9).FontColor(DarkBg);
        }

        private static string GetStatusName(string status) => status switch
        {
            "Processing" => "U obradi",
            "OnDelivery" => "Na dostavi",
            "Delivered" => "Dostavljeno",
            "Cancelled" => "Otkazano",
            _ => status
        };
    }
}
