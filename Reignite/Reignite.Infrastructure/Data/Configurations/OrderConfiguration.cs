using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : BaseEntityConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);
            builder.Property(o => o.TotalAmount).HasPrecision(18, 2).IsRequired();
            builder.Property(o => o.PurchaseDate).IsRequired();
            builder.Property(o => o.StripePaymentId).HasMaxLength(255);
            builder.Property(o => o.CouponCode).HasMaxLength(50);
            builder.Property(o => o.DiscountAmount).HasPrecision(18, 2);

            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}

