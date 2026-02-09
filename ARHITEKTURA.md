# Arhitektura Reignite projekta

## Organizacija slojeva

Projekat je organizovan prema Clean Architecture principima sa jasnom podjelom odgovornosti.

### Core (Domain)

Sadrži domenske entitete i enumeracije bez zavisnosti prema drugim projektima.

**Entiteti:**
- Svi nasljeđuju BaseEntity (Id, CreatedAt, UpdatedAt, IsDeleted za soft delete)
- User, Product, Order, OrderItem, Project, ProductReview, ProjectReview
- ProductCategory, Supplier, Hobby, Address, Coupon, Faq, Activity

**Enumeracije:**
- UserRole (Admin, User)
- OrderStatus (Pending, Processing, OnDelivery, Completed, Cancelled)
- DiscountType (Percentage, FixedAmount)
- AddressType (Home, Work, Other)

### Application

Poslovnu logiku i apstrakcije.

**IServices interfejsi:**
- Generički IService sa CRUD metodama
- Specifični interfejsi (IAuthService, IPaymentService, IReportService, itd.)

**DTOs:**
- Request objekti za kreiranje i ažuriranje (CreateProductRequest, UpdateUserRequest)
- Response objekti za vraćanje podataka (ProductResponse, UserResponse)
- Query filteri za pretragu i filtriranje (ProductQueryFilter, OrderQueryFilter)

**Helpers:**
- PaginationRequest i PagedResult za paging
- ValidationHelper za Data Annotations validaciju

### Infrastructure

Implementacija servisa i pristup podacima.

**Services:**
- BaseService - generička CRUD implementacija sa lifecycle hook metodama
- Konkretni servisi nasljeđuju BaseService i dodaju specifične metode
- AuthService - registracija, prijava, generiranje JWT tokena
- PaymentService - integracija sa Stripe API-jem
- PdfReportService - generiranje PDF izvještaja sa QuestPDF-om
- FileStorageService - upload i brisanje slika

**Data:**
- ReigniteDbContext - EF Core DbContext sa DbSet-ovima
- Configurations - Fluent API konfiguracija za svaki entitet
- DatabaseSeeder - seed podataka pri pokretanju aplikacije
- Migracije

**Repositories:**
- BaseRepository - generička CRUD implementacija
- IRepository interfejs

### API

Ulazna tačka aplikacije.

**Controllers:**
- BaseController - generički CRUD endpointi
- Konkretni kontroleri nasljeđuju BaseController i dodaju custom endpointe
- AuthController - registracija, prijava, refresh token
- PaymentController - Stripe konfiguracija i kreiranje PaymentIntent-a
- ReportController - JSON i PDF izvještaji

**Middleware:**
- ExceptionMiddleware - globalno hvatanje grešaka

**Extensions:**
- ServiceExtensions - registracija servisa u DI kontejneru

**Konfiguracija:**
- appsettings.json - osnovna konfiguracija
- appsettings.Development.json - development postavke (connection string, Stripe, JWT)

## Tokovi podataka

### Tipičan HTTP zahtjev

1. Zahtjev dolazi na Controller
2. Controller poziva odgovarajući Service
3. Service poziva Repository za pristup bazi
4. Repository koristi DbContext za EF Core operacije
5. Podaci se mapiraju u DTO i vraćaju kroz slojeve nazad do korisnika

### Autentifikacija

1. Korisnik šalje credentials na AuthController
2. AuthService validira credentials i provjerava ulogu
3. Generiše se JWT token i Refresh token
4. Tokeni se vraćaju klijentu
5. Klijent šalje JWT u Authorization header-u za zaštićene rute
6. JwtMiddleware validira token i postavlja User u HttpContext

### Plaćanje

1. Korisnik inicira checkout proces
2. Frontend dobija Stripe publishable key sa PaymentController-a
3. Frontend poziva PaymentController za kreiranje PaymentIntent-a
4. PaymentService komunicira sa Stripe API-jem
5. Vraća se client_secret koji frontend koristi sa Stripe Elements
6. Nakon potvrde plaćanja, OrderService kreira narudžbu

## Ključni design pattern-i

**Repository Pattern** - Apstrakcija pristupa podacima preko IRepository interfejsa.

**Dependency Injection** - Svi servisi i repositories se registruju u DI kontejneru.

**DTO Pattern** - Odvajanje domenskih entiteta od API kontrakta.

**Generic Base Classes** - BaseService i BaseController smanjuju dupliciranje koda.

**Service Layer Pattern** - Poslovna logika je odvojena od kontrolera.

**Configuration Pattern** - Fluent API konfiguracija za EF Core entitete.

**Soft Delete** - IsDeleted flag umjesto fizičkog brisanja.

**Async/Await sa CancellationToken** - Svi async metodi primaju CancellationToken za omogućavanje otkazivanja operacija.

## Sigurnost

**Autentifikacija:**
- JWT tokeni sa refresh token mehanizmom
- Password hashing sa SecurePasswordHasher

**Autorizacija:**
- Role-based autorizacija (Admin, User)
- [Authorize] atributi na kontrolerima i akcijama

**Validacija:**
- Data Annotations na DTO objektima
- Manualna validacija sa ValidationHelper-om
- Fluent API constraints u bazi

**CORS:**
- Konfigurisano za Angular aplikaciju

**SQL Injection:**
- Parametrizovani upiti kroz EF Core
- DatabaseSeeder koristi ExecuteSqlRaw ali sa kontrolisanim stringovima
