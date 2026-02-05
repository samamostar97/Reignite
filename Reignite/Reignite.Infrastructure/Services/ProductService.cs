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

        public ProductService(IRepository<Product, int> repository, IMapper mapper, IFileStorageService fileStorageService) : base(repository, mapper)
        {
            _fileStorageService = fileStorageService;
        }
        protected override IQueryable<Product> ApplyFilter(IQueryable<Product> query, ProductQueryFilter filter)
        {
            query = query.Include(x => x.ProductCategory).Include(x=>x.Supplier);
            if(!string.IsNullOrEmpty(filter.Search))
                query=query.Where(x=>x.Name.ToLower().Contains(filter.Search.ToLower()));
            if (!string.IsNullOrEmpty(filter.OrderBy)) { 
                query = filter.OrderBy.ToLower() switch
                {
                    "productname" => query.OrderBy(x => x.Name),
                    "createdatdesc" => query.OrderByDescending(x => x.CreatedAt),
                    _ => query.OrderBy(x => x.CreatedAt)
                };
                return query;
            }
            query = query.OrderBy(x => x.CreatedAt);
            return query;
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
    }
}
