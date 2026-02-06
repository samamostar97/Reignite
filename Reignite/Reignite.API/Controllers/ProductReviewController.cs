using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IServices;
using Reignite.Core.Entities;
using System.Security.Claims;

namespace Reignite.API.Controllers
{
    [ApiController]
    [Route("api/product-reviews")]
    public class ProductReviewController : BaseController<ProductReview, ProductReviewResponse, CreateProductReviewRequest, UpdateProductReviewRequest, ProductReviewQueryFilter, int>
    {
        private readonly IProductReviewService _productReviewService;

        public ProductReviewController(IProductReviewService service) : base(service)
        {
            _productReviewService = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public override Task<ActionResult<PagedResult<ProductReviewResponse>>> GetAllPagedAsync([FromQuery] ProductReviewQueryFilter filter)
            => base.GetAllPagedAsync(filter);

        [AllowAnonymous]
        [HttpGet("{id}")]
        public override Task<ActionResult<ProductReviewResponse>> GetById(int id)
            => base.GetById(id);

        [AllowAnonymous]
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<PagedResult<ProductReviewResponse>>> GetByProductId(int productId, [FromQuery] PaginationRequest pagination)
        {
            var result = await _productReviewService.GetByProductIdAsync(productId, pagination);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("product/{productId}/average-rating")]
        public async Task<ActionResult<double>> GetAverageRating(int productId)
        {
            var rating = await _productReviewService.GetAverageRatingAsync(productId);
            return Ok(rating);
        }

        [Authorize]
        [HttpGet("product/{productId}/has-reviewed")]
        public async Task<ActionResult<bool>> HasUserReviewed(int productId)
        {
            var userId = GetCurrentUserId();
            var hasReviewed = await _productReviewService.HasUserReviewedAsync(userId, productId);
            return Ok(hasReviewed);
        }

        [Authorize]
        [HttpPost]
        public override async Task<ActionResult<ProductReviewResponse>> Create([FromBody] CreateProductReviewRequest dto)
        {
            var userId = GetCurrentUserId();
            var result = await _productReviewService.CreateForUserAsync(userId, dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public override async Task<ActionResult<ProductReviewResponse>> Update(int id, [FromBody] UpdateProductReviewRequest dto)
        {
            var userId = GetCurrentUserId();
            var existing = await _productReviewService.GetByIdAsync(id);

            if (existing.UserId != userId)
                return Forbid();

            return await base.Update(id, dto);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public override async Task<ActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var existing = await _productReviewService.GetByIdAsync(id);

            var isAdmin = User.IsInRole("Admin");
            if (existing.UserId != userId && !isAdmin)
                return Forbid();

            return await base.Delete(id);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Korisnik nije prijavljen.");

            return userId;
        }
    }
}
