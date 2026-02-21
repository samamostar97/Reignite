using Microsoft.EntityFrameworkCore;
using Reignite.Core.Entities;
using Reignite.Core.Enums;

namespace Reignite.Infrastructure.Data;

public static class DatabaseSeeder
{
    // All seed dates are relative to "now" so the data always looks fresh
    private static readonly DateTime Now = DateTime.UtcNow;
    private static readonly DateTime SeedDate = Ago(90);

    private static DateTime Ago(int days, int hour = 0, int minute = 0)
        => Now.AddDays(-days).Date.AddHours(hour).AddMinutes(minute);

    private static DateTime FromNow(int days)
        => Now.AddDays(days).Date;

    public static async Task SeedAsync(ReigniteDbContext context, string webRootPath)
    {
        // InMemory provider doesn't support migrations, transactions, or raw SQL
        if (context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
            return;

        await context.Database.MigrateAsync();

        var hasAdmin = await context.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == "admin@reignite.ba");
        var hasProducts = await context.Products.AnyAsync();

        if (hasAdmin && hasProducts)
        {
            Console.WriteLine("[Seeder] Baza vec popunjena.");
            return;
        }

        Console.WriteLine("[Seeder] Popunjavam bazu podataka...");

        // Seed data FIRST — database must always be populated even if images fail
        await ClearAndSeedAsync(context);
        Console.WriteLine("[Seeder] Baza uspjesno popunjena!");

        // Download images AFTER seeding — non-fatal if it fails
        try
        {
            await CopySeedImagesAsync(webRootPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Seeder] Upozorenje: Slike nisu kopirane ({ex.Message}). Aplikacija ce raditi bez slika.");
        }
    }

    private static Task CopySeedImagesAsync(string webRootPath)
    {
        Console.WriteLine("[Seeder] Kopiram slike...");

        foreach (var dir in new[] { "users", "products", "projects" })
            Directory.CreateDirectory(Path.Combine(webRootPath, "uploads", "seed", dir));

        // Locate bundled seed images relative to the executing assembly
        var assemblyDir = Path.GetDirectoryName(typeof(DatabaseSeeder).Assembly.Location)!;
        var seedImagesDir = Path.Combine(assemblyDir, "Data", "SeedImages");

        var copies = new List<(string source, string destination)>();

        // Product images (bundled)
        for (int i = 1; i <= 18; i++)
            copies.Add((
                Path.Combine(seedImagesDir, "products", $"product{i}.jpg"),
                Path.Combine(webRootPath, "uploads", "seed", "products", $"product{i}.jpg")));

        // Project images (bundled)
        for (int i = 1; i <= 8; i++)
            copies.Add((
                Path.Combine(seedImagesDir, "projects", $"project{i}.jpg"),
                Path.Combine(webRootPath, "uploads", "seed", "projects", $"project{i}.jpg")));

        // User images — still download from picsum.photos (no bundled images)
        var userDownloads = new List<(string url, string destination)>();
        for (int i = 1; i <= 7; i++)
            userDownloads.Add((
                $"https://picsum.photos/seed/reignite-u{i}/200/200",
                Path.Combine(webRootPath, "uploads", "seed", "users", $"user{i}.jpg")));

        int copied = 0;
        foreach (var (source, destination) in copies)
        {
            if (File.Exists(destination)) continue;
            if (!File.Exists(source))
            {
                Console.WriteLine($"  Nedostaje: {source}");
                continue;
            }
            File.Copy(source, destination, overwrite: false);
            copied++;
        }
        Console.WriteLine($"[Seeder] Kopirano {copied} slika.");

        // Download user avatars from picsum (still placeholder — no real user photos)
        _ = Task.Run(async () =>
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            foreach (var (url, destination) in userDownloads)
            {
                if (File.Exists(destination)) continue;
                try
                {
                    var bytes = await http.GetByteArrayAsync(url);
                    await File.WriteAllBytesAsync(destination, bytes);
                }
                catch { /* Non-critical */ }
            }
        });

