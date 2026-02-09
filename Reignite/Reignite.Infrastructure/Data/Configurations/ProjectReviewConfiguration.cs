using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class ProjectReviewConfiguration : BaseEntityConfiguration<ProjectReview>
    {
        public override void Configure(EntityTypeBuilder<ProjectReview> builder)
        {
            base.Configure(builder);
            builder.Property(r => r.Rating).IsRequired();
            builder.Property(r => r.Comment).HasMaxLength(1000);

            builder.HasOne(r => r.User)
                .WithMany(u => u.ProjectReviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Project)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => new { r.UserId, r.ProjectId }).IsUnique().HasFilter("[IsDeleted] = 0");
        }
    }
}
