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
            builder.Property(h => h.IconUrl).HasMaxLength(500);

            builder.HasIndex(h => h.Name).IsUnique().HasFilter("[IsDeleted] = 0");

            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Hobby { Id = 1, Name = "Woodworking", Description = "The art of crafting objects from wood, from furniture to decorative pieces.", IconUrl = null, CreatedAt = seedDate },
                new Hobby { Id = 2, Name = "Leathercraft", Description = "Creating wallets, belts, bags and other items from leather.", IconUrl = null, CreatedAt = seedDate },
                new Hobby { Id = 3, Name = "Metalworking", Description = "Shaping and forging metal into tools, art, and functional objects.", IconUrl = null, CreatedAt = seedDate },
                new Hobby { Id = 4, Name = "Pottery", Description = "Molding clay into ceramics, from bowls to sculptures.", IconUrl = null, CreatedAt = seedDate },
                new Hobby { Id = 5, Name = "Candle Making", Description = "Crafting custom candles with unique scents and designs.", IconUrl = null, CreatedAt = seedDate }
            );
        }
    }
}
