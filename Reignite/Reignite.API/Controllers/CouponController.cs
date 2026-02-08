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
        public CouponController(ICouponService service) : base(service)
        {
        }
    }
}
