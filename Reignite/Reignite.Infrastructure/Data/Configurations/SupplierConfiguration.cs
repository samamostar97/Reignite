using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class SupplierConfiguration : BaseEntityConfiguration<Supplier>
    {
        public override void Configure(EntityTypeBuilder<Supplier> builder)
        {
            base.Configure(builder);
            builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
            builder.Property(s => s.Website).HasMaxLength(500);

            builder.HasIndex(s => s.Name).IsUnique();

            builder.HasData(
                new Supplier { Id = 1, Name = "Bosanski Kovači d.o.o.", Website = "https://bosanskikovaci.ba", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Supplier { Id = 2, Name = "Zanatlija Mostar", Website = "https://zanatlija.ba", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Supplier { Id = 3, Name = "Drvni Majstor Tuzla", Website = "https://drvnimajstor.ba", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}

