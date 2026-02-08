using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.Infrastructure.Services
{
    public class CouponService : BaseService<Coupon, CouponResponse, CreateCouponRequest, UpdateCouponRequest, CouponQueryFilter, int>, ICouponService
    {
        public CouponService(IRepository<Coupon, int> repository, IMapper mapper) : base(repository, mapper)
        {
        }

        protected override async Task BeforeCreateAsync(Coupon entity, CreateCouponRequest dto)
        {
            // Validate unique code
            var existingCoupon = await _repository.AsQueryable()
                .FirstOrDefaultAsync(c => c.Code.ToLower() == entity.Code.ToLower());

            if (existingCoupon != null)
                throw new InvalidOperationException($"Kupon sa kodom '{entity.Code}' već postoji.");

            // Validate discount type
            if (entity.DiscountType != "Percentage" && entity.DiscountType != "Fixed")
                throw new InvalidOperationException("Tip popusta mora biti 'Percentage' ili 'Fixed'.");

            // Validate percentage discount value
            if (entity.DiscountType == "Percentage" && entity.DiscountValue > 100)
                throw new InvalidOperationException("Procenat popusta ne može biti veći od 100%.");

            // Set initial TimesUsed to 0
            entity.TimesUsed = 0;
        }

        protected override async Task BeforeUpdateAsync(Coupon entity, UpdateCouponRequest dto)
        {
            // Validate unique code (excluding current entity)
            var existingCoupon = await _repository.AsQueryable()
                .FirstOrDefaultAsync(c => c.Code.ToLower() == dto.Code.ToLower() && c.Id != entity.Id);

            if (existingCoupon != null)
                throw new InvalidOperationException($"Kupon sa kodom '{dto.Code}' već postoji.");

            // Validate discount type
            if (dto.DiscountType != "Percentage" && dto.DiscountType != "Fixed")
                throw new InvalidOperationException("Tip popusta mora biti 'Percentage' ili 'Fixed'.");

            // Validate percentage discount value
            if (dto.DiscountType == "Percentage" && dto.DiscountValue > 100)
                throw new InvalidOperationException("Procenat popusta ne može biti veći od 100%.");
        }

        protected override async Task BeforeDeleteAsync(Coupon entity)
        {
            // Can safely delete coupons (no foreign key dependencies)
            await Task.CompletedTask;
        }

        protected override IQueryable<Coupon> ApplyFilter(IQueryable<Coupon> query, CouponQueryFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Search))
            {
                var searchLower = filter.Search.ToLower();
                query = query.Where(c => c.Code.ToLower().Contains(searchLower));
            }

            if (filter.IsActive.HasValue)
                query = query.Where(c => c.IsActive == filter.IsActive.Value);

            if (filter.IsExpired.HasValue)
            {
                var now = DateTime.UtcNow;
                if (filter.IsExpired.Value)
                    query = query.Where(c => c.ExpiryDate.HasValue && c.ExpiryDate.Value < now);
                else
                    query = query.Where(c => !c.ExpiryDate.HasValue || c.ExpiryDate.Value >= now);
            }

            if (!string.IsNullOrEmpty(filter.OrderBy))
            {
                query = filter.OrderBy.ToLower() switch
                {
                    "code" => query.OrderBy(c => c.Code),
                    "codedesc" => query.OrderByDescending(c => c.Code),
                    "expiry" => query.OrderBy(c => c.ExpiryDate),
                    "expirydesc" => query.OrderByDescending(c => c.ExpiryDate),
                    "uses" => query.OrderBy(c => c.TimesUsed),
                    "usesdesc" => query.OrderByDescending(c => c.TimesUsed),
                    _ => query.OrderByDescending(c => c.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(c => c.CreatedAt);
            }

            return query;
        }
    }
}
