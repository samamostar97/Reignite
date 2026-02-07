using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Infrastructure.Services
{
    public class ProductService : BaseService<Product, ProductResponse, CreateProductRequest, UpdateProductRequest, ProductQueryFilter, int>,IProductService
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IRepository<OrderItem, int> _orderItemRepository;

        public ProductService(
            IRepository<Product, int> repository,
            IMapper mapper,
            IFileStorageService fileStorageService,
            IRepository<OrderItem, int> orderItemRepository) : base(repository, mapper)
        {
            _fileStorageService = fileStorageService;
            _orderItemRepository = orderItemRepository;
        }

        public override async Task<ProductResponse> GetByIdAsync(int id)
        {
            var product = await _repository.AsQueryable()
                .Include(x => x.ProductCategory)
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
                throw new KeyNotFoundException("Proizvod nije pronađen.");

            return _mapper.Map<ProductResponse>(product);
        }

        public async Task<ProductResponse> CreateWithImageAsync(CreateProductRequest dto, FileUploadRequest? imageRequest)
        {
            var product = _mapper.Map<Product>(dto);
            await _repository.AddAsync(product);

            if (imageRequest != null && imageRequest.FileStream != null && imageRequest.FileSize > 0)
            {
                var uploadResult = await _fileStorageService.UploadAsync(imageRequest, "products", product.Id.ToString());

                if (uploadResult.Success)
                {
                    product.ProductImageUrl = uploadResult.FileUrl;
                    await _repository.UpdateAsync(product);
                }
            }

            var result = await _repository.AsQueryable()
                .Include(x => x.ProductCategory)
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.Id == product.Id);

            return _mapper.Map<ProductResponse>(result!);
        }

        protected override IQueryable<Product> ApplyFilter(IQueryable<Product> query, ProductQueryFilter filter)
        {
            query = query.Include(x => x.ProductCategory).Include(x=>x.Supplier);

            if(!string.IsNullOrEmpty(filter.Search))
                query = query.Where(x => x.Name.ToLower().Contains(filter.Search.ToLower()));

            if(filter.ProductCategoryId.HasValue)
                query = query.Where(x => x.ProductCategoryId == filter.ProductCategoryId.Value);

            if (!string.IsNullOrEmpty(filter.OrderBy)) {
                query = filter.OrderBy.ToLower() switch
                {
                    "name" or "productname" => query.OrderBy(x => x.Name),
                    "priceasc" => query.OrderBy(x => x.Price),
                    "pricedesc" => query.OrderByDescending(x => x.Price),
                    "createdatdesc" => query.OrderByDescending(x => x.CreatedAt),
                    "createdatasc" => query.OrderBy(x => x.CreatedAt),
                    _ => query.OrderByDescending(x => x.CreatedAt)
                };
                return query;
            }
            query = query.OrderByDescending(x => x.CreatedAt);
            return query;
        }

        protected override async Task BeforeDeleteAsync(Product entity)
        {
            if (!string.IsNullOrEmpty(entity.ProductImageUrl))
                await _fileStorageService.DeleteAsync(entity.ProductImageUrl);
        }

        protected override async Task AfterUpdateAsync(Product entity, UpdateProductRequest dto)
        {
            var loaded = await _repository.AsQueryable()
                .Include(x => x.ProductCategory)
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            if (loaded != null)
            {
                entity.ProductCategory = loaded.ProductCategory;
                entity.Supplier = loaded.Supplier;
            }
        }

        public async Task<ProductResponse> UploadImageAsync(int productId, FileUploadRequest fileRequest)
        {
            var product = await _repository.AsQueryable()
                .Include(x => x.Supplier)
                .Include(x => x.ProductCategory)
                .FirstOrDefaultAsync(x => x.Id == productId);

            if (product == null)
                throw new KeyNotFoundException("Proizvod nije pronađen");

            if (!string.IsNullOrEmpty(product.ProductImageUrl))
            {
                await _fileStorageService.DeleteAsync(product.ProductImageUrl);
            }

            var uploadResult = await _fileStorageService.UploadAsync(fileRequest, "products", productId.ToString());

            if (!uploadResult.Success)
                throw new InvalidOperationException(uploadResult.ErrorMessage);

            product.ProductImageUrl = uploadResult.FileUrl;
            await _repository.UpdateAsync(product);

            return _mapper.Map<ProductResponse>(product);
        }

        public async Task<bool> DeleteImageAsync(int productId)
        {
            var product = await _repository.GetByIdAsync(productId);

            if (product == null)
                throw new KeyNotFoundException("Proizvod nije pronađen");

            if (string.IsNullOrEmpty(product.ProductImageUrl))
                return false;

            var deleted = await _fileStorageService.DeleteAsync(product.ProductImageUrl);

            product.ProductImageUrl = null;
            await _repository.UpdateAsync(product);

            return deleted;
        }

        public async Task<List<ProductResponse>> GetBestSellingAsync(int count = 5)
        {
            var bestSellingProductIds = await _orderItemRepository.AsQueryable()
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(count)
                .Select(x => x.ProductId)
                .ToListAsync();

            var products = await _repository.AsQueryable()
                .Include(x => x.ProductCategory)
                .Include(x => x.Supplier)
                .Where(p => bestSellingProductIds.Contains(p.Id))
                .ToListAsync();

            // Maintain the order from best selling
            var orderedProducts = bestSellingProductIds
                .Select(id => products.FirstOrDefault(p => p.Id == id))
                .Where(p => p != null)
                .ToList();

            return _mapper.Map<List<ProductResponse>>(orderedProducts);
        }
    }
}
