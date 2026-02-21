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

        protected override async Task BeforeCreateAsync(Coupon entity, CreateCouponRequest dto, CancellationToken cancellationToken = default)
        {
            // Validate unique code
            var existingCoupon = await _repository.AsQueryable()
                .FirstOrDefaultAsync(c => c.Code.ToLower() == entity.Code.ToLower(), cancellationToken);

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

        protected override async Task BeforeUpdateAsync(Coupon entity, UpdateCouponRequest dto, CancellationToken cancellationToken = default)
        {
            // Validate unique code (excluding current entity)
            var existingCoupon = await _repository.AsQueryable()
                .FirstOrDefaultAsync(c => c.Code.ToLower() == dto.Code.ToLower() && c.Id != entity.Id, cancellationToken);

            if (existingCoupon != null)
                throw new InvalidOperationException($"Kupon sa kodom '{dto.Code}' već postoji.");

            // Validate discount type
            if (dto.DiscountType != "Percentage" && dto.DiscountType != "Fixed")
                throw new InvalidOperationException("Tip popusta mora biti 'Percentage' ili 'Fixed'.");

            // Validate percentage discount value
            if (dto.DiscountType == "Percentage" && dto.DiscountValue > 100)
                throw new InvalidOperationException("Procenat popusta ne može biti veći od 100%.");
        }

        protected override async Task BeforeDeleteAsync(Coupon entity, CancellationToken cancellationToken = default)
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

        public async Task<CouponResponse> ValidateCouponAsync(string code, decimal orderTotal, CancellationToken cancellationToken = default)
        {
            var coupon = await _repository.AsQueryable()
                .FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower(), cancellationToken);

            if (coupon == null)
                throw new KeyNotFoundException("Kupon nije pronađen.");

            if (!coupon.IsActive)
                throw new InvalidOperationException("Ovaj kupon nije aktivan.");

            if (coupon.ExpiryDate.HasValue && coupon.ExpiryDate.Value < DateTime.UtcNow)
                throw new InvalidOperationException("Ovaj kupon je istekao.");

            if (coupon.MaxUses.HasValue && coupon.TimesUsed >= coupon.MaxUses.Value)
                throw new InvalidOperationException("Ovaj kupon je iskorišten maksimalan broj puta.");

            if (coupon.MinimumOrderAmount.HasValue && orderTotal < coupon.MinimumOrderAmount.Value)
                throw new InvalidOperationException($"Minimalan iznos narudžbe za ovaj kupon je {coupon.MinimumOrderAmount.Value:F2} KM.");

            return _mapper.Map<CouponResponse>(coupon);
        }

        public async Task IncrementUsageAsync(string code, CancellationToken cancellationToken = default)
        {
            var coupon = await _repository.AsQueryable()
                .FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower(), cancellationToken);

            if (coupon != null)
            {
                coupon.TimesUsed++;
                await _repository.UpdateAsync(coupon, cancellationToken);
            }
        }

        public async Task<List<CouponResponse>> GetFeaturedCouponsAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var coupons = await _repository.AsQueryable()
                .Where(c => c.IsFeatured && c.IsActive
                    && (!c.ExpiryDate.HasValue || c.ExpiryDate.Value >= now)
                    && (!c.MaxUses.HasValue || c.TimesUsed < c.MaxUses.Value))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<CouponResponse>>(coupons);
        }
    }
}
