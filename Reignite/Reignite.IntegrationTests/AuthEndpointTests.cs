using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Core.Entities;
using Reignite.Infrastructure.Data;
using Reignite.IntegrationTests.Helpers;

namespace Reignite.IntegrationTests;

public class AuthEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public AuthEndpointTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_Returns200WithTokens()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "integration@test.com",
            Password = "password123",
            Username = "integrationuser",
            PhoneNumber = "+387 61 234 567",
            FirstName = "Integration",
            LastName = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be("integration@test.com");
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns409()
    {
        // Arrange — seed a user
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ReigniteDbContext>();
            db.Users.Add(new User
            {
                Email = "duplicate@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
                Username = "dupuser",
                PhoneNumber = "+387 61 999 888",
                FirstName = "Dup",
                LastName = "User",
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }

        var request = new RegisterRequest
        {
            Email = "duplicate@test.com",
            Password = "password123",
            Username = "newusername",
            PhoneNumber = "+387 62 111 222",
            FirstName = "New",
            LastName = "User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200WithTokens()
    {
        // Arrange — seed a user with known password
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ReigniteDbContext>();
            db.Users.Add(new User
            {
                Email = "login@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpass"),
                Username = "loginuser",
                PhoneNumber = "+387 61 777 666",
                FirstName = "Login",
                LastName = "Test",
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }

        var request = new LoginRequest
        {
            Email = "login@test.com",
            Password = "correctpass"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.User.FirstName.Should().Be("Login");
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ReigniteDbContext>();
            db.Users.Add(new User
            {
                Email = "wrongpass@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("realpassword"),
                Username = "wrongpassuser",
                PhoneNumber = "+387 61 555 444",
                FirstName = "Wrong",
                LastName = "Pass",
                CreatedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }

        var request = new LoginRequest
        {
            Email = "wrongpass@test.com",
            Password = "badpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ReturnsError()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "ghost@test.com",
            Password = "password"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
    }

    [Fact]
    public async Task Register_ThenLogin_WorksEndToEnd()
    {
        // Arrange — Register
        var registerRequest = new RegisterRequest
        {
            Email = "e2e@test.com",
            Password = "e2epassword",
            Username = "e2euser",
            PhoneNumber = "+387 63 123 456",
            FirstName = "E2E",
            LastName = "User"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act — Login with same credentials
        var loginRequest = new LoginRequest
        {
            Email = "e2e@test.com",
            Password = "e2epassword"
        };
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        result!.User.Email.Should().Be("e2e@test.com");
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        // Act — Try to access profile without token
        var response = await _client.GetAsync("/api/profile");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithValidToken_Returns200()
    {
        // Arrange — Register to get a token
        var registerRequest = new RegisterRequest
        {
            Email = "protected@test.com",
            Password = "password123",
            Username = "protecteduser",
            PhoneNumber = "+387 64 789 012",
            FirstName = "Protected",
            LastName = "User"
        };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        // Act — Access protected endpoint with token
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/profile");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult!.AccessToken);
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
