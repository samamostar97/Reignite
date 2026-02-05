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
    [Route("api/products")]
    [Authorize(Roles ="Admin")]
    public class ProductController : BaseController<Product, ProductResponse, CreateProductRequest, UpdateProductRequest, ProductQueryFilter, int>
    {
        private readonly IProductService _productService;
        public ProductController(IProductService service) : base(service)
        {
            _productService = service;
        }
        [AllowAnonymous]
        [HttpGet]
        public override Task<ActionResult<PagedResult<ProductResponse>>> GetAllPagedAsync([FromQuery] ProductQueryFilter filter) => base.GetAllPagedAsync(filter);
        [AllowAnonymous]
        [HttpGet("{id}")]
        public override Task<ActionResult<ProductResponse>> GetById(int id) => base.GetById(id);
    }
}
