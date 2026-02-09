# Reignite

Web aplikacija za prodaju alata i materijala za ručne radove sa platformom za dijeljenje projekata.

## Tehnologije

**Backend:**
- .NET 8 Web API
- Entity Framework Core
- SQL Server
- Stripe (plaćanje)
- QuestPDF (izvještaji)

**Frontend:**
- Angular 21
- Tailwind CSS
- Stripe Elements

## Arhitektura

Projekat koristi Clean Architecture sa slojevima:

**Core** - Domenske entitete i enumeracije. Nema zavisnosti prema drugim projektima.

**Application** - Poslovnu logiku, interfejse servisa, DTO objekte, filtere i validaciju. Zavisi samo od Core sloja.

**Infrastructure** - Konkretne implementacije servisa, pristup bazi podataka, EF Core konfiguraciju i externe integracije. Zavisi od Core i Application slojeva.

**API** - Kontrolere, middleware, autentifikaciju i konfiguracijske fajlove. Zavisi od svih slojeva.

Koristi se Repository pattern i BaseService/BaseController za CRUD operacije sa generičkim tipovima.

## Baza podataka

Aplikacija koristi SQL Server sa Code-First pristupom i EF Core migracijama.

**Glavne tabele:**
- Users (korisnici sa ulogama Admin/User)
- Products (proizvodi sa kategorijama i dobavljačima)
- Orders + OrderItems (narudžbe i stavke)
- Projects (projekti korisnika)
- ProductReviews + ProjectReviews (recenzije)
- Coupons (kuponi za popust)
- Hobbies (hobiji povezani sa korisnicima i projektima)
- Addresses (adrese korisnika za dostavu)
- Faqs (često postavljana pitanja)

Seeding podataka se vrši kroz DatabaseSeeder klasu prilikom pokretanja aplikacije.

## Pokretanje projekta

**Preduvjeti:**
- .NET 8 SDK
- SQL Server
- Node.js i npm
- Angular CLI

**Backend setup:**

1. Kreiraj bazu podataka u SQL Serveru

2. Kopiraj appsettings.json u appsettings.Development.json i postavi connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ReigniteDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

3. Dodaj Stripe i JWT konfiguraciju u appsettings.Development.json:

```json
{
  "Stripe": {
    "PublishableKey": "pk_test_...",
    "SecretKey": "sk_test_..."
  },
  "Jwt": {
    "Key": "tvoj-secret-key-ovdje-minimum-32-karaktera",
    "Issuer": "ReigniteAPI",
    "Audience": "ReigniteClient",
    "ExpiryMinutes": 60
  }
}
```

4. Pokreni migracije:

```bash
cd Reignite/Reignite.API
dotnet ef database update --project ../Reignite.Infrastructure
```

5. Pokreni API:

```bash
dotnet run
```

API će biti dostupan na http://localhost:5000

**Frontend setup:**

1. Instaliraj dependencies:

```bash
cd client
npm install
```

2. Kreiraj environment.development.ts fajl sa API URL-om:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000/api',
  stripePublishableKey: 'pk_test_...'
};
```

3. Pokreni Angular aplikaciju:

```bash
ng serve
```

Aplikacija će biti dostupna na http://localhost:4200

## Testni korisnici

Nakon seedinga biće dostupni sljedeći korisnici:

**Admin:**
- Email: admin@reignite.ba
- Password: Admin123!

**Korisnici:**
- Email: amir.kovac@example.ba / Password: User123!
- Email: lejla.hodzic@example.ba / Password: User123!
- Email: tarik.mehic@example.ba / Password: User123!

## Glavne funkcionalnosti

**Javni dio:**
- Pregled proizvoda sa filtriranjem i pretraživanjem
- Detalji proizvoda i recenzije
- Galerija projekata korisnika
- Registracija i prijava

**Korisnički dio:**
- Upravljanje profilom i adresama
- Lista želja (wishlist)
- Korpa i checkout proces sa Stripe plaćanjem
- Historija narudžbi
- Kreiranje i dijeljenje projekata
- Recenzije proizvoda i projekata

**Admin panel:**
- Dashboard sa statistikama
- CRUD za sve entitete (proizvodi, narudžbe, korisnici, itd.)
- Upravljanje statusom narudžbi
- Izvještaji (JSON i PDF format)
- Upravljanje kuponima i FAQ-ovima
