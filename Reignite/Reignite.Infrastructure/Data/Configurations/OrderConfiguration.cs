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

            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Order { Id = 1, UserId = 2, TotalAmount = 123.00m, PurchaseDate = seedDate, Status = Reignite.Core.Enums.OrderStatus.Processing, CreatedAt = seedDate },
                new Order { Id = 2, UserId = 3, TotalAmount = 188.49m, PurchaseDate = seedDate, Status = Reignite.Core.Enums.OrderStatus.OnDelivery, CreatedAt = seedDate },
                new Order { Id = 3, UserId = 4, TotalAmount = 84.80m, PurchaseDate = seedDate, Status = Reignite.Core.Enums.OrderStatus.Processing, CreatedAt = seedDate }
            );
        }
    }

}

