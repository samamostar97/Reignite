using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Core.Entities;

namespace Reignite.Application.IServices
{
    public interface ICouponService : IService<Coupon, CouponResponse, CreateCouponRequest, UpdateCouponRequest, CouponQueryFilter, int>
    {
        Task<CouponResponse> ValidateCouponAsync(string code, decimal orderTotal, CancellationToken cancellationToken = default);
        Task IncrementUsageAsync(string code, CancellationToken cancellationToken = default);
        Task<List<CouponResponse>> GetFeaturedCouponsAsync(CancellationToken cancellationToken = default);
    }
}
