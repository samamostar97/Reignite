using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Reignite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SimplifiedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MaxUses", "TimesUsed" },
                values: new object[] { 500, 47 });

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "MaxUses", "TimesUsed" },
                values: new object[] { 200, 23 });

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Code", "DiscountValue", "ExpiryDate", "MaxUses", "MinimumOrderAmount", "TimesUsed" },
                values: new object[] { "BESPLATNADOSTAVA", 7.00m, new DateTime(2026, 12, 31, 23, 59, 59, 0, DateTimeKind.Utc), 1000, 50.00m, 156 });

            migrationBuilder.UpdateData(
                table: "Faqs",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Answer", "Question" },
                values: new object[] { "Dostava je besplatna za sve narudžbe iznad 100 KM. Za narudžbe ispod tog iznosa, cijena dostave je 7 KM.", "Da li je dostava besplatna?" });

            migrationBuilder.UpdateData(
                table: "Faqs",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Answer", "Question" },
                values: new object[] { "Da, imate pravo na povrat proizvoda u roku od 14 dana od prijema. Proizvod mora biti nekorišten i u originalnom pakovanju.", "Mogu li vratiti proizvod?" });

            migrationBuilder.UpdateData(
                table: "Faqs",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Answer", "Question" },
                values: new object[] { "Prihvatamo plaćanje karticama (Visa, Mastercard), pouzeće i bankovni transfer. Sva online plaćanja su zaštićena SSL enkripcijom.", "Koji su načini plaćanja?" });

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 1,
                column: "IconUrl",
                value: "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=100");

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 2,
                column: "IconUrl",
                value: "https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=100");

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "IconUrl" },
                values: new object[] { "Oblikovanje i kovanje metala u alate i funkcionalne predmete.", "https://images.unsplash.com/photo-1504328345606-18bbc8c9d7d1?w=100" });

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 4,
                column: "IconUrl",
                value: "https://images.unsplash.com/photo-1493106641515-6b5631de4bb9?w=100");

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 5,
                column: "IconUrl",
                value: "https://images.unsplash.com/photo-1602523961358-f9f03dd557db?w=100");

            migrationBuilder.InsertData(
                table: "Hobbies",
                columns: new[] { "Id", "CreatedAt", "Description", "IconUrl", "IsDeleted", "Name" },
                values: new object[] { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Umjetnost oblikovanja drveta ručnim alatima.", "https://images.unsplash.com/photo-1452860606245-08befc0ff44b?w=100", false, "Rezbarenje" });

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Kompleti za Početnike");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProductImageUrl",
                value: "https://images.unsplash.com/photo-1586864387967-d02ef85d93e8?w=400");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "ProductImageUrl" },
                values: new object[] { "Komplet od 12 preciznih šrafcigera sa magnetnim vrhovima.", "https://images.unsplash.com/photo-1581092921461-eab62e97a780?w=400" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[] { "Profesionalno dlijeto za drvo širine 25mm, oštrica od krom-vanadijskog čelika.", "Dlijeto ručno 25mm", 38.50m, 1, "https://images.unsplash.com/photo-1572981779307-38b8cabb2407?w=400", 1 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[] { "Tradicionalna japanska pila za precizne rezove. Tanka oštrica za fine radove.", "Ručna pila japanska", 67.90m, 1, "https://images.unsplash.com/photo-1504148455328-c376907d081c?w=400", 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[] { "Visokokvalitetna hrastova daska, dimenzije 200x30x3cm, sušena u komori.", "Hrastova daska premium", 78.00m, 2, "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400", 3 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[] { "Prirodni orahov furnir debljine 0.6mm, idealan za završnu obradu namještaja.", "Orahov furnir 1m²", 55.90m, 2, "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400", 3 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl" },
                values: new object[] { "Visokokvalitetna goveđa koža debljine 1.5mm. Prirodna smeđa boja.", "Goveđa koža premium 1m²", 85.00m, 2, "https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=400" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[] { "Kvalitetni bukov blok dimenzija 15x15x30cm. Sušeno drvo bez čvorova.", "Bukov blok za rezbarenje", 35.00m, 2, "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400", 3 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[] { "Sve za početak stolarije: ručna pila, čekić, 3 dlijeta, metar, kutnik i vodič.", "Stolarski komplet za početnike", 149.99m, 3, "https://images.unsplash.com/photo-1530124566582-a618bc2615dc?w=400", 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[] { "Set za obradu kože: 2 noža, šila, igle, voskirani konac, probijač i kalup.", "Komplet za obradu kože", 189.00m, 3, "https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=400", 2 });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CreatedAt", "Description", "IsDeleted", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[,]
                {
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Osnovni set za rezbarenje: 4 noža, brusilica, 2 lipova bloka i priručnik.", false, "Set za rezbarenje početnički", 79.90m, 3, "https://images.unsplash.com/photo-1452860606245-08befc0ff44b?w=400", 2 },
                    { 12, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Početni set za keramiku: 2kg gline, alati za modeliranje i upute.", false, "Keramičarski starter kit", 165.00m, 3, "https://images.unsplash.com/photo-1493106641515-6b5631de4bb9?w=400", 2 },
                    { 13, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Zaštitna pregača od prave goveđe kože sa 4 džepa za alat.", false, "Radionička pregača kožna", 65.00m, 4, "https://images.unsplash.com/photo-1556909114-44e3e9699e2b?w=400", 1 },
                    { 14, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Brzo-stezna stolarska stega sa čeljustima od 300mm, pritisak do 200kg.", false, "Stega stolarska 300mm", 28.75m, 4, "https://images.unsplash.com/photo-1504148455328-c376907d081c?w=400", 1 },
                    { 15, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mješoviti set brusnih papira: granulacija 80, 120, 180, 240 i 320.", false, "Brusni papir set 50 kom", 19.90m, 4, "https://images.unsplash.com/photo-1504148455328-c376907d081c?w=400", 3 }
                });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "Comment",
                value: "Prelijepo urađen novčanik! Šavovi su savršeni.");

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Comment", "UserId" },
                values: new object[] { "Kutlače su prelijepe! Prirodna forma drveta je očuvana.", 3 });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Comment", "Rating" },
                values: new object[] { "Praktične i lijepe, idealan poklon za kuhinju.", 4 });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Comment", "Rating" },
                values: new object[] { "Rustični stil je fantastičan! Hrast je prelijepo došao do izražaja.", 5 });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Comment", "UserId" },
                values: new object[] { "Polica izgleda profesionalno. Svaka čast na vještini!", 4 });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Comment", "Rating", "UserId" },
                values: new object[] { "Elegantna futrola, magnetni zatvarač je odličan dodatak.", 5, 2 });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Comment", "Rating" },
                values: new object[] { "Kvalitetna izrada, koža je mekana i ugodna na dodir.", 4 });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 9,
                column: "Comment",
                value: "Rezbarenje je nevjerovatno! Svako pero je vidljivo.");

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Comment", "Rating", "UserId" },
                values: new object[] { "Ova sova izgleda kao da će progovoriti! Talent!", 5, 3 });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 11,
                column: "Comment",
                value: "Keramička zdjela je prelijepa! Boje su tople i privlačne.");

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 12,
                column: "Comment",
                value: "Ručni rad se vidi u svakom detalju. Divno!");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "ImageUrl", "ProductId" },
                values: new object[] { "Moj prvi projekat sa kompletom za obradu kože. Napravio sam klasični muški novčanik sa 6 slotova za kartice.", "https://images.unsplash.com/photo-1627123424574-724758594e93?w=400", 10 });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "HoursSpent", "ImageUrl", "ProductId", "Title", "UserId" },
                values: new object[] { "Komplet od tri kutlače različitih veličina, ručno izrezbarene od jednog komada hrastovine.", 6, "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=400", 9, "Set drvenih kutlača", 2 });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "HobbyId", "HoursSpent", "ImageUrl", "ProductId", "Title", "UserId" },
                values: new object[] { "Od hrastove daske sam napravila zidnu policu u rustikalnom stilu. Dimenzije su 120x25cm.", 1, 8, "https://images.unsplash.com/photo-1594620302200-9a762244a156?w=400", 5, "Rustikalna polica za knjige", 3 });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "HobbyId", "HoursSpent", "ImageUrl", "ProductId", "Title", "UserId" },
                values: new object[] { "Elegantna futrola za naočale od tamnosmeđe kože sa magnetnim zatvaračem.", 2, 4, "https://images.unsplash.com/photo-1584917865442-de89df76afd3?w=400", 10, "Kožna futrola za naočale", 3 });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "HobbyId", "HoursSpent", "ImageUrl", "ProductId", "Title", "UserId" },
                values: new object[] { "Dekorativna figurica sove visine 15cm, izrezbarena od bukovog drveta.", 6, 20, "https://images.unsplash.com/photo-1452860606245-08befc0ff44b?w=400", 11, "Rezbarena figurica sove", 4 });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "HobbyId", "HoursSpent", "ImageUrl", "ProductId", "Title" },
                values: new object[] { "Ručno oblikovana keramička zdjela promjera 30cm. Glazirana u toplim zemljanim tonovima.", 4, 8, "https://images.unsplash.com/photo-1493106641515-6b5631de4bb9?w=400", 12, "Keramička zdjela za voće" });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProductId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Comment", "ProductId" },
                values: new object[] { "Savršen komplet za početnike, sve što treba na jednom mjestu.", 9 });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 4,
                column: "ProductId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Comment", "ProductId" },
                values: new object[] { "Komplet za kožu ima sve potrebno. Napravio sam novčanik!", 10 });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Comment", "ProductId", "Rating" },
                values: new object[] { "Brusni papir je ok za cijenu, ali se brže troši.", 15, 4 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "PasswordHash", "ProfileImageUrl" },
                values: new object[] { "admin", "$2a$11$r2qfyc5ic4xn12pabxQnqutUdAnwvk.pZk9868MOTFC4Jo6dQOCPK", "https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=150" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProfileImageUrl",
                value: "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=150");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "ProfileImageUrl",
                value: "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=150");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "ProfileImageUrl",
                value: "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=150");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MaxUses", "TimesUsed" },
                values: new object[] { 100, 0 });

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "MaxUses", "TimesUsed" },
                values: new object[] { 50, 0 });

            migrationBuilder.UpdateData(
                table: "Coupons",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Code", "DiscountValue", "ExpiryDate", "MaxUses", "MinimumOrderAmount", "TimesUsed" },
                values: new object[] { "BESPLATNA", 10.00m, new DateTime(2026, 6, 30, 23, 59, 59, 0, DateTimeKind.Utc), 200, 30.00m, 0 });

            migrationBuilder.UpdateData(
                table: "Faqs",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Answer", "Question" },
                values: new object[] { "Da, imate pravo na povrat proizvoda u roku od 14 dana od prijema. Proizvod mora biti nekorišten i u originalnom pakovanju. Kontaktirajte našu podršku za pokretanje povrata.", "Mogu li vratiti proizvod?" });

            migrationBuilder.UpdateData(
                table: "Faqs",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Answer", "Question" },
                values: new object[] { "Prihvatamo plaćanje karticama (Visa, Mastercard), pouzeće i bankovni transfer. Sva online plaćanja su zaštićena SSL enkripcijom.", "Koji su načini plaćanja?" });

            migrationBuilder.UpdateData(
                table: "Faqs",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Answer", "Question" },
                values: new object[] { "Unesite kod kupona u predviđeno polje prilikom završetka narudžbe. Popust će biti automatski primijenjen na vašu narudžbu ako su ispunjeni uslovi kupona.", "Kako mogu koristiti kupon za popust?" });

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 1,
                column: "IconUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 2,
                column: "IconUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "IconUrl" },
                values: new object[] { "Oblikovanje i kovanje metala u alate, umjetničke i funkcionalne predmete.", null });

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 4,
                column: "IconUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hobbies",
                keyColumn: "Id",
                keyValue: 5,
                column: "IconUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "ProductCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Komplete za Početnike");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "ProductImageUrl",
                value: "/uploads/products/product_1.jpg");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "ProductImageUrl" },
                values: new object[] { "Komplet od 12 preciznih šrafcigera sa magnetnim vrhovima, ravni i krstasti.", "/uploads/products/product_2.jpg" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[] { "Visokokvalitetna hrastova daska, dimenzije 200x30x3cm, sušena u komori.", "Hrastova daska premium", 78.00m, 2, "/uploads/products/product_3.jpg", 3 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[] { "Prirodni orahov furnir debljine 0.6mm, idealan za završnu obradu namještaja.", "Orahov furnir 1m²", 55.90m, 2, "/uploads/products/product_4.jpg", 3 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[] { "Sve što vam treba za početak stolarije: ručna pila, čekić, dlijeta, metar i kutnik.", "Stolarski komplet za početnike", 149.99m, 3, "/uploads/products/product_5.jpg", 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[] { "Profesionalni set za obradu kože: noževi, šila, igle, konac i kalup za šivanje.", "Komplet za obradu kože", 189.00m, 3, "/uploads/products/product_6.jpg", 2 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl" },
                values: new object[] { "Zaštitna pregača od prave goveđe kože sa džepovima za alat.", "Radionička pregača kožna", 65.00m, 4, "/uploads/products/product_7.jpg" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[] { "Brzo-stezna stolarska stega sa čeljustima od 300mm, pritisak do 200kg.", "Stega stolarska 300mm", 28.75m, 4, "/uploads/products/product_8.jpg", 1 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[] { "Mješoviti set brusnih papira: granulacija 80, 120, 180, 240 i 320, po 10 komada.", "Brusni papir set 50 kom", 19.90m, 4, "/uploads/products/product_9.jpg", 3 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Description", "Name", "Price", "ProductCategoryId", "ProductImageUrl", "SupplierId" },
                values: new object[] { "Profesionalno dlijeto za drvo širine 25mm, oštrica od krom-vanadijskog čelika.", "Dlijeto ručno 25mm", 38.50m, 1, "/uploads/products/product_10.jpg", 1 });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "Comment",
                value: "Prelijepo urađen novčanik! Šavovi su savršeni, vidi se pažnja u svakom detalju.");

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Comment", "UserId" },
                values: new object[] { "Rustični stil je fantastičan! Hrast je prelijepo došao do izražaja.", 2 });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Comment", "Rating" },
                values: new object[] { "Polica izgleda profesionalno. Svaka čast na vještini!", 5 });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Comment", "Rating" },
                values: new object[] { "Kombinacija drveta i metala je odlična ideja. Veoma praktično.", 4 });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Comment", "UserId" },
                values: new object[] { "Ovo je pravo majstorstvo! Kovani detalji su nevjerovatni.", 3 });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Comment", "Rating", "UserId" },
                values: new object[] { "Lijep set kutlača, prirodna obrada drveta je super.", 4, 3 });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Comment", "Rating" },
                values: new object[] { "Ručni rad u najboljem izdanju. Koristim ih svaki dan!", 5 });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 9,
                column: "Comment",
                value: "Elegantna futrola, magnetni zatvarač je odličan dodatak.");

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Comment", "Rating", "UserId" },
                values: new object[] { "Kvalitetna izrada, koža je mekana i ugodna na dodir.", 4, 4 });

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 11,
                column: "Comment",
                value: "Moderni dizajn koji se uklapa u svaki prostor. Bravo!");

            migrationBuilder.UpdateData(
                table: "ProjectReviews",
                keyColumn: "Id",
                keyValue: 12,
                column: "Comment",
                value: "Kovano željezo daje poseban šarm. Odlična završna obrada.");

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "ImageUrl", "ProductId" },
                values: new object[] { "Moj prvi projekat sa kompletom za obradu kože. Napravio sam klasični muški novčanik sa 6 slotova za kartice i pretincem za novčanice. Koristio sam smeđu goveđu kožu i ručno šivanje sedlarskim bodom.", "/uploads/projects/project_1.jpg", 6 });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "HoursSpent", "ImageUrl", "ProductId", "Title", "UserId" },
                values: new object[] { "Od hrastove daske sam napravio zidnu policu u rustikalnom stilu. Dimenzije su 120x25cm, sa tri police. Završna obrada je ulje za drvo koje naglašava prirodnu teksturu hrasta.", 8, "/uploads/projects/project_2.jpg", 3, "Rustikalna polica za knjige", 3 });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "HobbyId", "HoursSpent", "ImageUrl", "ProductId", "Title", "UserId" },
                values: new object[] { "Kombinacija stolarije i metalnog rada. Baza je od orahovog furnira, a metalni nosači su ručno kovani. Drži 5 kuhinjskih noževa elegantno na kuhinjskom pultu.", 3, 15, "/uploads/projects/project_3.jpg", 4, "Dekorativni stalak za noževe", 4 });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "HobbyId", "HoursSpent", "ImageUrl", "ProductId", "Title", "UserId" },
                values: new object[] { "Komplet od tri kutlače različitih veličina, ručno izrezbarene od jednog komada hrastovine. Savršene za svakodnevnu upotrebu u kuhinji, obrađene prehrambeno sigurnim uljem.", 1, 6, "/uploads/projects/project_4.jpg", 5, "Set drvenih kutlača", 2 });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Description", "HobbyId", "HoursSpent", "ImageUrl", "ProductId", "Title", "UserId" },
                values: new object[] { "Elegantna futrola za naočale od tamnosmeđe kože sa magnetnim zatvaračem. Unutrašnjost je obložena mekom tkaninom koja štiti stakla od ogrebotina.", 2, 4, "/uploads/projects/project_5.jpg", 6, "Kožna futrola za naočale", 3 });

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Description", "HobbyId", "HoursSpent", "ImageUrl", "ProductId", "Title" },
                values: new object[] { "Dekorativni držač za tri svijeće, izrađen od kovanog željeza. Moderna geometrijska forma sa crnom mat završnom obradom. Idealan kao centralni element na trpezarijskom stolu.", 3, 10, "/uploads/projects/project_6.jpg", 1, "Kovani držač za svijeće" });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProductId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Comment", "ProductId" },
                values: new object[] { "Savršen komplet za početnike, sve što vam treba na jednom mjestu.", 5 });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 4,
                column: "ProductId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Comment", "ProductId" },
                values: new object[] { "Vrhunski set za obradu kože, profesionalni kvalitet.", 6 });

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Comment", "ProductId", "Rating" },
                values: new object[] { "Brusni papir je ok za cijenu, ali granulacija 80 se brzo troši.", 9, 3 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "PasswordHash", "ProfileImageUrl" },
                values: new object[] { "admin@reignite.com", "$2a$11$u7rBIoSp8ouEtSRDsEUwCe3PXUpc3EoPZTYhp6D0abbXyOEOk.gjS", null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "ProfileImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "ProfileImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                column: "ProfileImageUrl",
                value: null);
        }
    }
}
