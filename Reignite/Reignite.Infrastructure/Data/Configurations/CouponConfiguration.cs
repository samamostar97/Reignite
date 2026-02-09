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
        }
    }
}
