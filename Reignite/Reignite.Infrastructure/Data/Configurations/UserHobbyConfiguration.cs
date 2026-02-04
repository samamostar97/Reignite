using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class UserHobbyConfiguration : BaseEntityConfiguration<UserHobby>
    {
        public override void Configure(EntityTypeBuilder<UserHobby> builder)
        {
            base.Configure(builder);

            builder.Property(uh => uh.Bio).HasMaxLength(500);

            builder.HasIndex(uh => new { uh.UserId, uh.HobbyId })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");

            builder.HasOne(uh => uh.User)
                .WithMany(u => u.UserHobbies)
                .HasForeignKey(uh => uh.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(uh => uh.Hobby)
                .WithMany(h => h.UserHobbies)
                .HasForeignKey(uh => uh.HobbyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
