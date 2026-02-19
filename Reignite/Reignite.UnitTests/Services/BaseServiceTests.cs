using FluentAssertions;
using MapsterMapper;
using Moq;
using Reignite.Application.Common;
using Reignite.Application.IRepositories;
using Reignite.Core.Entities;
using Reignite.Infrastructure.Services;

namespace Reignite.UnitTests.Services;

// Concrete implementation for testing the abstract BaseService
public class TestProductService : BaseService<Product, ProductTestDto, CreateProductTestDto, UpdateProductTestDto, TestQueryFilter, int>
{
    public TestProductService(IRepository<Product, int> repository, IMapper mapper)
        : base(repository, mapper) { }

    protected override IQueryable<Product> ApplyFilter(IQueryable<Product> query, TestQueryFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.Search))
            query = query.Where(p => p.Name.Contains(filter.Search));
        return query;
    }
}

// Test DTOs
public class ProductTestDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class CreateProductTestDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class UpdateProductTestDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class TestQueryFilter : PaginationRequest
{
    public string? Search { get; set; }
}

public class BaseServiceTests
{
    private readonly Mock<IRepository<Product, int>> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TestProductService _service;

    public BaseServiceTests()
    {
        _repoMock = new Mock<IRepository<Product, int>>();
        _mapperMock = new Mock<IMapper>();
        _service = new TestProductService(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsMappedDto()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test Product", Price = 10.0m };
        var dto = new ProductTestDto { Id = 1, Name = "Test Product", Price = 10.0m };

        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mapperMock.Setup(m => m.Map<ProductTestDto>(product))
            .Returns(dto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test Product");
        result.Price.Should().Be(10.0m);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Entity not found"));

        // Act & Assert
        await _service.Invoking(s => s.GetByIdAsync(999))
            .Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_ValidDto_CallsRepositoryAndReturnsDto()
    {
        // Arrange
        var createDto = new CreateProductTestDto { Name = "New Product", Price = 25.0m };
        var entity = new Product { Id = 0, Name = "New Product", Price = 25.0m };
        var resultDto = new ProductTestDto { Id = 1, Name = "New Product", Price = 25.0m };

        _mapperMock.Setup(m => m.Map<Product>(createDto)).Returns(entity);
        _mapperMock.Setup(m => m.Map<ProductTestDto>(entity)).Returns(resultDto);
        _repoMock.Setup(r => r.AddAsync(entity, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Product");
        _repoMock.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ExistingId_CallsRepository()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "To Delete", Price = 5.0m };
        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _repoMock.Setup(r => r.DeleteAsync(product, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _repoMock.Verify(r => r.DeleteAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ExistingId_MapsAndUpdates()
    {
        // Arrange
        var existing = new Product { Id = 1, Name = "Old Name", Price = 10.0m };
        var updateDto = new UpdateProductTestDto { Name = "New Name", Price = 20.0m };
        var resultDto = new ProductTestDto { Id = 1, Name = "New Name", Price = 20.0m };

        _repoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);
        _mapperMock.Setup(m => m.Map(updateDto, existing)).Returns(existing);
        _repoMock.Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<ProductTestDto>(existing)).Returns(resultDto);

        // Act
        var result = await _service.UpdateAsync(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("New Name");
        _repoMock.Verify(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsPagedResult()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Product A", Price = 10.0m },
            new() { Id = 2, Name = "Product B", Price = 20.0m }
        };
        var pagedResult = new PagedResult<Product>
        {
            Items = products,
            TotalCount = 2,
            PageNumber = 1
        };
        var dtos = new List<ProductTestDto>
        {
            new() { Id = 1, Name = "Product A", Price = 10.0m },
            new() { Id = 2, Name = "Product B", Price = 20.0m }
        };
        var filter = new TestQueryFilter { PageNumber = 1, PageSize = 10 };

        _repoMock.Setup(r => r.AsQueryable()).Returns(products.AsQueryable());
        _repoMock.Setup(r => r.GetPagedAsync(It.IsAny<IQueryable<Product>>(), filter, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult);
        _mapperMock.Setup(m => m.Map<List<ProductTestDto>>(products)).Returns(dtos);

        // Act
        var result = await _service.GetPagedAsync(filter);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.PageNumber.Should().Be(1);
    }

    [Fact]
    public async Task GetPagedAsync_WithSearchFilter_FiltersResults()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Apple Watch", Price = 399.0m },
            new() { Id = 2, Name = "Samsung Galaxy", Price = 299.0m },
            new() { Id = 3, Name = "Apple iPhone", Price = 999.0m }
        };
        var filter = new TestQueryFilter { Search = "Apple", PageNumber = 1, PageSize = 10 };

        _repoMock.Setup(r => r.AsQueryable()).Returns(products.AsQueryable());
        _repoMock.Setup(r => r.GetPagedAsync(It.IsAny<IQueryable<Product>>(), filter, It.IsAny<CancellationToken>()))
            .Returns((IQueryable<Product> q, PaginationRequest _, CancellationToken _) =>
            {
                var items = q.ToList();
                return Task.FromResult(new PagedResult<Product>
                {
                    Items = items,
                    TotalCount = items.Count,
                    PageNumber = 1
                });
            });
        _mapperMock.Setup(m => m.Map<List<ProductTestDto>>(It.IsAny<List<Product>>()))
            .Returns((List<Product> src) => src.Select(p => new ProductTestDto
            {
                Id = p.Id, Name = p.Name, Price = p.Price
            }).ToList());

        // Act
        var result = await _service.GetPagedAsync(filter);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().AllSatisfy(item => item.Name.Should().Contain("Apple"));
    }
}