        return Task.CompletedTask;
    }

    private static async Task ClearAndSeedAsync(ReigniteDbContext context)
    {
        // Everything inside a single transaction — atomic clear + seed
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            // Delete in FK-safe order (children before parents)
            var tablesToClear = new[]
            {
                "WishlistItems", "Wishlist", "OrderItems", "ProjectReviews",
                "ProductReviews", "Projects", "UserHobbies", "UserAddresses",
                "RefreshTokens", "Orders", "Products", "ProductCategories",
                "Suppliers", "Hobbies", "Coupons", "Faqs", "Users"
            };

            foreach (var table in tablesToClear)
            {
                await context.Database.ExecuteSqlRawAsync("DELETE FROM [" + table + "]");
                try { await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[" + table + "]', RESEED, 0)"); }
                catch { /* Table might not have identity column */ }
            }

            var adminHash = BCrypt.Net.BCrypt.HashPassword("test");
            var userHash = BCrypt.Net.BCrypt.HashPassword("test123");

            // ==================== USERS (7) ====================
            var users = new[]
            {
                new User { Id = 1, FirstName = "Admin", LastName = "Korisnik", Username = "admin", Email = "admin@reignite.ba", PhoneNumber = "+38761000000", Role = UserRole.Admin, PasswordHash = adminHash, ProfileImageUrl = "/uploads/seed/users/user1.jpg", CreatedAt = SeedDate },
                new User { Id = 2, FirstName = "Test", LastName = "Korisnik", Username = "test", Email = "test@reignite.ba", PhoneNumber = "+38762000000", Role = UserRole.AppUser, PasswordHash = adminHash, ProfileImageUrl = "/uploads/seed/users/user2.jpg", CreatedAt = SeedDate },
                new User { Id = 3, FirstName = "Amir", LastName = "Hadzic", Username = "amir.h", Email = "amir@mail.com", PhoneNumber = "+38762111111", Role = UserRole.AppUser, PasswordHash = userHash, ProfileImageUrl = "/uploads/seed/users/user3.jpg", CreatedAt = SeedDate },
                new User { Id = 4, FirstName = "Lejla", LastName = "Begovic", Username = "lejla.b", Email = "lejla@mail.com", PhoneNumber = "+38763222222", Role = UserRole.AppUser, PasswordHash = userHash, ProfileImageUrl = "/uploads/seed/users/user4.jpg", CreatedAt = SeedDate },
                new User { Id = 5, FirstName = "Tarik", LastName = "Memic", Username = "tarik.m", Email = "tarik@mail.com", PhoneNumber = "+38761333333", Role = UserRole.AppUser, PasswordHash = userHash, ProfileImageUrl = "/uploads/seed/users/user5.jpg", CreatedAt = SeedDate },
                new User { Id = 6, FirstName = "Amina", LastName = "Delic", Username = "amina.d", Email = "amina@mail.com", PhoneNumber = "+38762444444", Role = UserRole.AppUser, PasswordHash = userHash, ProfileImageUrl = "/uploads/seed/users/user6.jpg", CreatedAt = SeedDate },
                new User { Id = 7, FirstName = "Kenan", LastName = "Mujic", Username = "kenan.m", Email = "kenan@mail.com", PhoneNumber = "+38763555555", Role = UserRole.AppUser, PasswordHash = userHash, ProfileImageUrl = "/uploads/seed/users/user7.jpg", CreatedAt = SeedDate },
            };
            await InsertWithIdentityAsync(context, "Users", users, 7);

            // ==================== CATEGORIES (5) ====================
            var categories = new[]
            {
                new ProductCategory { Id = 1, Name = "Rucni Alati", CreatedAt = SeedDate },
                new ProductCategory { Id = 2, Name = "Premium Materijali", CreatedAt = SeedDate },
                new ProductCategory { Id = 3, Name = "Kompleti za Pocetnike", CreatedAt = SeedDate },
                new ProductCategory { Id = 4, Name = "Pribor za Radionicu", CreatedAt = SeedDate },
                new ProductCategory { Id = 5, Name = "Boje i Premazi", CreatedAt = SeedDate },
            };
            await InsertWithIdentityAsync(context, "ProductCategories", categories, 5);

            // ==================== SUPPLIERS (4) ====================
            var suppliers = new[]
            {
                new Supplier { Id = 1, Name = "ZanatCraft d.o.o.", Website = "https://zanatcraft.ba", CreatedAt = SeedDate },
                new Supplier { Id = 2, Name = "MajstorShop Sarajevo", Website = "https://majstorshop.ba", CreatedAt = SeedDate },
                new Supplier { Id = 3, Name = "DrvoProm Zenica", Website = "https://drvoprom.ba", CreatedAt = SeedDate },
                new Supplier { Id = 4, Name = "MetalArt Tuzla", Website = "https://metalart.ba", CreatedAt = SeedDate },
            };
            await InsertWithIdentityAsync(context, "Suppliers", suppliers, 4);

            // ==================== HOBBIES (6) ====================
            var hobbies = new[]
            {
                new Hobby { Id = 1, Name = "Stolarija", Description = "Umjetnost izrade predmeta od drveta, od namjestaja do dekorativnih komada.", CreatedAt = SeedDate },
                new Hobby { Id = 2, Name = "Obrada koze", Description = "Izrada novcanika, remena, torbi i drugih predmeta od koze.", CreatedAt = SeedDate },
                new Hobby { Id = 3, Name = "Obrada metala", Description = "Oblikovanje i kovanje metala u alate i funkcionalne predmete.", CreatedAt = SeedDate },
                new Hobby { Id = 4, Name = "Keramika", Description = "Oblikovanje gline u keramicke predmete, od posuda do skulptura.", CreatedAt = SeedDate },
                new Hobby { Id = 5, Name = "Izrada svijeca", Description = "Izrada jedinstvenih svijeca sa posebnim mirisima i dizajnom.", CreatedAt = SeedDate },
                new Hobby { Id = 6, Name = "Rezbarenje", Description = "Umjetnost oblikovanja drveta rucnim alatima.", CreatedAt = SeedDate },
            };
            await InsertWithIdentityAsync(context, "Hobbies", hobbies, 6);

            // ==================== PRODUCTS (18) ====================
            var products = new[]
            {
                // Category 1: Rucni Alati (Supplier 1, 2)
                new Product { Id = 1, Name = "Cekic kovacki 500g", Price = 45.00m, Description = "Profesionalni kovacki cekic od kaljenog celika, ergonomska drska od jasena.", ProductCategoryId = 1, SupplierId = 1, ProductImageUrl = "/uploads/seed/products/product1.jpg", CreatedAt = SeedDate },
                new Product { Id = 2, Name = "Set srafcigera 12 kom", Price = 32.50m, Description = "Komplet od 12 preciznih srafcigera sa magnetnim vrhovima.", ProductCategoryId = 1, SupplierId = 1, ProductImageUrl = "/uploads/seed/products/product2.jpg", CreatedAt = SeedDate },
                new Product { Id = 3, Name = "Dlijeto rucno 25mm", Price = 38.50m, Description = "Profesionalno dlijeto za drvo sirine 25mm, ostrica od krom-vanadijskog celika.", ProductCategoryId = 1, SupplierId = 1, ProductImageUrl = "/uploads/seed/products/product3.jpg", CreatedAt = SeedDate },
                new Product { Id = 4, Name = "Rucna pila japanska", Price = 67.90m, Description = "Tradicionalna japanska pila za precizne rezove. Tanka ostrica za fine radove.", ProductCategoryId = 1, SupplierId = 2, ProductImageUrl = "/uploads/seed/products/product4.jpg", CreatedAt = SeedDate },

                // Category 2: Premium Materijali (Supplier 2, 3)
                new Product { Id = 5, Name = "Hrastova daska premium", Price = 78.00m, Description = "Visokokvalitetna hrastova daska, dimenzije 200x30x3cm, susena u komori.", ProductCategoryId = 2, SupplierId = 3, ProductImageUrl = "/uploads/seed/products/product5.jpg", CreatedAt = SeedDate },
                new Product { Id = 6, Name = "Orahov furnir 1m2", Price = 55.90m, Description = "Prirodni orahov furnir debljine 0.6mm, idealan za zavrsnu obradu namjestaja.", ProductCategoryId = 2, SupplierId = 3, ProductImageUrl = "/uploads/seed/products/product6.jpg", CreatedAt = SeedDate },
                new Product { Id = 7, Name = "Govedja koza premium 1m2", Price = 85.00m, Description = "Visokokvalitetna govedja koza debljine 1.5mm. Prirodna smedja boja.", ProductCategoryId = 2, SupplierId = 2, ProductImageUrl = "/uploads/seed/products/product7.jpg", CreatedAt = SeedDate },
                new Product { Id = 8, Name = "Bukov blok za rezbarenje", Price = 35.00m, Description = "Kvalitetni bukov blok dimenzija 15x15x30cm. Suseno drvo bez cvorova.", ProductCategoryId = 2, SupplierId = 3, ProductImageUrl = "/uploads/seed/products/product8.jpg", CreatedAt = SeedDate },

                // Category 3: Kompleti za Pocetnike (Supplier 2)
                new Product { Id = 9, Name = "Stolarski komplet za pocetnike", Price = 149.99m, Description = "Sve za pocetak stolarije: rucna pila, cekic, 3 dlijeta, metar, kutnik i vodic.", ProductCategoryId = 3, SupplierId = 2, ProductImageUrl = "/uploads/seed/products/product9.jpg", CreatedAt = SeedDate },
                new Product { Id = 10, Name = "Komplet za obradu koze", Price = 189.00m, Description = "Set za obradu koze: 2 noza, sila, igle, voskirani konac, probijac i kalup.", ProductCategoryId = 3, SupplierId = 2, ProductImageUrl = "/uploads/seed/products/product10.jpg", CreatedAt = SeedDate },
                new Product { Id = 11, Name = "Set za rezbarenje pocetnicki", Price = 79.90m, Description = "Osnovni set za rezbarenje: 4 noza, brusilica, 2 lipova bloka i prirucnik.", ProductCategoryId = 3, SupplierId = 2, ProductImageUrl = "/uploads/seed/products/product11.jpg", CreatedAt = SeedDate },
                new Product { Id = 12, Name = "Keramicarski starter kit", Price = 165.00m, Description = "Pocetni set za keramiku: 2kg gline, alati za modeliranje i upute.", ProductCategoryId = 3, SupplierId = 2, ProductImageUrl = "/uploads/seed/products/product12.jpg", CreatedAt = SeedDate },

                // Category 4: Pribor za Radionicu (Supplier 1, 3)
                new Product { Id = 13, Name = "Radionicka pregaca kozna", Price = 65.00m, Description = "Zastitna pregaca od prave govedje koze sa 4 dzepa za alat.", ProductCategoryId = 4, SupplierId = 1, ProductImageUrl = "/uploads/seed/products/product13.jpg", CreatedAt = SeedDate },
                new Product { Id = 14, Name = "Stega stolarska 300mm", Price = 28.75m, Description = "Brzo-stezna stolarska stega sa celjustima od 300mm, pritisak do 200kg.", ProductCategoryId = 4, SupplierId = 1, ProductImageUrl = "/uploads/seed/products/product14.jpg", CreatedAt = SeedDate },
                new Product { Id = 15, Name = "Brusni papir set 50 kom", Price = 19.90m, Description = "Mjesoviti set brusnih papira: granulacija 80, 120, 180, 240 i 320.", ProductCategoryId = 4, SupplierId = 3, ProductImageUrl = "/uploads/seed/products/product15.jpg", CreatedAt = SeedDate },

                // Category 5: Boje i Premazi (Supplier 3, 4)
                new Product { Id = 16, Name = "Lak za drvo mat 1L", Price = 24.90m, Description = "Visokokvalitetni mat lak za zastitu drvenih povrsina. Bez mirisa, brzo se susi.", ProductCategoryId = 5, SupplierId = 3, ProductImageUrl = "/uploads/seed/products/product16.jpg", CreatedAt = SeedDate },
                new Product { Id = 17, Name = "Ulje za drvo laneno 500ml", Price = 18.50m, Description = "Prirodno laneno ulje za njegu i zastitu drveta. Pojacava prirodnu teksturu.", ProductCategoryId = 5, SupplierId = 3, ProductImageUrl = "/uploads/seed/products/product17.jpg", CreatedAt = SeedDate },
                new Product { Id = 18, Name = "Boja za kozu set 6 boja", Price = 42.00m, Description = "Set od 6 boja za kozu na bazi vode. Postojane boje, lagana aplikacija.", ProductCategoryId = 5, SupplierId = 4, ProductImageUrl = "/uploads/seed/products/product18.jpg", CreatedAt = SeedDate },
            };
            await InsertWithIdentityAsync(context, "Products", products, 18);

            // ==================== USER ADDRESSES (6) ====================
            var addresses = new[]
            {
                new UserAddress { Id = 1, UserId = 2, Street = "Marsala Tita 1", City = "Sarajevo", PostalCode = "71000", Country = "Bosna i Hercegovina", CreatedAt = SeedDate },
                new UserAddress { Id = 2, UserId = 3, Street = "Zmaja od Bosne 15", City = "Sarajevo", PostalCode = "71000", Country = "Bosna i Hercegovina", CreatedAt = SeedDate },
                new UserAddress { Id = 3, UserId = 4, Street = "Bulevar Mese Selimovica 22", City = "Mostar", PostalCode = "88000", Country = "Bosna i Hercegovina", CreatedAt = SeedDate },
                new UserAddress { Id = 4, UserId = 5, Street = "Turalibegova 50", City = "Tuzla", PostalCode = "75000", Country = "Bosna i Hercegovina", CreatedAt = SeedDate },
                new UserAddress { Id = 5, UserId = 6, Street = "Titova 8", City = "Zenica", PostalCode = "72000", Country = "Bosna i Hercegovina", CreatedAt = SeedDate },
                new UserAddress { Id = 6, UserId = 7, Street = "Kulina bana 3", City = "Bihac", PostalCode = "77000", Country = "Bosna i Hercegovina", CreatedAt = SeedDate },
            };
            await InsertWithIdentityAsync(context, "UserAddresses", addresses, 6);

            // ==================== USER HOBBIES (15) ====================
            var userHobbies = new[]
            {
                // User 2 (Test): Stolarija + Keramika
                new UserHobby { Id = 1, UserId = 2, HobbyId = 1, SkillLevel = SkillLevel.Intermediate, Bio = "Volim raditi male projekte od drveta.", CreatedAt = SeedDate },
                new UserHobby { Id = 2, UserId = 2, HobbyId = 4, SkillLevel = SkillLevel.Beginner, Bio = "Tek sam poceo sa keramikom.", CreatedAt = SeedDate },
                // User 3 (Amir): Stolarija + Rezbarenje + Obrada metala
                new UserHobby { Id = 3, UserId = 3, HobbyId = 1, SkillLevel = SkillLevel.Advanced, Bio = "Stolar sa 5 godina iskustva.", CreatedAt = SeedDate },
                new UserHobby { Id = 4, UserId = 3, HobbyId = 6, SkillLevel = SkillLevel.Intermediate, Bio = "Rezbarim dekorativne predmete.", CreatedAt = SeedDate },
                new UserHobby { Id = 5, UserId = 3, HobbyId = 3, SkillLevel = SkillLevel.Beginner, Bio = "Ucim osnove kovackog zanata.", CreatedAt = SeedDate },
                // User 4 (Lejla): Obrada koze + Izrada svijeca
                new UserHobby { Id = 6, UserId = 4, HobbyId = 2, SkillLevel = SkillLevel.Advanced, Bio = "Pravim rucne torbe i novcanike.", CreatedAt = SeedDate },
                new UserHobby { Id = 7, UserId = 4, HobbyId = 5, SkillLevel = SkillLevel.Beginner, Bio = "Svijece pravim kao poklon.", CreatedAt = SeedDate },
                // User 5 (Tarik): Rezbarenje + Izrada svijeca + Obrada metala
                new UserHobby { Id = 8, UserId = 5, HobbyId = 6, SkillLevel = SkillLevel.Advanced, Bio = "Specijaliziran za figurativno rezbarenje.", CreatedAt = SeedDate },
                new UserHobby { Id = 9, UserId = 5, HobbyId = 5, SkillLevel = SkillLevel.Intermediate, Bio = "Pravim mirisne svijece od prirodnog voska.", CreatedAt = SeedDate },
                new UserHobby { Id = 10, UserId = 5, HobbyId = 3, SkillLevel = SkillLevel.Intermediate, Bio = "Radim sa bakrom i mesingom.", CreatedAt = SeedDate },
                // User 6 (Amina): Keramika + Obrada koze
                new UserHobby { Id = 11, UserId = 6, HobbyId = 4, SkillLevel = SkillLevel.Advanced, Bio = "Keramicar sa vlastitom radionicom.", CreatedAt = SeedDate },
                new UserHobby { Id = 12, UserId = 6, HobbyId = 2, SkillLevel = SkillLevel.Beginner, Bio = "Pocetnica u radu sa kozom.", CreatedAt = SeedDate },
                // User 7 (Kenan): Obrada metala + Stolarija + Rezbarenje
                new UserHobby { Id = 13, UserId = 7, HobbyId = 3, SkillLevel = SkillLevel.Intermediate, Bio = "Kovac hobista, pravim nozeve.", CreatedAt = SeedDate },
                new UserHobby { Id = 14, UserId = 7, HobbyId = 1, SkillLevel = SkillLevel.Beginner, Bio = "Pocinjem sa jednostavnim projektima.", CreatedAt = SeedDate },
                new UserHobby { Id = 15, UserId = 7, HobbyId = 6, SkillLevel = SkillLevel.Beginner, Bio = "Tek sam kupio prvi set za rezbarenje.", CreatedAt = SeedDate },
            };
            await InsertWithIdentityAsync(context, "UserHobbies", userHobbies, 15);

            // ==================== ORDERS (20) ====================
            var orders = new[]
            {
                // ~3.5 months ago - early adoption
                new Order { Id = 1, UserId = 3, TotalAmount = 64.90m, PurchaseDate = Ago(108, 14, 30), Status = OrderStatus.Delivered, CreatedAt = Ago(108, 14, 30) },
                new Order { Id = 2, UserId = 4, TotalAmount = 149.99m, PurchaseDate = Ago(101, 10, 15), Status = OrderStatus.Delivered, CreatedAt = Ago(101, 10, 15) },
                new Order { Id = 3, UserId = 2, TotalAmount = 116.50m, PurchaseDate = Ago(93, 16, 45), Status = OrderStatus.Delivered, CreatedAt = Ago(93, 16, 45) },

                // ~2-2.5 months ago - peak period
                new Order { Id = 4, UserId = 5, TotalAmount = 274.00m, PurchaseDate = Ago(80, 9, 20), Status = OrderStatus.Delivered, CreatedAt = Ago(80, 9, 20) },
                new Order { Id = 5, UserId = 3, TotalAmount = 86.15m, PurchaseDate = Ago(75, 11, 0), Status = OrderStatus.Delivered, CreatedAt = Ago(75, 11, 0) },
                new Order { Id = 6, UserId = 6, TotalAmount = 165.00m, PurchaseDate = Ago(71, 13, 30), Status = OrderStatus.Delivered, CreatedAt = Ago(71, 13, 30) },
                new Order { Id = 7, UserId = 2, TotalAmount = 102.90m, PurchaseDate = Ago(68, 17, 0), Status = OrderStatus.Delivered, CreatedAt = Ago(68, 17, 0) },
                new Order { Id = 8, UserId = 7, TotalAmount = 144.90m, PurchaseDate = Ago(65, 15, 15), Status = OrderStatus.Delivered, CreatedAt = Ago(65, 15, 15) },
                new Order { Id = 9, UserId = 4, TotalAmount = 55.90m, PurchaseDate = Ago(61, 12, 0), Status = OrderStatus.Cancelled, CreatedAt = Ago(61, 12, 0) },
                new Order { Id = 10, UserId = 5, TotalAmount = 102.00m, PurchaseDate = Ago(55, 10, 45), Status = OrderStatus.Delivered, CreatedAt = Ago(55, 10, 45) },

                // ~1-1.5 months ago - slower period
                new Order { Id = 11, UserId = 3, TotalAmount = 48.65m, PurchaseDate = Ago(47, 11, 30), Status = OrderStatus.Delivered, CreatedAt = Ago(47, 11, 30) },
                new Order { Id = 12, UserId = 6, TotalAmount = 42.00m, PurchaseDate = Ago(40, 14, 0), Status = OrderStatus.Delivered, CreatedAt = Ago(40, 14, 0) },
                new Order { Id = 13, UserId = 2, TotalAmount = 174.89m, PurchaseDate = Ago(34, 16, 30), Status = OrderStatus.Cancelled, CreatedAt = Ago(34, 16, 30) },
                new Order { Id = 14, UserId = 7, TotalAmount = 113.00m, PurchaseDate = Ago(27, 9, 45), Status = OrderStatus.Delivered, CreatedAt = Ago(27, 9, 45) },

                // ~2-3 weeks ago - recent, mixed statuses
                new Order { Id = 15, UserId = 4, TotalAmount = 246.40m, PurchaseDate = Ago(20, 10, 0), Status = OrderStatus.Delivered, CreatedAt = Ago(20, 10, 0) },
                new Order { Id = 16, UserId = 3, TotalAmount = 150.00m, PurchaseDate = Ago(18, 13, 15), Status = OrderStatus.OnDelivery, CreatedAt = Ago(18, 13, 15) },
                new Order { Id = 17, UserId = 5, TotalAmount = 165.00m, PurchaseDate = Ago(16, 15, 0), Status = OrderStatus.Processing, CreatedAt = Ago(16, 15, 0) },
                new Order { Id = 18, UserId = 2, TotalAmount = 147.80m, PurchaseDate = Ago(15, 11, 30), Status = OrderStatus.OnDelivery, CreatedAt = Ago(15, 11, 30) },
                new Order { Id = 19, UserId = 6, TotalAmount = 83.50m, PurchaseDate = Ago(14, 14, 45), Status = OrderStatus.Processing, CreatedAt = Ago(14, 14, 45) },
                new Order { Id = 20, UserId = 7, TotalAmount = 149.99m, PurchaseDate = Ago(13, 8, 0), Status = OrderStatus.Processing, CreatedAt = Ago(13, 8, 0) },
            };
            await InsertWithIdentityAsync(context, "Orders", orders, 20);

            // ==================== ORDER ITEMS (37) ====================
            var orderItems = new[]
            {
                // Order 1 (64.90): Cekic + Brusni papir
                new OrderItem { Id = 1, OrderId = 1, ProductId = 1, Quantity = 1, UnitPrice = 45.00m, CreatedAt = orders[0].CreatedAt },
                new OrderItem { Id = 2, OrderId = 1, ProductId = 15, Quantity = 1, UnitPrice = 19.90m, CreatedAt = orders[0].CreatedAt },
                // Order 2 (149.99): Stolarski komplet
                new OrderItem { Id = 3, OrderId = 2, ProductId = 9, Quantity = 1, UnitPrice = 149.99m, CreatedAt = orders[1].CreatedAt },
                // Order 3 (116.50): Hrastova daska + Dlijeto
                new OrderItem { Id = 4, OrderId = 3, ProductId = 5, Quantity = 1, UnitPrice = 78.00m, CreatedAt = orders[2].CreatedAt },
                new OrderItem { Id = 5, OrderId = 3, ProductId = 3, Quantity = 1, UnitPrice = 38.50m, CreatedAt = orders[2].CreatedAt },
                // Order 4 (274.00): Komplet koza + Govedja koza
                new OrderItem { Id = 6, OrderId = 4, ProductId = 10, Quantity = 1, UnitPrice = 189.00m, CreatedAt = orders[3].CreatedAt },
                new OrderItem { Id = 7, OrderId = 4, ProductId = 7, Quantity = 1, UnitPrice = 85.00m, CreatedAt = orders[3].CreatedAt },
                // Order 5 (86.15): Srafcigeri + Stega + Lak
                new OrderItem { Id = 8, OrderId = 5, ProductId = 2, Quantity = 1, UnitPrice = 32.50m, CreatedAt = orders[4].CreatedAt },
                new OrderItem { Id = 9, OrderId = 5, ProductId = 14, Quantity = 1, UnitPrice = 28.75m, CreatedAt = orders[4].CreatedAt },
                new OrderItem { Id = 10, OrderId = 5, ProductId = 16, Quantity = 1, UnitPrice = 24.90m, CreatedAt = orders[4].CreatedAt },
                // Order 6 (165.00): Keramicarski kit
                new OrderItem { Id = 11, OrderId = 6, ProductId = 12, Quantity = 1, UnitPrice = 165.00m, CreatedAt = orders[5].CreatedAt },
                // Order 7 (102.90): Japanska pila + Bukov blok
                new OrderItem { Id = 12, OrderId = 7, ProductId = 4, Quantity = 1, UnitPrice = 67.90m, CreatedAt = orders[6].CreatedAt },
                new OrderItem { Id = 13, OrderId = 7, ProductId = 8, Quantity = 1, UnitPrice = 35.00m, CreatedAt = orders[6].CreatedAt },
                // Order 8 (144.90): Set rezbarenje + Pregaca
                new OrderItem { Id = 14, OrderId = 8, ProductId = 11, Quantity = 1, UnitPrice = 79.90m, CreatedAt = orders[7].CreatedAt },
                new OrderItem { Id = 15, OrderId = 8, ProductId = 13, Quantity = 1, UnitPrice = 65.00m, CreatedAt = orders[7].CreatedAt },
                // Order 9 (55.90): Orahov furnir - CANCELLED
                new OrderItem { Id = 16, OrderId = 9, ProductId = 6, Quantity = 1, UnitPrice = 55.90m, CreatedAt = orders[8].CreatedAt },
                // Order 10 (102.00): Cekic + Ulje + Dlijeto
                new OrderItem { Id = 17, OrderId = 10, ProductId = 1, Quantity = 1, UnitPrice = 45.00m, CreatedAt = orders[9].CreatedAt },
                new OrderItem { Id = 18, OrderId = 10, ProductId = 17, Quantity = 1, UnitPrice = 18.50m, CreatedAt = orders[9].CreatedAt },
                new OrderItem { Id = 19, OrderId = 10, ProductId = 3, Quantity = 1, UnitPrice = 38.50m, CreatedAt = orders[9].CreatedAt },
                // Order 11 (48.65): Stega + Brusni papir
                new OrderItem { Id = 20, OrderId = 11, ProductId = 14, Quantity = 1, UnitPrice = 28.75m, CreatedAt = orders[10].CreatedAt },
                new OrderItem { Id = 21, OrderId = 11, ProductId = 15, Quantity = 1, UnitPrice = 19.90m, CreatedAt = orders[10].CreatedAt },
                // Order 12 (42.00): Boja za kozu
                new OrderItem { Id = 22, OrderId = 12, ProductId = 18, Quantity = 1, UnitPrice = 42.00m, CreatedAt = orders[11].CreatedAt },
                // Order 13 (174.89): Stolarski komplet + Lak - CANCELLED
                new OrderItem { Id = 23, OrderId = 13, ProductId = 9, Quantity = 1, UnitPrice = 149.99m, CreatedAt = orders[12].CreatedAt },
                new OrderItem { Id = 24, OrderId = 13, ProductId = 16, Quantity = 1, UnitPrice = 24.90m, CreatedAt = orders[12].CreatedAt },
                // Order 14 (113.00): Hrastova daska + Bukov blok
                new OrderItem { Id = 25, OrderId = 14, ProductId = 5, Quantity = 1, UnitPrice = 78.00m, CreatedAt = orders[13].CreatedAt },
                new OrderItem { Id = 26, OrderId = 14, ProductId = 8, Quantity = 1, UnitPrice = 35.00m, CreatedAt = orders[13].CreatedAt },
                // Order 15 (246.40): Komplet koza + Srafcigeri + Lak
                new OrderItem { Id = 27, OrderId = 15, ProductId = 10, Quantity = 1, UnitPrice = 189.00m, CreatedAt = orders[14].CreatedAt },
                new OrderItem { Id = 28, OrderId = 15, ProductId = 2, Quantity = 1, UnitPrice = 32.50m, CreatedAt = orders[14].CreatedAt },
                new OrderItem { Id = 29, OrderId = 15, ProductId = 16, Quantity = 1, UnitPrice = 24.90m, CreatedAt = orders[14].CreatedAt },
                // Order 16 (150.00): Govedja koza + Pregaca
                new OrderItem { Id = 30, OrderId = 16, ProductId = 7, Quantity = 1, UnitPrice = 85.00m, CreatedAt = orders[15].CreatedAt },
                new OrderItem { Id = 31, OrderId = 16, ProductId = 13, Quantity = 1, UnitPrice = 65.00m, CreatedAt = orders[15].CreatedAt },
                // Order 17 (165.00): Keramicarski kit
                new OrderItem { Id = 32, OrderId = 17, ProductId = 12, Quantity = 1, UnitPrice = 165.00m, CreatedAt = orders[16].CreatedAt },
                // Order 18 (147.80): Japanska pila + Set rezbarenje
                new OrderItem { Id = 33, OrderId = 18, ProductId = 4, Quantity = 1, UnitPrice = 67.90m, CreatedAt = orders[17].CreatedAt },
                new OrderItem { Id = 34, OrderId = 18, ProductId = 11, Quantity = 1, UnitPrice = 79.90m, CreatedAt = orders[17].CreatedAt },
                // Order 19 (83.50): Cekic + Dlijeto
                new OrderItem { Id = 35, OrderId = 19, ProductId = 1, Quantity = 1, UnitPrice = 45.00m, CreatedAt = orders[18].CreatedAt },
                new OrderItem { Id = 36, OrderId = 19, ProductId = 3, Quantity = 1, UnitPrice = 38.50m, CreatedAt = orders[18].CreatedAt },
                // Order 20 (149.99): Stolarski komplet
                new OrderItem { Id = 37, OrderId = 20, ProductId = 9, Quantity = 1, UnitPrice = 149.99m, CreatedAt = orders[19].CreatedAt },
            };
            await InsertWithIdentityAsync(context, "OrderItems", orderItems, 37);

            // ==================== PRODUCT REVIEWS (18) ====================
            var productReviews = new[]
            {
                new ProductReview { Id = 1, UserId = 3, ProductId = 1, Rating = 5, Comment = "Odlican cekic, savrsen balans. Koristim ga svakodnevno u radionici.", CreatedAt = Ago(103) },
                new ProductReview { Id = 2, UserId = 4, ProductId = 2, Rating = 4, Comment = "Kvalitetni srafcigeri, samo bi mogao imati vise velicina.", CreatedAt = Ago(98) },
                new ProductReview { Id = 3, UserId = 5, ProductId = 3, Rating = 5, Comment = "Ostrica drzi dugo, odlicno za precizne radove.", CreatedAt = Ago(93) },
                new ProductReview { Id = 4, UserId = 2, ProductId = 4, Rating = 4, Comment = "Japanski kvalitet, sjajni rezovi. Malo skuplja ali vrijedi.", CreatedAt = Ago(78) },
                new ProductReview { Id = 5, UserId = 6, ProductId = 5, Rating = 5, Comment = "Prekrasna daska, bez ikakvih gresaka u drvetu.", CreatedAt = Ago(73) },
                new ProductReview { Id = 6, UserId = 7, ProductId = 6, Rating = 3, Comment = "Dobar furnir ali pakovanje moglo biti bolje.", CreatedAt = Ago(68) },
                new ProductReview { Id = 7, UserId = 3, ProductId = 7, Rating = 5, Comment = "Vrhunska koza, idealna za izradu torbi.", CreatedAt = Ago(63) },
                new ProductReview { Id = 8, UserId = 5, ProductId = 8, Rating = 4, Comment = "Dobar blok za rezbarenje, bez cvorova kao sto stoji u opisu.", CreatedAt = Ago(58) },
                new ProductReview { Id = 9, UserId = 4, ProductId = 9, Rating = 5, Comment = "Savrsen komplet za pocetnike! Sve sto trebate na jednom mjestu.", CreatedAt = Ago(47) },
                new ProductReview { Id = 10, UserId = 6, ProductId = 10, Rating = 4, Comment = "Dobar set, nozevi su ostri. Fali mozda bolji prirucnik.", CreatedAt = Ago(42) },
                new ProductReview { Id = 11, UserId = 2, ProductId = 11, Rating = 5, Comment = "Odlican pocetni set za rezbarenje, napravio sam prvu figuricu!", CreatedAt = Ago(37) },
                new ProductReview { Id = 12, UserId = 7, ProductId = 12, Rating = 4, Comment = "Kvalitetna glina, lako se modelira. Preporucujem pocetnicima.", CreatedAt = Ago(32) },
                new ProductReview { Id = 13, UserId = 3, ProductId = 13, Rating = 5, Comment = "Pregaca je jaka i funkcionalna, dzepovi su prakticni.", CreatedAt = Ago(27) },
                new ProductReview { Id = 14, UserId = 5, ProductId = 14, Rating = 4, Comment = "Stega dobro drzi, laka za upotrebu. Cijena odlicna.", CreatedAt = Ago(22) },
                new ProductReview { Id = 15, UserId = 4, ProductId = 15, Rating = 3, Comment = "Solidni brusni papiri, ali bi moglo biti vise finijih granulacija.", CreatedAt = Ago(20) },
                new ProductReview { Id = 16, UserId = 6, ProductId = 16, Rating = 5, Comment = "Lak se lijepo nanosi, bez tragova cetke. Odlican zavrsni sloj.", CreatedAt = Ago(18) },
                new ProductReview { Id = 17, UserId = 2, ProductId = 17, Rating = 4, Comment = "Prirodno ulje, drvo izgleda prekrasno nakon nanosenja.", CreatedAt = Ago(16) },
                new ProductReview { Id = 18, UserId = 7, ProductId = 18, Rating = 4, Comment = "Boje su zivopisne i dobro se upijaju u kozu.", CreatedAt = Ago(14) },
            };
            await InsertWithIdentityAsync(context, "ProductReviews", productReviews, 18);

            // ==================== PROJECTS (8) ====================
            var projects = new[]
            {
                new Project { Id = 1, Title = "Polica za knjige od hrasta", Description = "Rucno radjena polica od hrastovine sa 4 police. Dimenzije 120x80x25cm. Koristio sam stolarski komplet i hrastovu dasku iz Reignite ponude.", ImageUrl = "/uploads/seed/projects/project1.jpg", HoursSpent = 20, UserId = 3, HobbyId = 1, ProductId = 9, CreatedAt = Ago(82) },
                new Project { Id = 2, Title = "Rucno radjen novcanik", Description = "Novcanik od premium govedje koze sa rucnim savovima. 6 pretinaca za kartice i odjeljak za novcanice.", ImageUrl = "/uploads/seed/projects/project2.jpg", HoursSpent = 12, UserId = 4, HobbyId = 2, ProductId = 10, CreatedAt = Ago(73) },
                new Project { Id = 3, Title = "Drvena figura sove", Description = "Figurica sove rezbarena u bukovom drvetu, visina 25cm. Detaljan rad sa finom zavrsnom obradom.", ImageUrl = "/uploads/seed/projects/project3.jpg", HoursSpent = 8, UserId = 5, HobbyId = 6, ProductId = 11, CreatedAt = Ago(63) },
                new Project { Id = 4, Title = "Stolic za kafu rustikal", Description = "Rustikalni stolic za kafu od hrastovine sa metalnim nogama. Dimenzije 80x50x40cm. Najzahtjevniji projekt do sada!", ImageUrl = "/uploads/seed/projects/project4.jpg", HoursSpent = 30, UserId = 2, HobbyId = 1, ProductId = 5, CreatedAt = Ago(47) },
                new Project { Id = 5, Title = "Set keramickih solja", Description = "Set od 4 keramicke solje sa glazurom u toplim tonovima. Svaka solja je unikatna.", ImageUrl = "/uploads/seed/projects/project5.jpg", HoursSpent = 15, UserId = 6, HobbyId = 4, ProductId = 12, CreatedAt = Ago(37) },
                new Project { Id = 6, Title = "Kovani drzac za svijece", Description = "Drzac za 3 svijece od kovanog zeljeza. Rustikalni dizajn sa spiralnim detaljima.", ImageUrl = "/uploads/seed/projects/project6.jpg", HoursSpent = 10, UserId = 7, HobbyId = 3, ProductId = null, CreatedAt = Ago(27) },
                new Project { Id = 7, Title = "Ukrasna kutija od bukve", Description = "Rucno rezbarena kutija za nakit od bukovog drveta. Poklopac sa floralnim motivom.", ImageUrl = "/uploads/seed/projects/project7.jpg", HoursSpent = 18, UserId = 3, HobbyId = 6, ProductId = 8, CreatedAt = Ago(20) },
                new Project { Id = 8, Title = "Mirisne svijece od pcelinjeg voska", Description = "Set od 6 rucno radjenih svijeca od pcelinjeg voska sa esencijalnim uljima lavande i ruzmarina.", ImageUrl = "/uploads/seed/projects/project8.jpg", HoursSpent = 5, UserId = 5, HobbyId = 5, ProductId = null, CreatedAt = Ago(16) },
            };
            await InsertWithIdentityAsync(context, "Projects", projects, 8);

            // ==================== PROJECT REVIEWS (16) ====================
            var projectReviews = new[]
            {
                // Project 1 - Polica (Amir)
                new ProjectReview { Id = 1, UserId = 4, ProjectId = 1, Rating = 5, Comment = "Prekrasna polica! Hrastovina izgleda fenomenalno.", CreatedAt = Ago(78) },
                new ProjectReview { Id = 2, UserId = 5, ProjectId = 1, Rating = 4, Comment = "Odlican rad, vidi se iskustvo. Svaka cast!", CreatedAt = Ago(75) },
                // Project 2 - Novcanik (Lejla)
                new ProjectReview { Id = 3, UserId = 3, ProjectId = 2, Rating = 5, Comment = "Novcanik izgleda profesionalno! Savovi su savrseni.", CreatedAt = Ago(68) },
                new ProjectReview { Id = 4, UserId = 6, ProjectId = 2, Rating = 5, Comment = "Inspirativno! I ja zelim nauciti raditi sa kozom.", CreatedAt = Ago(65) },
                // Project 3 - Sova (Tarik)
                new ProjectReview { Id = 5, UserId = 2, ProjectId = 3, Rating = 4, Comment = "Detalji na sovi su nevjerovatni za samo 8 sati rada.", CreatedAt = Ago(58) },
                new ProjectReview { Id = 6, UserId = 7, ProjectId = 3, Rating = 5, Comment = "Kao iz trgovine! Talent za rezbarenje.", CreatedAt = Ago(55) },
                // Project 4 - Stolic (Test user)
                new ProjectReview { Id = 7, UserId = 3, ProjectId = 4, Rating = 5, Comment = "Stolic je brutalan! Rustikalni stil savrseno pogodjen.", CreatedAt = Ago(42) },
                new ProjectReview { Id = 8, UserId = 4, ProjectId = 4, Rating = 4, Comment = "Hrastovina i metal se odlicno kombiniraju. Bravo!", CreatedAt = Ago(40) },
                // Project 5 - Solje (Amina)
                new ProjectReview { Id = 9, UserId = 5, ProjectId = 5, Rating = 5, Comment = "Svaka solja je djelo za sebe. Glazura je prelijepa.", CreatedAt = Ago(32) },
                new ProjectReview { Id = 10, UserId = 2, ProjectId = 5, Rating = 4, Comment = "Topli tonovi glazure daju poseban sarm.", CreatedAt = Ago(30) },
                // Project 6 - Drzac (Kenan)
                new ProjectReview { Id = 11, UserId = 6, ProjectId = 6, Rating = 4, Comment = "Kovano zeljezo daje rustikalni ugodjaj. Lijepo!", CreatedAt = Ago(24) },
                new ProjectReview { Id = 12, UserId = 3, ProjectId = 6, Rating = 5, Comment = "Spiralni detalji su fenomenalni. Zelim naruciti!", CreatedAt = Ago(22) },
                // Project 7 - Kutija (Amir)
                new ProjectReview { Id = 13, UserId = 7, ProjectId = 7, Rating = 5, Comment = "Floralni motiv na poklopcu je zadivljujuci.", CreatedAt = Ago(18) },
                new ProjectReview { Id = 14, UserId = 4, ProjectId = 7, Rating = 4, Comment = "Amir uvijek odusevljava sa svojim projektima!", CreatedAt = Ago(16) },
                // Project 8 - Svijece (Tarik)
                new ProjectReview { Id = 15, UserId = 6, ProjectId = 8, Rating = 5, Comment = "Miris lavande je prelep! Zelim recept.", CreatedAt = Ago(14) },
                new ProjectReview { Id = 16, UserId = 2, ProjectId = 8, Rating = 4, Comment = "Pcelinji vosak daje posebnu toplinu svijecama.", CreatedAt = Ago(13) },
            };
            await InsertWithIdentityAsync(context, "ProjectReviews", projectReviews, 16);

            // ==================== WISHLISTS (5) ====================
            var wishlists = new[]
            {
                new Wishlist { Id = 1, UserId = 2, CreatedAt = SeedDate },
                new Wishlist { Id = 2, UserId = 3, CreatedAt = SeedDate },
                new Wishlist { Id = 3, UserId = 4, CreatedAt = SeedDate },
                new Wishlist { Id = 4, UserId = 5, CreatedAt = SeedDate },
                new Wishlist { Id = 5, UserId = 6, CreatedAt = SeedDate },
            };
            await InsertWithIdentityAsync(context, "Wishlist", wishlists, 5);

            // ==================== WISHLIST ITEMS (14) ====================
            var wishlistItems = new[]
            {
                // User 2 (Test): Komplet koza, Keramicarski kit, Japanska pila
                new WishlistItem { Id = 1, WishlistId = 1, ProductId = 10, Quantity = 1, UnitPrice = 189.00m, CreatedAt = SeedDate },
                new WishlistItem { Id = 2, WishlistId = 1, ProductId = 12, Quantity = 1, UnitPrice = 165.00m, CreatedAt = SeedDate },
                new WishlistItem { Id = 3, WishlistId = 1, ProductId = 4, Quantity = 1, UnitPrice = 67.90m, CreatedAt = SeedDate },
                // User 3 (Amir): Govedja koza, Lak, Boja za kozu
                new WishlistItem { Id = 4, WishlistId = 2, ProductId = 7, Quantity = 1, UnitPrice = 85.00m, CreatedAt = SeedDate },
                new WishlistItem { Id = 5, WishlistId = 2, ProductId = 16, Quantity = 1, UnitPrice = 24.90m, CreatedAt = SeedDate },
                new WishlistItem { Id = 6, WishlistId = 2, ProductId = 18, Quantity = 1, UnitPrice = 42.00m, CreatedAt = SeedDate },
                // User 4 (Lejla): Cekic, Hrastova daska, Stolarski komplet
                new WishlistItem { Id = 7, WishlistId = 3, ProductId = 1, Quantity = 1, UnitPrice = 45.00m, CreatedAt = SeedDate },
                new WishlistItem { Id = 8, WishlistId = 3, ProductId = 5, Quantity = 1, UnitPrice = 78.00m, CreatedAt = SeedDate },
                new WishlistItem { Id = 9, WishlistId = 3, ProductId = 9, Quantity = 1, UnitPrice = 149.99m, CreatedAt = SeedDate },
                // User 5 (Tarik): Orahov furnir, Pregaca
                new WishlistItem { Id = 10, WishlistId = 4, ProductId = 6, Quantity = 1, UnitPrice = 55.90m, CreatedAt = SeedDate },
                new WishlistItem { Id = 11, WishlistId = 4, ProductId = 13, Quantity = 1, UnitPrice = 65.00m, CreatedAt = SeedDate },
                // User 6 (Amina): Set rezbarenje, Stega, Ulje
                new WishlistItem { Id = 12, WishlistId = 5, ProductId = 11, Quantity = 1, UnitPrice = 79.90m, CreatedAt = SeedDate },
                new WishlistItem { Id = 13, WishlistId = 5, ProductId = 14, Quantity = 1, UnitPrice = 28.75m, CreatedAt = SeedDate },
                new WishlistItem { Id = 14, WishlistId = 5, ProductId = 17, Quantity = 1, UnitPrice = 18.50m, CreatedAt = SeedDate },
            };
            await InsertWithIdentityAsync(context, "WishlistItems", wishlistItems, 14);

            // ==================== COUPONS (4) ====================
            var coupons = new[]
            {
                new Coupon { Id = 1, Code = "DOBRODOSLI", DiscountType = "Percentage", DiscountValue = 10m, MinimumOrderAmount = 50m, ExpiryDate = FromNow(130), MaxUses = 100, TimesUsed = 12, IsActive = true, CreatedAt = SeedDate },
                new Coupon { Id = 2, Code = "SEZONA", DiscountType = "Percentage", DiscountValue = 15m, MinimumOrderAmount = 100m, ExpiryDate = FromNow(38), MaxUses = 50, TimesUsed = 8, IsActive = true, CreatedAt = SeedDate },
                new Coupon { Id = 3, Code = "PRVIPUT", DiscountType = "Fixed", DiscountValue = 20m, MinimumOrderAmount = 80m, ExpiryDate = null, MaxUses = 200, TimesUsed = 35, IsActive = true, CreatedAt = SeedDate },
                new Coupon { Id = 4, Code = "STARI2024", DiscountType = "Percentage", DiscountValue = 20m, MinimumOrderAmount = 50m, ExpiryDate = Ago(205), MaxUses = 100, TimesUsed = 67, IsActive = false, CreatedAt = Ago(296) },
            };
            await InsertWithIdentityAsync(context, "Coupons", coupons, 4);

            // ==================== FAQS (6) ====================
            var faqs = new[]
            {
                new Faq { Id = 1, Question = "Kako naruciti proizvod?", Answer = "Odaberite zeljeni proizvod, dodajte ga u kosaricu i pratite korake za narucivanje. Potrebno je imati korisnicki racun.", CreatedAt = SeedDate },
                new Faq { Id = 2, Question = "Koliko traje dostava?", Answer = "Dostava obicno traje 2-5 radnih dana na podrucju Bosne i Hercegovine. Za tacnije informacije pratite status vase narudzbe.", CreatedAt = SeedDate },
                new Faq { Id = 3, Question = "Mogu li vratiti proizvod?", Answer = "Da, imate pravo na povrat proizvoda u roku od 14 dana od primitka. Proizvod mora biti nekoristen i u originalnom pakovanju.", CreatedAt = SeedDate },
                new Faq { Id = 4, Question = "Kako koristiti kupon za popust?", Answer = "Na stranici za placanje unesite kod kupona u za to predvidjeno polje. Popust ce automatski biti primijenjen na vasu narudzbu.", CreatedAt = SeedDate },
                new Faq { Id = 5, Question = "Da li imate fizicku radnju?", Answer = "Trenutno poslujemo iskljucivo online. Svi proizvodi se salju postom sa podrucja Bosne i Hercegovine.", CreatedAt = SeedDate },
                new Faq { Id = 6, Question = "Kako mogu dijeliti svoje projekte?", Answer = "Na stranici 'Projekti' mozete dodati svoj projekt sa slikama i opisom. Potrebno je biti prijavljeni i imati odabran hobi na svom profilu.", CreatedAt = SeedDate },
            };
            await InsertWithIdentityAsync(context, "Faqs", faqs, 6);

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"[Seeder] GRESKA pri seedanju: {ex.Message}");
            if (ex.InnerException != null)
                Console.WriteLine($"[Seeder] Detalji: {ex.InnerException.Message}");
            throw;
        }
    }

    private static async Task InsertWithIdentityAsync<T>(
        ReigniteDbContext context, string tableName, IEnumerable<T> entities, int maxId) where T : class
    {
        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [" + tableName + "] ON");
        context.Set<T>().AddRange(entities);
        await context.SaveChangesAsync();
        await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [" + tableName + "] OFF");
        await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('[" + tableName + "]', RESEED, " + maxId + ")");
        context.ChangeTracker.Clear();
    }
}
