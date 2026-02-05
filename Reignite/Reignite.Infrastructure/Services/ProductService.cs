using MapsterMapper;
using Microsoft.EntityFrameworkCore;
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
        public ProductService(IRepository<Product, int> repository, IMapper mapper) : base(repository, mapper)
        {
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
    }
}
