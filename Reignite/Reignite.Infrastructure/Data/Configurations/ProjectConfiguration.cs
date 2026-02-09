using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class ProjectConfiguration : BaseEntityConfiguration<Project>
    {
        public override void Configure(EntityTypeBuilder<Project> builder)
        {
            base.Configure(builder);

            builder.Property(p => p.Title).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(2000);
            builder.Property(p => p.ImageUrl).HasMaxLength(500);

            builder.HasOne(p => p.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Hobby)
                .WithMany(h => h.Projects)
                .HasForeignKey(p => p.HobbyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Product)
                .WithMany(pr => pr.Projects)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
