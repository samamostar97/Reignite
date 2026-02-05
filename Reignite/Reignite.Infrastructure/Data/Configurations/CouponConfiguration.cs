using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class CouponConfiguration : BaseEntityConfiguration<Coupon>
    {
        public override void Configure(EntityTypeBuilder<Coupon> builder)
        {
            base.Configure(builder);
            builder.HasIndex(c => c.Code).IsUnique();
            builder.Property(c => c.DiscountValue).HasPrecision(18, 2);
            builder.Property(c => c.MinimumOrderAmount).HasPrecision(18, 2);

            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Coupon
                {
                    Id = 1,
                    Code = "DOBRODOSLI10",
                    DiscountType = "Percentage",
                    DiscountValue = 10.00m,
                    MinimumOrderAmount = 50.00m,
                    ExpiryDate = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    MaxUses = 500,
                    TimesUsed = 47,
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new Coupon
                {
                    Id = 2,
                    Code = "LJETO2026",
                    DiscountType = "Percentage",
                    DiscountValue = 15.00m,
                    MinimumOrderAmount = 100.00m,
                    ExpiryDate = new DateTime(2026, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                    MaxUses = 200,
                    TimesUsed = 23,
                    IsActive = true,
                    CreatedAt = seedDate
                },
                new Coupon
                {
                    Id = 3,
                    Code = "BESPLATNADOSTAVA",
                    DiscountType = "Fixed",
                    DiscountValue = 7.00m,
                    MinimumOrderAmount = 50.00m,
                    ExpiryDate = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    MaxUses = 1000,
                    TimesUsed = 156,
                    IsActive = true,
                    CreatedAt = seedDate
                }
            );
        }
    }
}
