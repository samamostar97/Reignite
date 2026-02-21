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
    [Route("api/coupons")]
    [Authorize(Roles = "Admin")]
    public class CouponController : BaseController<Coupon, CouponResponse, CreateCouponRequest, UpdateCouponRequest, CouponQueryFilter, int>
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService service) : base(service)
        {
            _couponService = service;
        }

        // GET api/coupons/featured - public endpoint for landing page promo banner
        [HttpGet("featured")]
        [AllowAnonymous]
        public async Task<ActionResult<List<CouponResponse>>> GetFeaturedCoupons(CancellationToken cancellationToken = default)
        {
            var coupons = await _couponService.GetFeaturedCouponsAsync(cancellationToken);
            return Ok(coupons);
        }

        // POST api/coupons/validate - accessible to any authenticated user
        [HttpPost("validate")]
        [AllowAnonymous]
        public async Task<ActionResult<CouponResponse>> ValidateCoupon([FromBody] ValidateCouponRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var coupon = await _couponService.ValidateCouponAsync(request.Code, request.OrderTotal, cancellationToken);
                return Ok(coupon);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
