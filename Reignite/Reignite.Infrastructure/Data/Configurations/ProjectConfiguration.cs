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

            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Project
                {
                    Id = 1,
                    Title = "Ručno izrađeni novčanik od kože",
                    Description = "Moj prvi projekat sa kompletom za obradu kože. Napravio sam klasični muški novčanik sa 6 slotova za kartice.",
                    HoursSpent = 12,
                    UserId = 2,
                    HobbyId = 2,
                    ProductId = 10,
                    ImageUrl = "https://images.unsplash.com/photo-1627123424574-724758594e93?w=400",
                    CreatedAt = seedDate
                },
                new Project
                {
                    Id = 2,
                    Title = "Set drvenih kutlača",
                    Description = "Komplet od tri kutlače različitih veličina, ručno izrezbarene od jednog komada hrastovine.",
                    HoursSpent = 6,
                    UserId = 2,
                    HobbyId = 1,
                    ProductId = 9,
                    ImageUrl = "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=400",
                    CreatedAt = seedDate
                },
                new Project
                {
                    Id = 3,
                    Title = "Rustikalna polica za knjige",
                    Description = "Od hrastove daske sam napravila zidnu policu u rustikalnom stilu. Dimenzije su 120x25cm.",
                    HoursSpent = 8,
                    UserId = 3,
                    HobbyId = 1,
                    ProductId = 5,
                    ImageUrl = "https://images.unsplash.com/photo-1594620302200-9a762244a156?w=400",
                    CreatedAt = seedDate
                },
                new Project
                {
                    Id = 4,
                    Title = "Kožna futrola za naočale",
                    Description = "Elegantna futrola za naočale od tamnosmeđe kože sa magnetnim zatvaračem.",
                    HoursSpent = 4,
                    UserId = 3,
                    HobbyId = 2,
                    ProductId = 10,
                    ImageUrl = "https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=400",
                    CreatedAt = seedDate
                },
                new Project
                {
                    Id = 5,
                    Title = "Rezbarena figurica sove",
                    Description = "Dekorativna figurica sove visine 15cm, izrezbarena od bukovog drveta.",
                    HoursSpent = 20,
                    UserId = 4,
                    HobbyId = 6,
                    ProductId = 11,
                    ImageUrl = "https://images.unsplash.com/photo-1452860606245-08befc0ff44b?w=400",
                    CreatedAt = seedDate
                },
                new Project
                {
                    Id = 6,
                    Title = "Keramička zdjela za voće",
                    Description = "Ručno oblikovana keramička zdjela promjera 30cm. Glazirana u toplim zemljanim tonovima.",
                    HoursSpent = 8,
                    UserId = 4,
                    HobbyId = 4,
                    ProductId = 12,
                    ImageUrl = "https://images.unsplash.com/photo-1493106641515-6b5631de4bb9?w=400",
                    CreatedAt = seedDate
                }
            );
        }
    }
}
