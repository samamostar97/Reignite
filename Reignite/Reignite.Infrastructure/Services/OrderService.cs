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
    public class OrderService : BaseService<Order, OrderResponse, CreateOrderRequest, UpdateOrderRequest, OrderQueryFilter, int>, IOrderService
    {
        private readonly IRepository<Product, int> _productRepository;

        public OrderService(
            IRepository<Order, int> repository,
            IRepository<Product, int> productRepository,
            IMapper mapper) : base(repository, mapper)
        {
            _productRepository = productRepository;
        }

        public override async Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
        {
            // Get product prices for calculating total
            var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _productRepository.AsQueryable()
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.Price, cancellationToken);

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
                StripePaymentId = request.StripePaymentIntentId,
                OrderItems = orderItems
            };

            await _repository.AddAsync(order, cancellationToken);

            // Reload with navigation properties
            return await GetByIdAsync(order.Id, cancellationToken);
        }

        public override async Task<OrderResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var order = await _repository.AsQueryable()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

            if (order == null)
                throw new KeyNotFoundException("Narudžba nije pronađena");

            return MapToResponse(order);
        }

        public override async Task<PagedResult<OrderResponse>> GetPagedAsync(OrderQueryFilter filter, CancellationToken cancellationToken = default)
        {
            IQueryable<Order> query = _repository.AsQueryable()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product);

            query = ApplyFilter(query, filter);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<OrderResponse>
            {
                Items = items.Select(MapToResponse).ToList(),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber
            };
        }

        protected override IQueryable<Order> ApplyFilter(IQueryable<Order> query, OrderQueryFilter filter)
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

        protected override async Task BeforeUpdateAsync(Order entity, UpdateOrderRequest dto, CancellationToken cancellationToken = default)
        {
            // Validate status transition if needed
            if (entity.Status == OrderStatus.Cancelled && dto.Status != OrderStatus.Cancelled)
            {
                throw new InvalidOperationException("Otkazane narudžbe ne mogu biti vraćene.");
            }
            await Task.CompletedTask;
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
                StripePaymentId = order.StripePaymentId,
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
