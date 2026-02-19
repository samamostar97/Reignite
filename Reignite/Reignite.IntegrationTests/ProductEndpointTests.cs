using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Common;
using Reignite.Core.Entities;
using Reignite.Core.Enums;
using Reignite.Infrastructure.Data;
using Reignite.IntegrationTests.Helpers;

namespace Reignite.IntegrationTests;

public class ProductEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public ProductEndpointTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> GetAdminTokenAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReigniteDbContext>();

        // Check if admin already exists
        var existing = db.Users.FirstOrDefault(u => u.Email == "admin@products.test");
        if (existing == null)
        {
            db.Users.Add(new User
            {
                Email = "admin@products.test",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("adminpass"),
                Username = "productadmin",
                PhoneNumber = "+387 61 100 100",
                FirstName = "Admin",
                LastName = "Test",
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "admin@products.test",
            Password = "adminpass"
        });

        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        return auth!.AccessToken;
    }

    private async Task<(int CategoryId, int SupplierId)> SeedCategoryAndSupplier()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReigniteDbContext>();

        var cat = db.ProductCategories.FirstOrDefault(c => c.Name == "Electronics");
        var sup = db.Suppliers.FirstOrDefault(s => s.Name == "TestSupplier");

        if (cat == null)
        {
            cat = new ProductCategory { Name = "Electronics", CreatedAt = DateTime.UtcNow };
            db.ProductCategories.Add(cat);
            sup = new Supplier { Name = "TestSupplier", CreatedAt = DateTime.UtcNow };
            db.Suppliers.Add(sup);
            await db.SaveChangesAsync();
        }

        return (cat!.Id, sup!.Id);
    }

    [Fact]
    public async Task GetProducts_ReturnsPagedResult()
    {
        // Arrange
        var (catId, supId) = await SeedCategoryAndSupplier();
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ReigniteDbContext>();
            if (!db.Products.Any(p => p.Name == "TestProduct1"))
            {
                db.Products.Add(new Product
                {
                    Name = "TestProduct1", Price = 29.99m,
                    ProductCategoryId = catId, SupplierId = supId, CreatedAt = DateTime.UtcNow
                });
                db.Products.Add(new Product
                {
                    Name = "TestProduct2", Price = 49.99m,
                    ProductCategoryId = catId, SupplierId = supId, CreatedAt = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
            }
        }

        // Act
        var response = await _client.GetAsync("/api/products?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductResponse>>();
        result.Should().NotBeNull();
        result!.Items.Should().NotBeEmpty();
        result.TotalCount.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetProductById_ReturnsProduct()
    {
        // Arrange
        var (catId, supId) = await SeedCategoryAndSupplier();
        int productId;
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ReigniteDbContext>();
            var product = new Product
            {
                Name = "SpecificProduct", Price = 99.99m,
                ProductCategoryId = catId, SupplierId = supId, CreatedAt = DateTime.UtcNow
            };
            db.Products.Add(product);
            await db.SaveChangesAsync();
            productId = product.Id;
        }

        // Act
        var response = await _client.GetAsync($"/api/products/{productId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProductById_NonExistent_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/products/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProducts_WithSearchFilter_FiltersResults()
    {
        // Arrange
        var (catId, supId) = await SeedCategoryAndSupplier();
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ReigniteDbContext>();
            if (!db.Products.Any(p => p.Name == "UniqueSearchableProduct"))
            {
                db.Products.Add(new Product
                {
                    Name = "UniqueSearchableProduct", Price = 15.00m,
                    ProductCategoryId = catId, SupplierId = supId, CreatedAt = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
            }
        }

        // Act
        var response = await _client.GetAsync("/api/products?search=UniqueSearchable&pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<ProductResponse>>();
        result!.Items.Should().Contain(p => p.Name.Contains("UniqueSearchable"));
    }

    [Fact]
    public async Task CreateProduct_WithoutAuth_Returns401()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/products", new { Name = "Unauthorized", Price = 10m });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

// Minimal DTO for deserialization
public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? ProductImageUrl { get; set; }
}
