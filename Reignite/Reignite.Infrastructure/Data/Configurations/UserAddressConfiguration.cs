using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class UserAddressConfiguration : BaseEntityConfiguration<UserAddress>
    {
        public override void Configure(EntityTypeBuilder<UserAddress> builder)
        {
            base.Configure(builder);
            // Unique constraint - one address per user
            builder.HasIndex(a => a.UserId)
                .IsUnique();

            builder.HasData(
                new UserAddress
                {
                    Id = 1,
                    UserId = 2,
                    Street = "Ferhadija 15",
                    City = "Sarajevo",
                    PostalCode = "71000",
                    Country = "Bosna i Hercegovina",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new UserAddress
                {
                    Id = 2,
                    UserId = 3,
                    Street = "Bulevar narodne revolucije 8",
                    City = "Mostar",
                    PostalCode = "88000",
                    Country = "Bosna i Hercegovina",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new UserAddress
                {
                    Id = 3,
                    UserId = 4,
                    Street = "Turalibegova 22",
                    City = "Tuzla",
                    PostalCode = "75000",
                    Country = "Bosna i Hercegovina",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}

