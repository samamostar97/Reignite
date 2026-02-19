using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Reignite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHasDataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Clear all dependent records that may reference seeded data (runtime seeder re-creates them)
            migrationBuilder.Sql("DELETE FROM [ProjectReviews]");
            migrationBuilder.Sql("DELETE FROM [ProductReviews]");
            migrationBuilder.Sql("DELETE FROM [OrderItems]");
            migrationBuilder.Sql("DELETE FROM [Orders]");
            migrationBuilder.Sql("DELETE FROM [WishlistItems]");
            migrationBuilder.Sql("DELETE FROM [Wishlist]");
            migrationBuilder.Sql("DELETE FROM [UserHobbies]");
            migrationBuilder.Sql("DELETE FROM [Projects]");
            migrationBuilder.Sql("DELETE FROM [Products]");
            migrationBuilder.Sql("DELETE FROM [ProductCategories]");
            migrationBuilder.Sql("DELETE FROM [Hobbies]");
            migrationBuilder.Sql("DELETE FROM [Coupons]");
            migrationBuilder.Sql("DELETE FROM [Faqs]");

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Faqs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Faqs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Faqs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Faqs",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Faqs",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrderItems",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProductReviews",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductReviews",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductReviews",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProductReviews",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProductReviews",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProductReviews",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "Id", "Code", "CreatedAt", "DiscountType", "DiscountValue", "ExpiryDate", "IsActive", "IsDeleted", "MaxUses", "MinimumOrderAmount", "TimesUsed" },
                values: new object[,]
                {
                    { 1, "DOBRODOSLI10", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Percentage", 10.00m, new DateTime(2026, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), true, false, 500, 50.00m, 47 },
                    { 2, "LJETO2026", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Percentage", 15.00m, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, false, 200, 100.00m, 23 },
                    { 3, "BESPLATNADOSTAVA", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fixed", 7.00m, new DateTime(2026, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), true, false, 1000, 50.00m, 156 }
                });

            migrationBuilder.InsertData(
                table: "Faqs",
                columns: new[] { "Id", "Answer", "CreatedAt", "IsDeleted", "Question" },
                values: new object[,]
                {
                    { 1, "Jednostavno dodajte željene proizvode u korpu, unesite podatke za dostavu i izvršite plaćanje. Narudžbu možete pratiti u sekciji 'Moje narudžbe'.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Kako mogu naručiti proizvode?" },
                    { 2, "Dostava na području Bosne i Hercegovine traje 2-5 radnih dana. Za veće narudžbe ili udaljenije lokacije, rok može biti do 7 radnih dana.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Koliko traje dostava?" },
                    { 3, "Dostava je besplatna za sve narudžbe iznad 100 KM. Za narudžbe ispod tog iznosa, cijena dostave je 7 KM.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Da li je dostava besplatna?" },
                    { 4, "Da, imate pravo na povrat proizvoda u roku od 14 dana od prijema. Proizvod mora biti nekorišten i u originalnom pakovanju.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Mogu li vratiti proizvod?" },
                    { 5, "Prihvatamo plaćanje karticama (Visa, Mastercard), pouzeće i bankovni transfer. Sva online plaćanja su zaštićena SSL enkripcijom.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Koji su načini plaćanja?" }
                });

            migrationBuilder.InsertData(
                table: "Hobbies",
                columns: new[] { "Id", "CreatedAt", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Umjetnost izrade predmeta od drveta, od namještaja do dekorativnih komada.", false, "Stolarija" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Izrada novčanika, remena, torbi i drugih predmeta od kože.", false, "Obrada kože" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Oblikovanje i kovanje metala u alate i funkcionalne predmete.", false, "Obrada metala" },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Oblikovanje gline u keramičke predmete, od posuda do skulptura.", false, "Keramika" },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Izrada jedinstvenih svijeća sa posebnim mirisima i dizajnom.", false, "Izrada svijeća" },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Umjetnost oblikovanja drveta ručnim alatima.", false, "Rezbarenje" }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "PurchaseDate", "Status", "StripePaymentId", "TotalAmount", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, 123.00m, 2 },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, null, 188.49m, 3 },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, null, 84.80m, 4 }
                });

            migrationBuilder.InsertData(
                table: "ProductCategories",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Ručni Alati" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Premium Materijali" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Kompleti za Početnike" },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Pribor za Radionicu" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "Description", "IsDeleted", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Profesionalni kovački čekić od kaljenog čelika, ergonomska drška od jasena.", false, "Čekić kovački 500g", 45.00m, 1, "https://images.unsplash.com/photo-1586864387967-d02ef85d93e8?w=400", 1 },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Komplet od 12 preciznih šrafcigera sa magnetnim vrhovima.", false, "Set šrafcigera 12 kom", 32.50m, 1, "https://images.unsplash.com/photo-1581092921461-eab62e97a780?w=400", 1 },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Profesionalno dlijeto za drvo širine 25mm, oštrica od krom-vanadijskog čelika.", false, "Dlijeto ručno 25mm", 38.50m, 1, "https://images.unsplash.com/photo-1572981779307-38b8cabb2407?w=400", 1 },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tradicionalna japanska pila za precizne rezove. Tanka oštrica za fine radove.", false, "Ručna pila japanska", 67.90m, 1, "https://images.unsplash.com/photo-1504148455328-c376907d081c?w=400", 2 },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Visokokvalitetna hrastova daska, dimenzije 200x30x3cm, sušena u komori.", false, "Hrastova daska premium", 78.00m, 2, "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400", 3 },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Prirodni orahov furnir debljine 0.6mm, idealan za završnu obradu namještaja.", false, "Orahov furnir 1m²", 55.90m, 2, "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400", 3 },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Visokokvalitetna goveđa koža debljine 1.5mm. Prirodna smeđa boja.", false, "Goveđa koža premium 1m²", 85.00m, 2, "https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=400", 2 },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kvalitetni bukov blok dimenzija 15x15x30cm. Sušeno drvo bez čvorova.", false, "Bukov blok za rezbarenje", 35.00m, 2, "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400", 3 },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sve za početak stolarije: ručna pila, čekić, 3 dlijeta, metar, kutnik i vodič.", false, "Stolarski komplet za početnike", 149.99m, 3, "https://images.unsplash.com/photo-1530124566582-a618bc2615dc?w=400", 2 },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Set za obradu kože: 2 noža, šila, igle, voskirani konac, probijač i kalup.", false, "Komplet za obradu kože", 189.00m, 3, "https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=400", 2 },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Osnovni set za rezbarenje: 4 noža, brusilica, 2 lipova bloka i priručnik.", false, "Set za rezbarenje početnički", 79.90m, 3, "https://images.unsplash.com/photo-1452860606245-08befc0ff44b?w=400", 2 },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Početni set za keramiku: 2kg gline, alati za modeliranje i upute.", false, "Keramičarski starter kit", 165.00m, 3, "https://images.unsplash.com/photo-1493106641515-6b5631de4bb9?w=400", 2 },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Zaštitna pregača od prave goveđe kože sa 4 džepa za alat.", false, "Radionička pregača kožna", 65.00m, 4, "https://images.unsplash.com/photo-1452860606245-08befc0ff44b?w=400", 1 },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Brzo-stezna stolarska stega sa čeljustima od 300mm, pritisak do 200kg.", false, "Stega stolarska 300mm", 28.75m, 4, "https://images.unsplash.com/photo-1504148455328-c376907d081c?w=400", 1 },
                    { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mješoviti set brusnih papira: granulacija 80, 120, 180, 240 i 320.", false, "Brusni papir set 50 kom", 19.90m, 4, "https://images.unsplash.com/photo-1504148455328-c376907d081c?w=400", 3 }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "OrderId", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 1, 1, 1, 45.00m },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 1, 3, 1, 78.00m },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 2, 10, 1, 38.50m },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 2, 5, 1, 149.99m },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 3, 9, 2, 19.90m },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 3, 1, 1, 45.00m }
                });

            migrationBuilder.InsertData(
                table: "ProductReviews",
                columns: new[] { "Id", "Comment", "CreatedAt", "IsDeleted", "ProductId", "Rating", "UserId" },
                values: new object[,]
                {
                    { 1, "Odličan čekić, savršena ravnoteža i čvrst zahvat. Preporučujem!", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 1, 5, 2 },
                    { 2, "Kvalitetna hrastova daska, samo je pakovanje moglo biti bolje.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 5, 4, 2 },
                    { 3, "Savršen komplet za početnike, sve što treba na jednom mjestu.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 9, 5, 3 },
                    { 4, "Dlijeto drži oštricu jako dobro, zadovoljna sam kupovinom.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 3, 4, 3 },
                    { 5, "Komplet za kožu ima sve potrebno. Napravio sam novčanik!", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 10, 5, 4 },
                    { 6, "Brusni papir je ok za cijenu, ali se brže troši.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 15, 4, 4 }
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "CreatedAt", "Description", "HobbyId", "HoursSpent", "ImageUrl", "IsDeleted", "ProductId", "Title", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Moj prvi projekat sa kompletom za obradu kože. Napravio sam klasični muški novčanik sa 6 slotova za kartice.", 2, 12, "https://images.unsplash.com/photo-1627123424574-724758594e93?w=400", false, 10, "Ručno izrađeni novčanik od kože", 2 },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Komplet od tri kutlače različitih veličina, ručno izrezbarene od jednog komada hrastovine.", 1, 6, "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=400", false, 9, "Set drvenih kutlača", 2 },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Od hrastove daske sam napravila zidnu policu u rustikalnom stilu. Dimenzije su 120x25cm.", 1, 8, "https://images.unsplash.com/photo-1594620302200-9a762244a156?w=400", false, 5, "Rustikalna polica za knjige", 3 },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Elegantna futrola za naočale od tamnosmeđe kože sa magnetnim zatvaračem.", 2, 4, "https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=400", false, 10, "Kožna futrola za naočale", 3 },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dekorativna figurica sove visine 15cm, izrezbarena od bukovog drveta.", 6, 20, "https://images.unsplash.com/photo-1452860606245-08befc0ff44b?w=400", false, 11, "Rezbarena figurica sove", 4 },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ručno oblikovana keramička zdjela promjera 30cm. Glazirana u toplim zemljanim tonovima.", 4, 8, "https://images.unsplash.com/photo-1493106641515-6b5631de4bb9?w=400", false, 12, "Keramička zdjela za voće", 4 }
                });

            migrationBuilder.InsertData(
                table: "ProjectReviews",
                columns: new[] { "Id", "Comment", "CreatedAt", "IsDeleted", "ProjectId", "Rating", "UserId" },
                values: new object[,]
                {
                    { 1, "Prelijepo urađen novčanik! Šavovi su savršeni.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 1, 5, 3 },
                    { 2, "Odličan rad za prvi projekat. Koža je lijepo obrađena.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 1, 4, 4 },
                    { 3, "Kutlače su prelijepe! Prirodna forma drveta je očuvana.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 2, 5, 3 },
                    { 4, "Praktične i lijepe, idealan poklon za kuhinju.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 2, 4, 4 },
                    { 5, "Rustični stil je fantastičan! Hrast je prelijepo došao do izražaja.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 3, 5, 2 },
                    { 6, "Polica izgleda profesionalno. Svaka čast na vještini!", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 3, 5, 4 },
                    { 7, "Elegantna futrola, magnetni zatvarač je odličan dodatak.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 4, 5, 2 },
                    { 8, "Kvalitetna izrada, koža je mekana i ugodna na dodir.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 4, 4, 4 },
                    { 9, "Rezbarenje je nevjerovatno! Svako pero je vidljivo.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 5, 5, 2 },
                    { 10, "Ova sova izgleda kao da će progovoriti! Talent!", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 5, 5, 3 },
                    { 11, "Keramička zdjela je prelijepa! Boje su tople i privlačne.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 6, 5, 2 },
                    { 12, "Ručni rad se vidi u svakom detalju. Divno!", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, 6, 5, 3 }
                });
        }
    }
}
