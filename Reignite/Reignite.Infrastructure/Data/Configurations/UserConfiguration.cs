using Reignite.Core.Entities;
using Reignite.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class UserConfiguration : BaseEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            builder.Property(u => u.Username).HasMaxLength(50).IsRequired();
            builder.Property(u => u.Email).HasMaxLength(255).IsRequired();
            builder.Property(u => u.PhoneNumber).HasMaxLength(20).IsRequired();
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.ProfileImageUrl).HasMaxLength(500);

            builder.HasIndex(u => u.PhoneNumber).IsUnique().HasFilter("[IsDeleted]= 0");
            builder.HasIndex(u => u.Username).IsUnique().HasFilter("[IsDeleted] = 0");
            builder.HasIndex(u => u.Email).IsUnique().HasFilter("[IsDeleted] = 0");

            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var userPasswordHash = "$2a$11$8DGhfdncLbX2/fqYXGvIQ.8PpoMUXEltO5KPjLW7D1mYc6850OmDm"; // password: Test123!

            builder.HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "Korisnik",
                    Username = "admin",
                    Email = "admin",
                    PhoneNumber = "+38761000000",
                    Role = UserRole.Admin,
                    PasswordHash = "$2a$11$r2qfyc5ic4xn12pabxQnqutUdAnwvk.pZk9868MOTFC4Jo6dQOCPK",
                    ProfileImageUrl = "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150",
                    CreatedAt = seedDate
                },
                new User
                {
                    Id = 2,
                    FirstName = "Amir",
                    LastName = "Hadžić",
                    Username = "amir.h",
                    Email = "amir@mail.com",
                    PhoneNumber = "+38762111111",
                    Role = UserRole.AppUser,
                    PasswordHash = userPasswordHash,
                    ProfileImageUrl = "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150",
                    CreatedAt = seedDate
                },
                new User
                {
                    Id = 3,
                    FirstName = "Lejla",
                    LastName = "Begović",
                    Username = "lejla.b",
                    Email = "lejla@mail.com",
                    PhoneNumber = "+38763222222",
                    Role = UserRole.AppUser,
                    PasswordHash = userPasswordHash,
                    ProfileImageUrl = "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=150",
                    CreatedAt = seedDate
                },
                new User
                {
                    Id = 4,
                    FirstName = "Tarik",
                    LastName = "Memić",
                    Username = "tarik.m",
                    Email = "tarik@mail.com",
                    PhoneNumber = "+38761333333",
                    Role = UserRole.AppUser,
                    PasswordHash = userPasswordHash,
                    ProfileImageUrl = "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=150",
                    CreatedAt = seedDate
                }
            );
        }
    }
}
