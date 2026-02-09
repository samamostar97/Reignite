using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class HobbyConfiguration : BaseEntityConfiguration<Hobby>
    {
        public override void Configure(EntityTypeBuilder<Hobby> builder)
        {
            base.Configure(builder);

            builder.Property(h => h.Name).HasMaxLength(100).IsRequired();
            builder.Property(h => h.Description).HasMaxLength(500);

            builder.HasIndex(h => h.Name).IsUnique().HasFilter("[IsDeleted] = 0");
        }
    }
}
