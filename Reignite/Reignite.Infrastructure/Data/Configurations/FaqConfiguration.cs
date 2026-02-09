using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class FaqConfiguration : BaseEntityConfiguration<Faq>
    {
        public override void Configure(EntityTypeBuilder<Faq> builder)
        {
            base.Configure(builder);
            builder.Property(f => f.Question).HasMaxLength(500).IsRequired();
            builder.Property(f => f.Answer).HasMaxLength(2000).IsRequired();
        }
    }
}
