using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;
using Reignite.Core.Enums;

namespace Reignite.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order, int> _orderRepository;
        private readonly IRepository<Product, int> _productRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IRepository<Order, int> repository,
            IRepository<Product, int> productRepository,
            IMapper mapper)
        {
            _orderRepository = repository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<OrderResponse>> GetPagedAsync(OrderQueryFilter filter)
        {
            var query = _orderRepository.AsQueryable()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsQueryable();

            query = ApplyFilter(query, filter);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new PagedResult<OrderResponse>
            {
                Items = items.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber
            };
        }

        public async Task<OrderResponse> GetByIdAsync(int id)
        {
            var order = await _orderRepository.AsQueryable()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                throw new KeyNotFoundException("Narudžba nije pronađena");

            return MapToResponse(order);
        }

        public async Task<OrderResponse> CreateTestOrderAsync(CreateOrderRequest request)
        {
            // Get product prices for calculating total
            var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _productRepository.AsQueryable()
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.Price);

            // Validate all products exist
            var missingProducts = productIds.Where(id => !products.ContainsKey(id)).ToList();
            if (missingProducts.Any())
                throw new KeyNotFoundException($"Proizvodi nisu pronađeni: {string.Join(", ", missingProducts)}");

            // Create order items with prices
            var orderItems = request.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = products[item.ProductId]
            }).ToList();

            // Calculate total amount
            var totalAmount = orderItems.Sum(oi => oi.Quantity * oi.UnitPrice);

            // Create order
            var order = new Order
            {
                UserId = request.UserId,
                TotalAmount = totalAmount,
                PurchaseDate = DateTime.UtcNow,
                Status = OrderStatus.Processing,
                OrderItems = orderItems
            };

            await _orderRepository.AddAsync(order);

            // Reload with navigation properties
            return await GetByIdAsync(order.Id);
        }

        private IQueryable<Order> ApplyFilter(IQueryable<Order> query, OrderQueryFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Search))
            {
                var searchLower = filter.Search.ToLower();
                query = query.Where(o =>
                    o.User.FirstName.ToLower().Contains(searchLower) ||
                    o.User.LastName.ToLower().Contains(searchLower) ||
                    o.User.Email.ToLower().Contains(searchLower));
            }

            if (filter.UserId.HasValue)
                query = query.Where(o => o.UserId == filter.UserId.Value);

            if (filter.Status.HasValue)
                query = query.Where(o => o.Status == filter.Status.Value);

            if (!string.IsNullOrEmpty(filter.OrderBy))
            {
                query = filter.OrderBy.ToLower() switch
                {
                    "date" => query.OrderBy(o => o.PurchaseDate),
                    "datedesc" => query.OrderByDescending(o => o.PurchaseDate),
                    "amount" => query.OrderBy(o => o.TotalAmount),
                    "amountdesc" => query.OrderByDescending(o => o.TotalAmount),
                    _ => query.OrderByDescending(o => o.PurchaseDate)
                };
            }
            else
            {
                query = query.OrderByDescending(o => o.PurchaseDate);
            }

            return query;
        }

        private OrderResponse MapToResponse(Order order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User != null
                    ? $"{order.User.FirstName} {order.User.LastName}"
                    : string.Empty,
                UserProfileImageUrl = order.User?.ProfileImageUrl,
                TotalAmount = order.TotalAmount,
                PurchaseDate = order.PurchaseDate,
                Status = order.Status,
                ItemCount = order.OrderItems?.Sum(oi => oi.Quantity) ?? 0,
                Items = order.OrderItems?.Select(oi => new OrderItemResponse
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? string.Empty,
                    ProductImageUrl = oi.Product?.ProductImageUrl,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList() ?? new List<OrderItemResponse>()
            };
        }
    }
}
