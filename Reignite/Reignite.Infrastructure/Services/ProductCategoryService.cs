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
    public class ProductCategoryService : BaseService<ProductCategory, ProductCategoryResponse, CreateProductCategoryRequest, UpdateProductCategoryRequest, ProductCategoryQueryFilter, int>, IProductCategoryService
    {
        public ProductCategoryService(IRepository<ProductCategory, int> repository, IMapper mapper) : base(repository, mapper)
        {
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
