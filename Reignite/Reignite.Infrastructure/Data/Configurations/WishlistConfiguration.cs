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
    public class WishlistConfiguration : BaseEntityConfiguration<Wishlist>
    {
        public override void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            base.Configure(builder);

            builder.HasOne(w => w.User)
                   .WithOne(u => u.Wishlist)
                   .HasForeignKey<Wishlist>(w => w.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(w => w.UserId).IsUnique(); // enforce 1 wishlist per user

            builder.HasData(
                new Wishlist { Id = 1, UserId = 2, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Wishlist { Id = 2, UserId = 3, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Wishlist { Id = 3, UserId = 4, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

