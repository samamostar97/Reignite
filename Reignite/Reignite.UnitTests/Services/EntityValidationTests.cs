using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Reignite.Application.DTOs.Request;
using Reignite.Core.Entities;
using Reignite.Core.Enums;

namespace Reignite.UnitTests.Services;

public class EntityValidationTests
{
    private static List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }

    // ==================== LoginRequest Validation ====================

    [Fact]
    public void LoginRequest_ValidData_PassesValidation()
    {
        var request = new LoginRequest { Email = "test@test.com", Password = "password" };
        ValidateModel(request).Should().BeEmpty();
    }

    [Fact]
    public void LoginRequest_MissingEmail_FailsValidation()
    {
        var request = new LoginRequest { Email = "", Password = "password" };
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void LoginRequest_MissingPassword_FailsValidation()
    {
        var request = new LoginRequest { Email = "test@test.com", Password = "" };
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("Password"));
    }

    // ==================== RegisterRequest Validation ====================

    [Fact]
    public void RegisterRequest_ValidData_PassesValidation()
    {
        var request = new RegisterRequest
        {
            Email = "test@test.com",
            Password = "password123",
            Username = "testuser",
            PhoneNumber = "+387 61 234 567",
            FirstName = "Test",
            LastName = "User"
        };
        ValidateModel(request).Should().BeEmpty();
    }

    [Fact]
    public void RegisterRequest_ShortPassword_FailsValidation()
    {
        var request = new RegisterRequest
        {
            Email = "test@test.com",
            Password = "12345", // too short
            Username = "testuser",
            PhoneNumber = "+387 61 234 567",
            FirstName = "Test",
            LastName = "User"
        };
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("Password"));
    }

    [Fact]
    public void RegisterRequest_InvalidEmail_FailsValidation()
    {
        var request = new RegisterRequest
        {
            Email = "notanemail",
            Password = "password123",
            Username = "testuser",
            PhoneNumber = "+387 61 234 567",
            FirstName = "Test",
            LastName = "User"
        };
        var results = ValidateModel(request);
        results.Should().NotBeEmpty();
    }

    [Fact]
    public void RegisterRequest_ShortUsername_FailsValidation()
    {
        var request = new RegisterRequest
        {
            Email = "test@test.com",
            Password = "password123",
            Username = "ab", // too short (min 3)
            PhoneNumber = "+387 61 234 567",
            FirstName = "Test",
            LastName = "User"
        };
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("Username"));
    }

    [Fact]
    public void RegisterRequest_MissingAllFields_FailsMultipleValidations()
    {
        var request = new RegisterRequest();
        var results = ValidateModel(request);
        results.Should().HaveCountGreaterThanOrEqualTo(4);
    }

    // ==================== CreateOrderRequest Validation ====================

    [Fact]
    public void CreateOrderRequest_ValidData_PassesValidation()
    {
        var request = new CreateOrderRequest
        {
            UserId = 1,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = 1, Quantity = 2 }
            }
        };
        ValidateModel(request).Should().BeEmpty();
    }

    [Fact]
    public void CreateOrderRequest_ZeroUserId_FailsValidation()
    {
        var request = new CreateOrderRequest
        {
            UserId = 0,
            Items = new List<CreateOrderItemRequest>
            {
                new() { ProductId = 1, Quantity = 1 }
            }
        };
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("UserId"));
    }

    [Fact]
    public void CreateOrderItemRequest_ZeroQuantity_FailsValidation()
    {
        var request = new CreateOrderItemRequest { ProductId = 1, Quantity = 0 };
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("Quantity"));
    }

    [Fact]
    public void CreateOrderItemRequest_QuantityExceedsMax_FailsValidation()
    {
        var request = new CreateOrderItemRequest { ProductId = 1, Quantity = 101 };
        var results = ValidateModel(request);
        results.Should().Contain(r => r.MemberNames.Contains("Quantity"));
    }

    // ==================== Entity Model Tests ====================

    [Fact]
    public void User_DefaultValues_AreCorrect()
    {
        var user = new User();
        user.IsDeleted.Should().BeFalse();
        user.Id.Should().Be(0);
        user.Role.Should().Be(UserRole.AppUser);
    }

    [Fact]
    public void Order_DefaultStatus_IsProcessing()
    {
        var order = new Order();
        order.Status.Should().Be(OrderStatus.Processing);
    }

    [Fact]
    public void RefreshToken_IsActive_WhenNotRevokedAndNotExpired()
    {
        var token = new RefreshToken
        {
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            RevokedAt = null
        };
        token.IsRevoked.Should().BeFalse();
        token.IsExpired.Should().BeFalse();
        token.IsActive.Should().BeTrue();
    }

    [Fact]
    public void RefreshToken_IsNotActive_WhenRevoked()
    {
        var token = new RefreshToken
        {
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            RevokedAt = DateTime.UtcNow
        };
        token.IsRevoked.Should().BeTrue();
        token.IsActive.Should().BeFalse();
    }

    [Fact]
    public void RefreshToken_IsNotActive_WhenExpired()
    {
        var token = new RefreshToken
        {
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            RevokedAt = null
        };
        token.IsExpired.Should().BeTrue();
        token.IsActive.Should().BeFalse();
    }

    // ==================== PaginationRequest Validation ====================

    [Fact]
    public void PaginationRequest_Defaults_AreCorrect()
    {
        var request = new Application.Common.PaginationRequest();
        request.PageNumber.Should().Be(1);
        request.PageSize.Should().Be(10);
    }

    [Fact]
    public void PaginationRequest_ValidRange_PassesValidation()
    {
        var request = new Application.Common.PaginationRequest { PageNumber = 5, PageSize = 50 };
        ValidateModel(request).Should().BeEmpty();
    }
}
