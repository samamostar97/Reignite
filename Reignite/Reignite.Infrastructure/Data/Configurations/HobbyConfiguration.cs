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
                new Hobby { Id = 1, Name = "Stolarija", Description = "Umjetnost izrade predmeta od drveta, od namještaja do dekorativnih komada.", IconUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=100", CreatedAt = seedDate },
                new Hobby { Id = 2, Name = "Obrada kože", Description = "Izrada novčanika, remena, torbi i drugih predmeta od kože.", IconUrl = "https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=100", CreatedAt = seedDate },
                new Hobby { Id = 3, Name = "Obrada metala", Description = "Oblikovanje i kovanje metala u alate i funkcionalne predmete.", IconUrl = "https://images.unsplash.com/photo-1504328345606-18bbc8c9d7d1?w=100", CreatedAt = seedDate },
                new Hobby { Id = 4, Name = "Keramika", Description = "Oblikovanje gline u keramičke predmete, od posuda do skulptura.", IconUrl = "https://images.unsplash.com/photo-1493106641515-6b5631de4bb9?w=100", CreatedAt = seedDate },
                new Hobby { Id = 5, Name = "Izrada svijeća", Description = "Izrada jedinstvenih svijeća sa posebnim mirisima i dizajnom.", IconUrl = "https://images.unsplash.com/photo-1602523961358-f9f03dd557db?w=100", CreatedAt = seedDate },
                new Hobby { Id = 6, Name = "Rezbarenje", Description = "Umjetnost oblikovanja drveta ručnim alatima.", IconUrl = "https://images.unsplash.com/photo-1452860606245-08befc0ff44b?w=100", CreatedAt = seedDate }
            );
        }
    }
}
