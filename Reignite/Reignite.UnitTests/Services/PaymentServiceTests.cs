using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Common;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;
using Reignite.Infrastructure.Data;
using Reignite.Infrastructure.Services;
using Reignite.UnitTests.Helpers;

namespace Reignite.UnitTests.Services;

/// <summary>
/// Wrapper repository that uses InMemory DbContext for PaymentService testing.
/// PaymentService uses ToDictionaryAsync which requires EF Core IAsyncQueryProvider.
/// </summary>
public class InMemoryProductRepository : IRepository<Product, int>
{
    private readonly ReigniteDbContext _context;

    public InMemoryProductRepository(ReigniteDbContext context) => _context = context;

    public IQueryable<Product> AsQueryable() => _context.Products.AsQueryable();
    public async Task<Product> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _context.Products.FindAsync(new object[] { id }, ct) ?? throw new KeyNotFoundException();
    public async Task AddAsync(Product entity, CancellationToken ct = default) { _context.Products.Add(entity); await _context.SaveChangesAsync(ct); }
    public async Task UpdateAsync(Product entity, CancellationToken ct = default) { _context.Products.Update(entity); await _context.SaveChangesAsync(ct); }
    public async Task DeleteAsync(Product entity, CancellationToken ct = default) { _context.Products.Remove(entity); await _context.SaveChangesAsync(ct); }
    public Task<PagedResult<Product>> GetPagedAsync(IQueryable<Product> query, PaginationRequest request, CancellationToken ct = default)
        => throw new NotImplementedException();
}

public class PaymentServiceTests : IDisposable
{
    private readonly ReigniteDbContext _context;
    private readonly Mock<ICouponService> _couponServiceMock;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _context = TestDbContextFactory.Create();
        _couponServiceMock = new Mock<ICouponService>();
        var productRepo = new InMemoryProductRepository(_context);
        _paymentService = new PaymentService(productRepo, _couponServiceMock.Object);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private async Task SeedProducts(params Product[] products)
    {
        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();
    }

    // ==================== CalculateSubtotal TESTS ====================

    [Fact]
    public async Task CalculateSubtotalAsync_SingleItem_ReturnsCorrectTotal()
    {
        // Arrange
        await SeedProducts(new Product { Id = 1, Name = "Product A", Price = 25.00m, CreatedAt = DateTime.UtcNow });
        var items = new List<CreateOrderItemRequest>
        {
            new() { ProductId = 1, Quantity = 3 }
        };

        // Act
        var result = await _paymentService.CalculateSubtotalAsync(items);

        // Assert
        result.Should().Be(75.00m);
    }

    [Fact]
    public async Task CalculateSubtotalAsync_MultipleItems_ReturnsCorrectTotal()
    {
        // Arrange
        await SeedProducts(
            new Product { Id = 1, Name = "Product A", Price = 10.00m, CreatedAt = DateTime.UtcNow },
            new Product { Id = 2, Name = "Product B", Price = 20.00m, CreatedAt = DateTime.UtcNow },
            new Product { Id = 3, Name = "Product C", Price = 5.50m, CreatedAt = DateTime.UtcNow }
        );
        var items = new List<CreateOrderItemRequest>
        {
            new() { ProductId = 1, Quantity = 2 },  // 20.00
            new() { ProductId = 2, Quantity = 1 },  // 20.00
            new() { ProductId = 3, Quantity = 4 }   // 22.00
        };

        // Act
        var result = await _paymentService.CalculateSubtotalAsync(items);

        // Assert
        result.Should().Be(62.00m);
    }

    [Fact]
    public async Task CalculateSubtotalAsync_NonExistentProduct_ThrowsKeyNotFoundException()
    {
        // Arrange
        await SeedProducts(new Product { Id = 1, Name = "Product A", Price = 10.00m, CreatedAt = DateTime.UtcNow });
        var items = new List<CreateOrderItemRequest>
        {
            new() { ProductId = 999, Quantity = 1 }
        };

        // Act & Assert
        await _paymentService.Invoking(s => s.CalculateSubtotalAsync(items))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*999*");
    }

    [Fact]
    public async Task CalculateSubtotalAsync_EmptyList_ReturnsZero()
    {
        // Arrange — no items
        var items = new List<CreateOrderItemRequest>();

        // Act
        var result = await _paymentService.CalculateSubtotalAsync(items);

        // Assert
        result.Should().Be(0m);
    }

    [Fact]
    public async Task CalculateSubtotalAsync_SingleItemQuantityOne_ReturnsPrice()
    {
        // Arrange
        await SeedProducts(new Product { Id = 5, Name = "Expensive", Price = 149.99m, CreatedAt = DateTime.UtcNow });
        var items = new List<CreateOrderItemRequest>
        {
            new() { ProductId = 5, Quantity = 1 }
        };

        // Act
        var result = await _paymentService.CalculateSubtotalAsync(items);

        // Assert
        result.Should().Be(149.99m);
    }

    [Fact]
    public async Task CalculateSubtotalAsync_DuplicateProductIds_CalculatesEachLine()
    {
        // Arrange
        await SeedProducts(new Product { Id = 1, Name = "Product A", Price = 10.00m, CreatedAt = DateTime.UtcNow });
        var items = new List<CreateOrderItemRequest>
        {
            new() { ProductId = 1, Quantity = 2 },
            new() { ProductId = 1, Quantity = 3 }
        };

        // Act
        var result = await _paymentService.CalculateSubtotalAsync(items);

        // Assert
        result.Should().Be(50.00m);  // (2 * 10) + (3 * 10)
    }
}
