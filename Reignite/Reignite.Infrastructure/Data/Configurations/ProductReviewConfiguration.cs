using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class ProductReviewConfiguration : BaseEntityConfiguration<ProductReview>
    {
        public override void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            base.Configure(builder);
            builder.Property(r => r.Rating).IsRequired();
            builder.Property(r => r.Comment).HasMaxLength(1000);

            builder.HasOne(r => r.User)
                .WithMany(u => u.ProductReviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Product)
                .WithMany(s => s.ProductReviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => new { r.UserId, r.ProductId }).IsUnique().HasFilter("[IsDeleted] = 0");
        }
    }
}
