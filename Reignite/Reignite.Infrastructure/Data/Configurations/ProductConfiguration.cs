using Reignite.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reignite.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : BaseEntityConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);
            builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
            builder.Property(s => s.Price).HasPrecision(18, 2).IsRequired();
            builder.Property(s => s.Description).HasMaxLength(1000);
            builder.Property(u => u.ProductImageUrl).HasMaxLength(500);


            builder.HasOne(s => s.ProductCategory)
                .WithMany(c => c.Products)
                .HasForeignKey(s => s.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Supplier)
                .WithMany(sup => sup.Products)
                .HasForeignKey(s => s.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            builder.HasData(
                // Ručni Alati (Category 1)
                new Product { Id = 1, Name = "Čekić kovački 500g", Price = 45.00m, Description = "Profesionalni kovački čekić od kaljenog čelika, ergonomska drška od jasena.", ProductCategoryId = 1, SupplierId = 1, ProductImageUrl = "https://images.unsplash.com/photo-1586864387967-d02ef85d93e8?w=400", CreatedAt = seedDate },
                new Product { Id = 2, Name = "Set šrafcigera 12 kom", Price = 32.50m, Description = "Komplet od 12 preciznih šrafcigera sa magnetnim vrhovima.", ProductCategoryId = 1, SupplierId = 1, ProductImageUrl = "https://images.unsplash.com/photo-1581092921461-eab62e97a780?w=400", CreatedAt = seedDate },
                new Product { Id = 3, Name = "Dlijeto ručno 25mm", Price = 38.50m, Description = "Profesionalno dlijeto za drvo širine 25mm, oštrica od krom-vanadijskog čelika.", ProductCategoryId = 1, SupplierId = 1, ProductImageUrl = "https://images.unsplash.com/photo-1572981779307-38b8cabb2407?w=400", CreatedAt = seedDate },
                new Product { Id = 4, Name = "Ručna pila japanska", Price = 67.90m, Description = "Tradicionalna japanska pila za precizne rezove. Tanka oštrica za fine radove.", ProductCategoryId = 1, SupplierId = 2, ProductImageUrl = "https://images.unsplash.com/photo-1504148455328-c376907d081c?w=400", CreatedAt = seedDate },

                // Premium Materijali (Category 2)
                new Product { Id = 5, Name = "Hrastova daska premium", Price = 78.00m, Description = "Visokokvalitetna hrastova daska, dimenzije 200x30x3cm, sušena u komori.", ProductCategoryId = 2, SupplierId = 3, ProductImageUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400", CreatedAt = seedDate },
                new Product { Id = 6, Name = "Orahov furnir 1m²", Price = 55.90m, Description = "Prirodni orahov furnir debljine 0.6mm, idealan za završnu obradu namještaja.", ProductCategoryId = 2, SupplierId = 3, ProductImageUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400", CreatedAt = seedDate },
                new Product { Id = 7, Name = "Goveđa koža premium 1m²", Price = 85.00m, Description = "Visokokvalitetna goveđa koža debljine 1.5mm. Prirodna smeđa boja.", ProductCategoryId = 2, SupplierId = 2, ProductImageUrl = "https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=400", CreatedAt = seedDate },
                new Product { Id = 8, Name = "Bukov blok za rezbarenje", Price = 35.00m, Description = "Kvalitetni bukov blok dimenzija 15x15x30cm. Sušeno drvo bez čvorova.", ProductCategoryId = 2, SupplierId = 3, ProductImageUrl = "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400", CreatedAt = seedDate },

                // Kompleti za Početnike (Category 3)
                new Product { Id = 9, Name = "Stolarski komplet za početnike", Price = 149.99m, Description = "Sve za početak stolarije: ručna pila, čekić, 3 dlijeta, metar, kutnik i vodič.", ProductCategoryId = 3, SupplierId = 2, ProductImageUrl = "https://images.unsplash.com/photo-1530124566582-a618bc2615dc?w=400", CreatedAt = seedDate },
                new Product { Id = 10, Name = "Komplet za obradu kože", Price = 189.00m, Description = "Set za obradu kože: 2 noža, šila, igle, voskirani konac, probijač i kalup.", ProductCategoryId = 3, SupplierId = 2, ProductImageUrl = "https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=400", CreatedAt = seedDate },
                new Product { Id = 11, Name = "Set za rezbarenje početnički", Price = 79.90m, Description = "Osnovni set za rezbarenje: 4 noža, brusilica, 2 lipova bloka i priručnik.", ProductCategoryId = 3, SupplierId = 2, ProductImageUrl = "https://images.unsplash.com/photo-1452860606245-08befc0ff44b?w=400", CreatedAt = seedDate },
                new Product { Id = 12, Name = "Keramičarski starter kit", Price = 165.00m, Description = "Početni set za keramiku: 2kg gline, alati za modeliranje i upute.", ProductCategoryId = 3, SupplierId = 2, ProductImageUrl = "https://images.unsplash.com/photo-1493106641515-6b5631de4bb9?w=400", CreatedAt = seedDate },

                // Pribor za Radionicu (Category 4)
                new Product { Id = 13, Name = "Radionička pregača kožna", Price = 65.00m, Description = "Zaštitna pregača od prave goveđe kože sa 4 džepa za alat.", ProductCategoryId = 4, SupplierId = 1, ProductImageUrl = "https://images.unsplash.com/photo-1452860606245-08befc0ff44b?w=400", CreatedAt = seedDate },
                new Product { Id = 14, Name = "Stega stolarska 300mm", Price = 28.75m, Description = "Brzo-stezna stolarska stega sa čeljustima od 300mm, pritisak do 200kg.", ProductCategoryId = 4, SupplierId = 1, ProductImageUrl = "https://images.unsplash.com/photo-1504148455328-c376907d081c?w=400", CreatedAt = seedDate },
                new Product { Id = 15, Name = "Brusni papir set 50 kom", Price = 19.90m, Description = "Mješoviti set brusnih papira: granulacija 80, 120, 180, 240 i 320.", ProductCategoryId = 4, SupplierId = 3, ProductImageUrl = "https://images.unsplash.com/photo-1504148455328-c376907d081c?w=400", CreatedAt = seedDate }
            );
        }
    }
}
