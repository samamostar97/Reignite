using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.API.Controllers
{
    [ApiController]
    [Route("api/product-categories")]
    [Authorize(Roles ="Admin")]
    public class ProductCategoryController : BaseController<ProductCategory, ProductCategoryResponse, CreateProductCategoryRequest, UpdateProductCategoryRequest, ProductCategoryQueryFilter, int>
    {
        private readonly IProductCategoryService _productCategoryService;
        public ProductCategoryController(IProductCategoryService service) : base(service)
        {
            _productCategoryService = service;
        }
        [AllowAnonymous]
        [HttpGet]
        public override Task<ActionResult<PagedResult<ProductCategoryResponse>>> GetAllPagedAsync([FromQuery] ProductCategoryQueryFilter filter, CancellationToken cancellationToken = default) => base.GetAllPagedAsync(filter, cancellationToken);
        [AllowAnonymous]
        [HttpGet("{id}")]
        public override Task<ActionResult<ProductCategoryResponse>> GetById(int id, CancellationToken cancellationToken = default) => base.GetById(id, cancellationToken);

    }
}
