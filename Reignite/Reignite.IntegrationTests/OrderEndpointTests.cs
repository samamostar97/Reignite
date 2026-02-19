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

public class OrderEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public OrderEndpointTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<(string Token, int UserId)> GetAdminTokenAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReigniteDbContext>();

        var existing = db.Users.FirstOrDefault(u => u.Email == "admin@orders.test");
        if (existing == null)
        {
            existing = new User
            {
                Email = "admin@orders.test",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("adminpass"),
                Username = "orderadmin",
                PhoneNumber = "+387 61 200 200",
                FirstName = "Order",
                LastName = "Admin",
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            };
            db.Users.Add(existing);
            await db.SaveChangesAsync();
        }

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "admin@orders.test",
            Password = "adminpass"
        });

        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        return (auth!.AccessToken, existing.Id);
    }

    private async Task SeedTestData()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReigniteDbContext>();

        if (!db.ProductCategories.Any(c => c.Name == "OrderTestCat"))
        {
            var cat = new ProductCategory { Name = "OrderTestCat", CreatedAt = DateTime.UtcNow };
            db.ProductCategories.Add(cat);
            var sup = new Supplier { Name = "OrderTestSup", CreatedAt = DateTime.UtcNow };
            db.Suppliers.Add(sup);
            await db.SaveChangesAsync();

            db.Products.Add(new Product
            {
                Name = "OrderTestProduct", Price = 50.00m,
                ProductCategoryId = cat.Id, SupplierId = sup.Id, CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task GetOrders_WithoutAuth_Returns401()
    {
        // Act
        var response = await _client.GetAsync("/api/orders");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetOrders_WithAuth_Returns200()
    {
        // Arrange
        var (token, _) = await GetAdminTokenAsync();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/orders?pageNumber=1&pageSize=10");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetOrderById_NonExistent_Returns404()
    {
        // Arrange
        var (token, _) = await GetAdminTokenAsync();

        // Act
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/orders/99999");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateOrder_WithAuth_Returns200()
    {
        // Arrange
        await SeedTestData();
        var (token, userId) = await GetAdminTokenAsync();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ReigniteDbContext>();
        var product = db.Products.First(p => p.Name == "OrderTestProduct");

        var orderRequest = new CreateOrderRequest
        {
            UserId = userId,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = product.Id, Quantity = 2 }
            }
        };

        // Act
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/orders");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(orderRequest);
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetFaqs_PublicEndpoint_Returns200()
    {
        // Act — FAQ is a public endpoint
        var response = await _client.GetAsync("/api/faqs?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetHobbies_PublicEndpoint_Returns200()
    {
        // Act
        var response = await _client.GetAsync("/api/hobbies?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetProjects_PublicEndpoint_Returns200()
    {
        // Act
        var response = await _client.GetAsync("/api/projects?pageNumber=1&pageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
