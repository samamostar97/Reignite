using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class WishlistItemConfiguration: BaseEntityConfiguration<WishlistItem>
    {
        public override void Configure(EntityTypeBuilder<WishlistItem> builder)
        {
            base.Configure(builder);

            builder.HasOne(wi => wi.Wishlist)
                   .WithMany(w => w.WishlistItems)
                   .HasForeignKey(wi => wi.WishlistId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(wi => wi.Product)
                   .WithMany() 
                   .HasForeignKey(wi => wi.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(wi => wi.UnitPrice)
                   .HasColumnType("decimal(18,2)");

            builder.HasIndex(wi => new { wi.WishlistId, wi.ProductId })
                   .IsUnique();

            builder.HasData(
                new WishlistItem { Id = 1, WishlistId = 1, ProductId = 3, Quantity = 1, UnitPrice = 78.00m, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new WishlistItem { Id = 2, WishlistId = 1, ProductId = 7, Quantity = 1, UnitPrice = 65.00m, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new WishlistItem { Id = 3, WishlistId = 2, ProductId = 5, Quantity = 1, UnitPrice = 149.99m, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new WishlistItem { Id = 4, WishlistId = 2, ProductId = 10, Quantity = 2, UnitPrice = 38.50m, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new WishlistItem { Id = 5, WishlistId = 3, ProductId = 1, Quantity = 1, UnitPrice = 45.00m, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new WishlistItem { Id = 6, WishlistId = 3, ProductId = 6, Quantity = 1, UnitPrice = 189.00m, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

