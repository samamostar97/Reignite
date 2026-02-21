using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class ChatMessageConfiguration : BaseEntityConfiguration<ChatMessage>
    {
        public override void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            base.Configure(builder);

            builder.Property(m => m.Content).HasMaxLength(500).IsRequired();

            builder.HasOne(m => m.User)
                .WithMany(u => u.ChatMessages)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Hobby)
                .WithMany(h => h.ChatMessages)
                .HasForeignKey(m => m.HobbyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(m => new { m.HobbyId, m.CreatedAt });
        }
    }
}
