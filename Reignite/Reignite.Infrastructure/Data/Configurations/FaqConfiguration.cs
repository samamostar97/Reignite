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

            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                new Faq { Id = 1, Question = "Kako mogu naručiti proizvode?", Answer = "Jednostavno dodajte željene proizvode u korpu, unesite podatke za dostavu i izvršite plaćanje. Narudžbu možete pratiti u sekciji 'Moje narudžbe'.", CreatedAt = seedDate },
                new Faq { Id = 2, Question = "Koliko traje dostava?", Answer = "Dostava na području Bosne i Hercegovine traje 2-5 radnih dana. Za veće narudžbe ili udaljenije lokacije, rok može biti do 7 radnih dana.", CreatedAt = seedDate },
                new Faq { Id = 3, Question = "Da li je dostava besplatna?", Answer = "Dostava je besplatna za sve narudžbe iznad 100 KM. Za narudžbe ispod tog iznosa, cijena dostave je 7 KM.", CreatedAt = seedDate },
                new Faq { Id = 4, Question = "Mogu li vratiti proizvod?", Answer = "Da, imate pravo na povrat proizvoda u roku od 14 dana od prijema. Proizvod mora biti nekorišten i u originalnom pakovanju.", CreatedAt = seedDate },
                new Faq { Id = 5, Question = "Koji su načini plaćanja?", Answer = "Prihvatamo plaćanje karticama (Visa, Mastercard), pouzeće i bankovni transfer. Sva online plaćanja su zaštićena SSL enkripcijom.", CreatedAt = seedDate }
            );
        }
    }
}
