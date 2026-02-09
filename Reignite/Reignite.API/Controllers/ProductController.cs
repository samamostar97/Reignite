using System.IO;
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
        public override Task<ActionResult<PagedResult<ProductResponse>>> GetAllPagedAsync([FromQuery] ProductQueryFilter filter, CancellationToken cancellationToken = default) => base.GetAllPagedAsync(filter, cancellationToken);
        [AllowAnonymous]
        [HttpGet("{id}")]
        public override Task<ActionResult<ProductResponse>> GetById(int id, CancellationToken cancellationToken = default) => base.GetById(id, cancellationToken);

        [HttpPost]
        public override async Task<ActionResult<ProductResponse>> Create([FromForm] CreateProductRequest dto, CancellationToken cancellationToken = default)
        {
            var image = Request.Form.Files.Count > 0 ? Request.Form.Files[0] : null;
            FileUploadRequest? fileRequest = null;
            Stream? imageStream = null;

            try
            {
                if (image != null && image.Length > 0)
                {
                    imageStream = image.OpenReadStream();
                    fileRequest = new FileUploadRequest
                    {
                        FileStream = imageStream,
                        FileName = image.FileName,
                        ContentType = image.ContentType,
                        FileSize = image.Length
                    };
                }

                var result = await _productService.CreateWithImageAsync(dto, fileRequest, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            finally
            {
                imageStream?.Dispose();
            }
        }

        [AllowAnonymous]
        [HttpGet("best-selling")]
        public async Task<ActionResult<List<ProductResponse>>> GetBestSelling([FromQuery] int count = 5, CancellationToken cancellationToken = default)
        {
            var products = await _productService.GetBestSellingAsync(count, cancellationToken);
            return Ok(products);
        }
    }
}
