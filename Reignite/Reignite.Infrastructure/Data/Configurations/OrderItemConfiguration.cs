using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class OrderItemConfiguration : BaseEntityConfiguration<OrderItem>
    {
        public override void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            base.Configure(builder);
            builder.Property(oi => oi.Quantity).IsRequired();
            builder.Property(oi => oi.UnitPrice).HasPrecision(18, 2).IsRequired();

            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                // Order 1 (User 2): Čekić 45.00 + Hrastova daska 78.00 = 123.00
                new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 1, UnitPrice = 45.00m, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OrderItem { Id = 2, OrderId = 1, ProductId = 3, Quantity = 1, UnitPrice = 78.00m, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                // Order 2 (User 3): Dlijeto 38.50 + Stolarski komplet 149.99 = 188.49
                new OrderItem { Id = 3, OrderId = 2, ProductId = 10, Quantity = 1, UnitPrice = 38.50m, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OrderItem { Id = 4, OrderId = 2, ProductId = 5, Quantity = 1, UnitPrice = 149.99m, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                // Order 3 (User 4): Set šrafcigera 32.50 + Orahov furnir 55.90 = 88.40 — adjusted total: let's use 2x brusni papir
                new OrderItem { Id = 5, OrderId = 3, ProductId = 9, Quantity = 2, UnitPrice = 19.90m, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new OrderItem { Id = 6, OrderId = 3, ProductId = 1, Quantity = 1, UnitPrice = 45.00m, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

