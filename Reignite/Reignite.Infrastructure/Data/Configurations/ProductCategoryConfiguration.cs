using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class ProductCategoryConfiguration : BaseEntityConfiguration<ProductCategory>
    {
        public override void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            base.Configure(builder);
            builder.Property(c => c.Name).HasMaxLength(100).IsRequired();

            builder.HasIndex(c => c.Name)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");


            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new ProductCategory { Id = 1, Name = "Ručni Alati", CreatedAt = seedDate },
                new ProductCategory { Id = 2, Name = "Premium Materijali",  CreatedAt = seedDate },
                new ProductCategory { Id = 3, Name = "Komplete za Početnike",  CreatedAt = seedDate },
                new ProductCategory { Id = 4, Name = "Pribor za Radionicu",  CreatedAt = seedDate }
            );
        }
    }
}

