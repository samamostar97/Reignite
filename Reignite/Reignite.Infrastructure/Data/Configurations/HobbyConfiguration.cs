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
                new Hobby { Id = 1, Name = "Stolarija", Description = "Umjetnost izrade predmeta od drveta, od namještaja do dekorativnih komada.", IconUrl = null, CreatedAt = seedDate },
                new Hobby { Id = 2, Name = "Obrada kože", Description = "Izrada novčanika, remena, torbi i drugih predmeta od kože.", IconUrl = null, CreatedAt = seedDate },
                new Hobby { Id = 3, Name = "Obrada metala", Description = "Oblikovanje i kovanje metala u alate, umjetničke i funkcionalne predmete.", IconUrl = null, CreatedAt = seedDate },
                new Hobby { Id = 4, Name = "Keramika", Description = "Oblikovanje gline u keramičke predmete, od posuda do skulptura.", IconUrl = null, CreatedAt = seedDate },
                new Hobby { Id = 5, Name = "Izrada svijeća", Description = "Izrada jedinstvenih svijeća sa posebnim mirisima i dizajnom.", IconUrl = null, CreatedAt = seedDate }
            );
        }
    }
}
