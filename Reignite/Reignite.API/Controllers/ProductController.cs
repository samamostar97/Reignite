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
        
        [HttpPost("{id}/image")]
        public async Task<ActionResult<ProductResponse>> UploadImage(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Nije odabrana slika");

            var fileRequest = new FileUploadRequest
            {
                FileStream = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileSize = file.Length
            };

            var result = await _productService.UploadImageAsync(id, fileRequest);
            return Ok(result);
        }
        [HttpDelete("{id}/image")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            await _productService.DeleteImageAsync(id);
            return NoContent();
        }
    }
}
