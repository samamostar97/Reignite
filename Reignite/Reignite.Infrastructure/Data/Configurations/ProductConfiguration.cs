using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : BaseEntityConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);
            builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
            builder.Property(s => s.Price).HasPrecision(18, 2).IsRequired();
            builder.Property(s => s.Description).HasMaxLength(1000);
            builder.Property(u => u.ProductImageUrl).HasMaxLength(500);


            builder.HasOne(s => s.ProductCategory)
                .WithMany(c => c.Products)
                .HasForeignKey(s => s.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Supplier)
                .WithMany(sup => sup.Products)
                .HasForeignKey(s => s.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
