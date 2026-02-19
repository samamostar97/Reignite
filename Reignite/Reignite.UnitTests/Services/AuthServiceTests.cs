using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Reignite.Application.DTOs.Request;
using Reignite.Application.Exceptions;
using Reignite.Application.IRepositories;
using Reignite.Application.Options;
using Reignite.Core.Entities;
using Reignite.Core.Enums;
using Reignite.Infrastructure.Data;
using Reignite.Infrastructure.Services;
using Reignite.UnitTests.Helpers;

namespace Reignite.UnitTests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly ReigniteDbContext _context;
    private readonly AuthService _authService;
    private readonly Mock<IRepository<RefreshToken, int>> _refreshTokenRepoMock;
    private readonly JwtSettings _jwtSettings;

    public AuthServiceTests()
    {
        _context = TestDbContextFactory.Create();
        _refreshTokenRepoMock = new Mock<IRepository<RefreshToken, int>>();
        _jwtSettings = new JwtSettings
        {
            Secret = "ThisIsAVeryLongSecretKeyForTestingPurposes123456!",
            Issuer = "ReigniteTest",
            Audience = "ReigniteTestApp",
            AccessTokenExpirationHours = 1,
            RefreshTokenExpirationDays = 7
        };

        var jwtOptions = Options.Create(_jwtSettings);

        _refreshTokenRepoMock.Setup(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _authService = new AuthService(_context, _refreshTokenRepoMock.Object, jwtOptions);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    // ==================== REGISTER TESTS ====================

    [Fact]
    public async Task RegisterAsync_WithValidData_ReturnsAuthResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "password123",
            Username = "testuser",
            PhoneNumber = "+387 61 234 567",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be("test@example.com");
        result.User.FirstName.Should().Be("Test");
        result.User.LastName.Should().Be("User");
        result.User.Role.Should().Be(UserRole.AppUser);
    }

    [Fact]
    public async Task RegisterAsync_CreatesUserInDatabase()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            Password = "securepass",
            Username = "newuser",
            PhoneNumber = "+387 62 345 678",
            FirstName = "New",
            LastName = "User"
        };

        // Act
        await _authService.RegisterAsync(request);

        // Assert
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == "newuser@example.com");
        user.Should().NotBeNull();
        user!.FirstName.Should().Be("New");
        user.LastName.Should().Be("User");
        user.Role.Should().Be(UserRole.AppUser);
        user.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task RegisterAsync_HashesPassword()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "hash@example.com",
            Password = "mypassword",
            Username = "hashuser",
            PhoneNumber = "+387 63 456 789",
            FirstName = "Hash",
            LastName = "Test"
        };

        // Act
        await _authService.RegisterAsync(request);

        // Assert
        var user = await _context.Users.FirstAsync(u => u.Email == "hash@example.com");
        user.PasswordHash.Should().NotBe("mypassword");
        BCrypt.Net.BCrypt.Verify("mypassword", user.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ThrowsConflictException()
    {
        // Arrange
        _context.Users.Add(new User
        {
            Email = "existing@example.com",
            PasswordHash = "hash",
            Username = "existing",
            PhoneNumber = "+387 61 111 111",
            FirstName = "Existing",
            LastName = "User",
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var request = new RegisterRequest
        {
            Email = "existing@example.com",
            Password = "password",
            Username = "newusername",
            PhoneNumber = "+387 62 222 222",
            FirstName = "New",
            LastName = "User"
        };

        // Act & Assert
        await _authService.Invoking(s => s.RegisterAsync(request))
            .Should().ThrowAsync<ConflictException>()
            .WithMessage("*Email*");
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateUsername_ThrowsConflictException()
    {
        // Arrange
        _context.Users.Add(new User
        {
            Email = "first@example.com",
            PasswordHash = "hash",
            Username = "taken",
            PhoneNumber = "+387 61 333 333",
            FirstName = "First",
            LastName = "User",
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var request = new RegisterRequest
        {
            Email = "second@example.com",
            Password = "password",
            Username = "taken",
            PhoneNumber = "+387 62 444 444",
            FirstName = "Second",
            LastName = "User"
        };

        // Act & Assert
        await _authService.Invoking(s => s.RegisterAsync(request))
            .Should().ThrowAsync<ConflictException>()
            .WithMessage("*Username*");
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicatePhone_ThrowsConflictException()
    {
        // Arrange
        _context.Users.Add(new User
        {
            Email = "phone1@example.com",
            PasswordHash = "hash",
            Username = "phone1",
            PhoneNumber = "+387 61 555 555",
            FirstName = "Phone",
            LastName = "One",
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var request = new RegisterRequest
        {
            Email = "phone2@example.com",
            Password = "password",
            Username = "phone2",
            PhoneNumber = "+387 61 555 555",
            FirstName = "Phone",
            LastName = "Two"
        };

        // Act & Assert
        await _authService.Invoking(s => s.RegisterAsync(request))
            .Should().ThrowAsync<ConflictException>()
            .WithMessage("*telefona*");
    }

    [Fact]
    public async Task RegisterAsync_SavesRefreshToken()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "refresh@example.com",
            Password = "password",
            Username = "refreshuser",
            PhoneNumber = "+387 61 666 666",
            FirstName = "Refresh",
            LastName = "Test"
        };

        // Act
        await _authService.RegisterAsync(request);

        // Assert
        _refreshTokenRepoMock.Verify(
            r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ==================== LOGIN TESTS ====================

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsAuthResponse()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        _context.Users.Add(new User
        {
            Email = "login@example.com",
            PasswordHash = passwordHash,
            Username = "loginuser",
            PhoneNumber = "+387 61 777 777",
            FirstName = "Login",
            LastName = "User",
            Role = UserRole.AppUser,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var request = new LoginRequest
        {
            Email = "login@example.com",
            Password = "correctpassword"
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be("login@example.com");
        result.User.FirstName.Should().Be("Login");
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ThrowsUnauthorized()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        _context.Users.Add(new User
        {
            Email = "wrong@example.com",
            PasswordHash = passwordHash,
            Username = "wrongpass",
            PhoneNumber = "+387 61 888 888",
            FirstName = "Wrong",
            LastName = "Pass",
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var request = new LoginRequest
        {
            Email = "wrong@example.com",
            Password = "incorrectpassword"
        };

        // Act & Assert
        await _authService.Invoking(s => s.LoginAsync(request))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentEmail_ThrowsUnauthorized()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = "password"
        };

        // Act & Assert
        await _authService.Invoking(s => s.LoginAsync(request))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task LoginAsync_WithDeletedUser_ThrowsUnauthorized()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password");
        _context.Users.Add(new User
        {
            Email = "deleted@example.com",
            PasswordHash = passwordHash,
            Username = "deleteduser",
            PhoneNumber = "+387 61 999 999",
            FirstName = "Deleted",
            LastName = "User",
            IsDeleted = true,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        var request = new LoginRequest
        {
            Email = "deleted@example.com",
            Password = "password"
        };

        // Act & Assert
        await _authService.Invoking(s => s.LoginAsync(request))
            .Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task LoginAsync_ReturnsValidJwtToken()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password");
        _context.Users.Add(new User
        {
            Email = "jwt@example.com",
            PasswordHash = passwordHash,
            Username = "jwtuser",
            PhoneNumber = "+387 62 111 111",
            FirstName = "Jwt",
            LastName = "Test",
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _authService.LoginAsync(new LoginRequest
        {
            Email = "jwt@example.com",
            Password = "password"
        });

        // Assert — JWT should have 3 parts separated by dots
        result.AccessToken.Split('.').Should().HaveCount(3);
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        result.User.Role.Should().Be(UserRole.Admin);
    }
}
