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

            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                // Reviews for Project 1 (User 2's leather wallet)
                new ProjectReview { Id = 1, UserId = 3, ProjectId = 1, Rating = 5, Comment = "Prelijepo urađen novčanik! Šavovi su savršeni, vidi se pažnja u svakom detalju.", CreatedAt = seedDate },
                new ProjectReview { Id = 2, UserId = 4, ProjectId = 1, Rating = 4, Comment = "Odličan rad za prvi projekat. Koža je lijepo obrađena.", CreatedAt = seedDate },

                // Reviews for Project 2 (User 3's book shelf)
                new ProjectReview { Id = 3, UserId = 2, ProjectId = 2, Rating = 5, Comment = "Rustični stil je fantastičan! Hrast je prelijepo došao do izražaja.", CreatedAt = seedDate },
                new ProjectReview { Id = 4, UserId = 4, ProjectId = 2, Rating = 5, Comment = "Polica izgleda profesionalno. Svaka čast na vještini!", CreatedAt = seedDate },

                // Reviews for Project 3 (User 4's knife holder)
                new ProjectReview { Id = 5, UserId = 2, ProjectId = 3, Rating = 4, Comment = "Kombinacija drveta i metala je odlična ideja. Veoma praktično.", CreatedAt = seedDate },
                new ProjectReview { Id = 6, UserId = 3, ProjectId = 3, Rating = 5, Comment = "Ovo je pravo majstorstvo! Kovani detalji su nevjerovatni.", CreatedAt = seedDate },

                // Reviews for Project 4 (User 2's wooden spoons)
                new ProjectReview { Id = 7, UserId = 3, ProjectId = 4, Rating = 4, Comment = "Lijep set kutlača, prirodna obrada drveta je super.", CreatedAt = seedDate },
                new ProjectReview { Id = 8, UserId = 4, ProjectId = 4, Rating = 5, Comment = "Ručni rad u najboljem izdanju. Koristim ih svaki dan!", CreatedAt = seedDate },

                // Reviews for Project 5 (User 3's glasses case)
                new ProjectReview { Id = 9, UserId = 2, ProjectId = 5, Rating = 5, Comment = "Elegantna futrola, magnetni zatvarač je odličan dodatak.", CreatedAt = seedDate },
                new ProjectReview { Id = 10, UserId = 4, ProjectId = 5, Rating = 4, Comment = "Kvalitetna izrada, koža je mekana i ugodna na dodir.", CreatedAt = seedDate },

                // Reviews for Project 6 (User 4's candle holder)
                new ProjectReview { Id = 11, UserId = 2, ProjectId = 6, Rating = 5, Comment = "Moderni dizajn koji se uklapa u svaki prostor. Bravo!", CreatedAt = seedDate },
                new ProjectReview { Id = 12, UserId = 3, ProjectId = 6, Rating = 5, Comment = "Kovano željezo daje poseban šarm. Odlična završna obrada.", CreatedAt = seedDate }
            );
        }
    }
}
