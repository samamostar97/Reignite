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
                new Product { Id = 1, Name = "Čekić kovački 500g", Price = 45.00m, Description = "Profesionalni kovački čekić od kaljenog čelika, ergonomska drška od jasena.", ProductCategoryId = 1, SupplierId = 1, CreatedAt = seedDate },
                new Product { Id = 2, Name = "Set šrafcigera 12 kom", Price = 32.50m, Description = "Komplet od 12 preciznih šrafcigera sa magnetnim vrhovima, ravni i krstasti.", ProductCategoryId = 1, SupplierId = 1, CreatedAt = seedDate },
                new Product { Id = 3, Name = "Hrastova daska premium", Price = 78.00m, Description = "Visokokvalitetna hrastova daska, dimenzije 200x30x3cm, sušena u komori.", ProductCategoryId = 2, SupplierId = 3, CreatedAt = seedDate },
                new Product { Id = 4, Name = "Orahov furnir 1m²", Price = 55.90m, Description = "Prirodni orahov furnir debljine 0.6mm, idealan za završnu obradu namještaja.", ProductCategoryId = 2, SupplierId = 3, CreatedAt = seedDate },
                new Product { Id = 5, Name = "Stolarski komplet za početnike", Price = 149.99m, Description = "Sve što vam treba za početak stolarije: ručna pila, čekić, dlijeta, metar i kutnik.", ProductCategoryId = 3, SupplierId = 2, CreatedAt = seedDate },
                new Product { Id = 6, Name = "Komplet za obradu kože", Price = 189.00m, Description = "Profesionalni set za obradu kože: noževi, šila, igle, konac i kalup za šivanje.", ProductCategoryId = 3, SupplierId = 2, CreatedAt = seedDate },
                new Product { Id = 7, Name = "Radionička pregača kožna", Price = 65.00m, Description = "Zaštitna pregača od prave goveđe kože sa džepovima za alat.", ProductCategoryId = 4, SupplierId = 2, CreatedAt = seedDate },
                new Product { Id = 8, Name = "Stega stolarska 300mm", Price = 28.75m, Description = "Brzo-stezna stolarska stega sa čeljustima od 300mm, pritisak do 200kg.", ProductCategoryId = 4, SupplierId = 1, CreatedAt = seedDate },
                new Product { Id = 9, Name = "Brusni papir set 50 kom", Price = 19.90m, Description = "Mješoviti set brusnih papira: granulacija 80, 120, 180, 240 i 320, po 10 komada.", ProductCategoryId = 4, SupplierId = 3, CreatedAt = seedDate },
                new Product { Id = 10, Name = "Dlijeto ručno 25mm", Price = 38.50m, Description = "Profesionalno dlijeto za drvo širine 25mm, oštrica od krom-vanadijskog čelika.", ProductCategoryId = 1, SupplierId = 1, CreatedAt = seedDate }
            );
        }
    }
}

