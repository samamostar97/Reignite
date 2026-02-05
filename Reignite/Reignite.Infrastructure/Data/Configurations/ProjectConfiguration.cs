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
                    Description = "Moj prvi projekat sa kompletom za obradu kože. Napravio sam klasični muški novčanik sa 6 slotova za kartice i pretincem za novčanice. Koristio sam smeđu goveđu kožu i ručno šivanje sedlarskim bodom.",
                    HoursSpent = 12,
                    UserId = 2,
                    HobbyId = 2,
                    ProductId = 6,
                    ImageUrl = "/uploads/projects/project_1.jpg",
                    CreatedAt = seedDate
                },
                new Project
                {
                    Id = 2,
                    Title = "Rustikalna polica za knjige",
                    Description = "Od hrastove daske sam napravio zidnu policu u rustikalnom stilu. Dimenzije su 120x25cm, sa tri police. Završna obrada je ulje za drvo koje naglašava prirodnu teksturu hrasta.",
                    HoursSpent = 8,
                    UserId = 3,
                    HobbyId = 1,
                    ProductId = 3,
                    ImageUrl = "/uploads/projects/project_2.jpg",
                    CreatedAt = seedDate
                },
                new Project
                {
                    Id = 3,
                    Title = "Dekorativni stalak za noževe",
                    Description = "Kombinacija stolarije i metalnog rada. Baza je od orahovog furnira, a metalni nosači su ručno kovani. Drži 5 kuhinjskih noževa elegantno na kuhinjskom pultu.",
                    HoursSpent = 15,
                    UserId = 4,
                    HobbyId = 3,
                    ProductId = 4,
                    ImageUrl = "/uploads/projects/project_3.jpg",
                    CreatedAt = seedDate
                },
                new Project
                {
                    Id = 4,
                    Title = "Set drvenih kutlača",
                    Description = "Komplet od tri kutlače različitih veličina, ručno izrezbarene od jednog komada hrastovine. Savršene za svakodnevnu upotrebu u kuhinji, obrađene prehrambeno sigurnim uljem.",
                    HoursSpent = 6,
                    UserId = 2,
                    HobbyId = 1,
                    ProductId = 5,
                    ImageUrl = "/uploads/projects/project_4.jpg",
                    CreatedAt = seedDate
                },
                new Project
                {
                    Id = 5,
                    Title = "Kožna futrola za naočale",
                    Description = "Elegantna futrola za naočale od tamnosmeđe kože sa magnetnim zatvaračem. Unutrašnjost je obložena mekom tkaninom koja štiti stakla od ogrebotina.",
                    HoursSpent = 4,
                    UserId = 3,
                    HobbyId = 2,
                    ProductId = 6,
                    ImageUrl = "/uploads/projects/project_5.jpg",
                    CreatedAt = seedDate
                },
                new Project
                {
                    Id = 6,
                    Title = "Kovani držač za svijeće",
                    Description = "Dekorativni držač za tri svijeće, izrađen od kovanog željeza. Moderna geometrijska forma sa crnom mat završnom obradom. Idealan kao centralni element na trpezarijskom stolu.",
                    HoursSpent = 10,
                    UserId = 4,
                    HobbyId = 3,
                    ProductId = 1,
                    ImageUrl = "/uploads/projects/project_6.jpg",
                    CreatedAt = seedDate
                }
            );
        }
    }
}
