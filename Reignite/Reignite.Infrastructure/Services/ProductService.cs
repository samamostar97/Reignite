using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Exceptions;
using Reignite.Application.Filters;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.Infrastructure.Services
{
    public class ProductService : BaseService<Product, ProductResponse, CreateProductRequest, UpdateProductRequest, ProductQueryFilter, int>,IProductService
    {
        private readonly IFileStorageService _fileStorageService;
        private readonly IRepository<OrderItem, int> _orderItemRepository;
        private readonly IRepository<ProductCategory, int> _categoryRepository;
        private readonly IRepository<Supplier, int> _supplierRepository;

        public ProductService(
            IRepository<Product, int> repository,
            IMapper mapper,
            IFileStorageService fileStorageService,
            IRepository<OrderItem, int> orderItemRepository,
            IRepository<ProductCategory, int> categoryRepository,
            IRepository<Supplier, int> supplierRepository) : base(repository, mapper)
        {
            _fileStorageService = fileStorageService;
            _orderItemRepository = orderItemRepository;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
        }

        public override async Task<ProductResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _repository.AsQueryable()
                .AsNoTracking()
                .Include(x => x.ProductCategory)
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (product == null)
                throw new KeyNotFoundException("Proizvod nije pronađen.");

            return _mapper.Map<ProductResponse>(product);
        }

        public async Task<ProductResponse> CreateWithImageAsync(CreateProductRequest dto, FileUploadRequest? imageRequest, CancellationToken cancellationToken = default)
        {
            var product = _mapper.Map<Product>(dto);
            await BeforeCreateAsync(product, dto, cancellationToken);
            await _repository.AddAsync(product, cancellationToken);

            if (imageRequest != null && imageRequest.FileStream != null && imageRequest.FileSize > 0)
            {
                var uploadResult = await _fileStorageService.UploadAsync(imageRequest, "products", product.Id.ToString(), cancellationToken);

                if (uploadResult.Success)
                {
                    product.ProductImageUrl = uploadResult.FileUrl;
                    await _repository.UpdateAsync(product, cancellationToken);
                }
            }

            var result = await _repository.AsQueryable()
                .Include(x => x.ProductCategory)
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.Id == product.Id, cancellationToken);

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

        protected override async Task BeforeCreateAsync(Product entity, CreateProductRequest dto, CancellationToken cancellationToken = default)
        {
            var categoryExists = await _categoryRepository.AsQueryable()
                .AnyAsync(c => c.Id == dto.ProductCategoryId, cancellationToken);
            if (!categoryExists)
                throw new KeyNotFoundException($"Kategorija sa ID {dto.ProductCategoryId} ne postoji.");

            var supplierExists = await _supplierRepository.AsQueryable()
                .AnyAsync(s => s.Id == dto.SupplierId, cancellationToken);
            if (!supplierExists)
                throw new KeyNotFoundException($"Dobavljač sa ID {dto.SupplierId} ne postoji.");

            var nameExists = await _repository.AsQueryable()
                .AnyAsync(p => p.Name.ToLower() == dto.Name.ToLower(), cancellationToken);
            if (nameExists)
                throw new ConflictException($"Proizvod sa nazivom '{dto.Name}' već postoji.");
        }

        protected override async Task BeforeUpdateAsync(Product entity, UpdateProductRequest dto, CancellationToken cancellationToken = default)
        {
            if (dto.ProductCategoryId.HasValue)
            {
                var categoryExists = await _categoryRepository.AsQueryable()
                    .AnyAsync(c => c.Id == dto.ProductCategoryId.Value, cancellationToken);
                if (!categoryExists)
                    throw new KeyNotFoundException($"Kategorija sa ID {dto.ProductCategoryId} ne postoji.");
            }

            if (dto.SupplierId.HasValue)
            {
                var supplierExists = await _supplierRepository.AsQueryable()
                    .AnyAsync(s => s.Id == dto.SupplierId.Value, cancellationToken);
                if (!supplierExists)
                    throw new KeyNotFoundException($"Dobavljač sa ID {dto.SupplierId} ne postoji.");
            }

            if (!string.IsNullOrEmpty(dto.Name))
            {
                var nameExists = await _repository.AsQueryable()
                    .AnyAsync(p => p.Id != entity.Id && p.Name.ToLower() == dto.Name.ToLower(), cancellationToken);
                if (nameExists)
                    throw new ConflictException($"Proizvod sa nazivom '{dto.Name}' već postoji.");
            }
        }

        protected override async Task BeforeDeleteAsync(Product entity, CancellationToken cancellationToken = default)
        {
            var orderItemCount = await _orderItemRepository.AsQueryable()
                .CountAsync(oi => oi.ProductId == entity.Id, cancellationToken);

            if (orderItemCount > 0)
                throw new EntityHasDependentsException("proizvod", "stavki narudžbi", orderItemCount);

            if (!string.IsNullOrEmpty(entity.ProductImageUrl))
                await _fileStorageService.DeleteAsync(entity.ProductImageUrl, cancellationToken);
        }

        protected override async Task AfterUpdateAsync(Product entity, UpdateProductRequest dto, CancellationToken cancellationToken = default)
        {
            var loaded = await _repository.AsQueryable()
                .Include(x => x.ProductCategory)
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);

            if (loaded != null)
            {
                entity.ProductCategory = loaded.ProductCategory;
                entity.Supplier = loaded.Supplier;
            }
        }

        public async Task<ProductResponse> UploadImageAsync(int productId, FileUploadRequest fileRequest, CancellationToken cancellationToken = default)
        {
            var product = await _repository.AsQueryable()
                .Include(x => x.Supplier)
                .Include(x => x.ProductCategory)
                .FirstOrDefaultAsync(x => x.Id == productId, cancellationToken);

            if (product == null)
                throw new KeyNotFoundException("Proizvod nije pronađen");

            if (!string.IsNullOrEmpty(product.ProductImageUrl))
            {
                await _fileStorageService.DeleteAsync(product.ProductImageUrl, cancellationToken);
            }

            var uploadResult = await _fileStorageService.UploadAsync(fileRequest, "products", productId.ToString(), cancellationToken);

            if (!uploadResult.Success)
                throw new InvalidOperationException(uploadResult.ErrorMessage);

            product.ProductImageUrl = uploadResult.FileUrl;
            await _repository.UpdateAsync(product, cancellationToken);

            return _mapper.Map<ProductResponse>(product);
        }

        public async Task<bool> DeleteImageAsync(int productId, CancellationToken cancellationToken = default)
        {
            var product = await _repository.GetByIdAsync(productId, cancellationToken);

            if (product == null)
                throw new KeyNotFoundException("Proizvod nije pronađen");

            if (string.IsNullOrEmpty(product.ProductImageUrl))
                return false;

            var deleted = await _fileStorageService.DeleteAsync(product.ProductImageUrl, cancellationToken);

            product.ProductImageUrl = null;
            await _repository.UpdateAsync(product, cancellationToken);

            return deleted;
        }

        public async Task<List<ProductResponse>> GetBestSellingAsync(int count = 5, CancellationToken cancellationToken = default)
        {
            var bestSellingProductIds = await _orderItemRepository.AsQueryable()
                .AsNoTracking()
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(count)
                .Select(x => x.ProductId)
                .ToListAsync(cancellationToken);

            var products = await _repository.AsQueryable()
                .AsNoTracking()
                .Include(x => x.ProductCategory)
                .Include(x => x.Supplier)
                .Where(p => bestSellingProductIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            // Maintain the order from best selling
            var orderedProducts = bestSellingProductIds
                .Select(id => products.FirstOrDefault(p => p.Id == id))
                .Where(p => p != null)
                .ToList();

            return _mapper.Map<List<ProductResponse>>(orderedProducts);
        }
    }
}
