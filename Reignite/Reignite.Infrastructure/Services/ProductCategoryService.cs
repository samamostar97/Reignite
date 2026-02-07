using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Exceptions;
using Reignite.Application.Filters;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.Infrastructure.Services
{
    public class ProductCategoryService : BaseService<ProductCategory, ProductCategoryResponse, CreateProductCategoryRequest, UpdateProductCategoryRequest, ProductCategoryQueryFilter, int>, IProductCategoryService
    {
        private readonly IRepository<Product, int> _productRepository;

        public ProductCategoryService(
            IRepository<ProductCategory, int> repository,
            IRepository<Product, int> productRepository,
            IMapper mapper) : base(repository, mapper)
        {
            _productRepository = productRepository;
        }

        protected override async Task BeforeCreateAsync(ProductCategory entity, CreateProductCategoryRequest dto)
        {
            var exists = await _repository.AsQueryable()
                .AnyAsync(c => c.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                throw new ConflictException($"Kategorija sa imenom '{dto.Name}' već postoji.");
        }

        protected override async Task BeforeUpdateAsync(ProductCategory entity, UpdateProductCategoryRequest dto)
        {
            var exists = await _repository.AsQueryable()
                .AnyAsync(c => c.Id != entity.Id && c.Name.ToLower() == dto.Name.ToLower());

            if (exists)
                throw new ConflictException($"Kategorija sa imenom '{dto.Name}' već postoji.");
        }

        protected override async Task BeforeDeleteAsync(ProductCategory entity)
        {
            var productCount = await _productRepository.AsQueryable()
                .CountAsync(p => p.ProductCategoryId == entity.Id);

            if (productCount > 0)
                throw new EntityHasDependentsException("kategoriju", "proizvoda", productCount);
        }

        protected override IQueryable<ProductCategory> ApplyFilter(IQueryable<ProductCategory> query, ProductCategoryQueryFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Search))
                query = query.Where(x => x.Name.ToLower().Contains(filter.Search.ToLower()));

            if (!string.IsNullOrEmpty(filter.OrderBy))
            {
                query = filter.OrderBy.ToLower() switch
                {
                    "name" => query.OrderBy(x => x.Name),
                    "namedesc" => query.OrderByDescending(x => x.Name),
                    "createdatdesc" => query.OrderByDescending(x => x.CreatedAt),
                    _ => query.OrderBy(x => x.CreatedAt)
                };
                return query;
            }

            query = query.OrderBy(x => x.CreatedAt);
            return query;
        }
    }
}
