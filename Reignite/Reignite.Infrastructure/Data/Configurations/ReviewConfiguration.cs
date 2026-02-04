using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class ReviewConfiguration : BaseEntityConfiguration<Review>
    {
        public override void Configure(EntityTypeBuilder<Review> builder)
        {
            base.Configure(builder);
            builder.Property(r => r.Rating).IsRequired();
            builder.Property(r => r.Comment).HasMaxLength(1000);

            builder.HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Product)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => new { r.UserId, r.ProductId }).IsUnique().HasFilter("[IsDeleted] = 0");

            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Review { Id = 1, UserId = 2, ProductId = 1, Rating = 5, Comment = "Odličan čekić, savršena ravnoteža i čvrst zahvat. Preporučujem!", CreatedAt = seedDate },
                new Review { Id = 2, UserId = 2, ProductId = 3, Rating = 4, Comment = "Kvalitetna hrastova daska, samo je pakovanje moglo biti bolje.", CreatedAt = seedDate },
                new Review { Id = 3, UserId = 3, ProductId = 5, Rating = 5, Comment = "Savršen komplet za početnike, sve što vam treba na jednom mjestu.", CreatedAt = seedDate },
                new Review { Id = 4, UserId = 3, ProductId = 10, Rating = 4, Comment = "Dlijeto drži oštricu jako dobro, zadovoljna sam kupovinom.", CreatedAt = seedDate },
                new Review { Id = 5, UserId = 4, ProductId = 6, Rating = 5, Comment = "Vrhunski set za obradu kože, profesionalni kvalitet.", CreatedAt = seedDate },
                new Review { Id = 6, UserId = 4, ProductId = 9, Rating = 3, Comment = "Brusni papir je ok za cijenu, ali granulacija 80 se brzo troši.", CreatedAt = seedDate }
            );
        }
    }
}

