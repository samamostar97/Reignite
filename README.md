# Reignite

Web aplikacija za prodaju alata i materijala za ručne radove sa platformom za dijeljenje projekata.

## Tehnologije

**Backend:**
- .NET 8 Web API
- Entity Framework Core (Code-First)
- SQL Server
- Stripe (plaćanje karticama)
- QuestPDF (PDF izvještaji)
- BCrypt (hashiranje lozinki)
- JWT autentifikacija sa refresh tokenima

**Frontend:**
- Angular 21
- Tailwind CSS
- Stripe Elements

## Arhitektura

Projekat koristi Clean Architecture sa slojevima:

- **Core** - Domenske entitete i enumeracije. Nema zavisnosti prema drugim projektima.
- **Application** - Interfejse servisa, DTO objekte, filtere i validaciju. Zavisi samo od Core sloja.
- **Infrastructure** - Implementacije servisa, pristup bazi, EF Core konfiguraciju i eksterne integracije (Stripe, QuestPDF). Zavisi od Core i Application slojeva.
- **API** - Kontrolere, middleware, autentifikaciju i konfiguraciju. Zavisi od svih slojeva.

Koristi se generički BaseService/BaseController pattern za CRUD operacije.

## Pokretanje projekta

### Preduvjeti

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express ili Developer Edition)
- [Node.js](https://nodejs.org/) (v18+) i npm
- [Stripe test ključevi](https://dashboard.stripe.com/test/apikeys) (besplatan račun)

### 1. Kloniraj projekat

```bash
git clone <repo-url>
cd Reignite
```

### 2. Provjeri konfiguraciju (.env)

`.env` fajl se nalazi u `Reignite/Reignite.API/.env` i dolazi sa podrazumjevanim vrijednostima (JWT, Stripe test ključevi). Jedino što je možda potrebno prilagoditi je konekcija na bazu:

```env
DB_SERVER=.
DB_PORT=1433
DB_NAME=ReigniteDb
DB_USER=sa
DB_PASSWORD=YourStrong@Passw0rd123
```

> **Napomena:** `DB_SERVER=.` koristi lokalni SQL Server. Prilagodi `DB_PASSWORD` svojoj SQL Server lozinci ako je drugačija.

### 3. Pokreni backend

```bash
cd Reignite/Reignite.API
dotnet run
```

Pri prvom pokretanju:
- Automatski se primjenjuju sve EF Core migracije (ne treba ručno)
- DatabaseSeeder popunjava bazu sa test podacima
- Preuzimaju se slike sa interneta (potrebna internet konekcija)

Backend će biti dostupan na: **http://localhost:5262**

Swagger UI: **http://localhost:5262/swagger**

### 4. Pokreni frontend

```bash
cd client
npm install
npm start
```

Frontend će biti dostupan na: **http://localhost:4200**

## Testni korisnici

Nakon seedinga, u konzoli će se ispisati kredencijali. Podrazumjevani su:

| Uloga | Email | Lozinka |
|-------|-------|---------|
| Admin | admin@reignite.ba | test |
| User | test@reignite.ba | test |
| User | amir@mail.com | test123 |
| User | lejla@mail.com | test123 |
| User | tarik@mail.com | test123 |
| User | amina@mail.com | test123 |
| User | kenan@mail.com | test123 |

## Stripe testno plaćanje

Za testiranje checkout-a koristi Stripe test karticu:

| Polje | Vrijednost |
|-------|-----------|
| Broj kartice | 4242 4242 4242 4242 |
| Datum isteka | Bilo koji budući datum (npr. 12/30) |
| CVC | Bilo koja 3 cifre (npr. 123) |

## Glavne funkcionalnosti

**Javni dio:**
- Pregled i pretraživanje proizvoda sa filtriranjem po kategoriji, cijeni i ocjeni
- Detalji proizvoda sa recenzijama
- Galerija korisničkih projekata
- FAQ stranica
- Registracija i prijava

**Korisnički dio:**
- Upravljanje profilom, hobijima i adresama
- Lista želja (wishlist)
- Korpa sa kuponima za popust
- Checkout sa Stripe plaćanjem karticom
- Historija narudžbi sa statusima
- Kreiranje i dijeljenje projekata
- Recenzije proizvoda i projekata

**Admin panel:**
- Dashboard sa statistikama i grafovima
- CRUD za sve entitete (proizvodi, kategorije, dobavljači, narudžbe, korisnici, projekti, hobiji, kuponi, FAQ)
- Upravljanje statusom narudžbi
- Pregled recenzija proizvoda i projekata
- Izvještaji u JSON i PDF formatu (narudžbe, prihodi)

## Baza podataka

Glavne tabele:
- **Users** - korisnici sa ulogama (Admin/AppUser)
- **Products** - proizvodi sa kategorijama i dobavljačima
- **Orders + OrderItems** - narudžbe sa Stripe plaćanjem i kupon popustima
- **Projects** - korisnički projekti povezani sa hobijima
- **ProductReviews + ProjectReviews** - recenzije sa ocjenama (1-5)
- **Coupons** - kuponi za popust (procenat ili fiksni iznos)
- **Wishlist + WishlistItems** - lista želja
- **UserAddresses** - adrese za dostavu
- **Hobbies + UserHobbies** - hobiji korisnika
- **Faqs** - često postavljana pitanja

## Struktura projekta

```
Reignite/
├── Reignite/                    # Backend (.NET 8)
│   ├── Reignite.API/            # Kontroleri, middleware, konfiguracija
│   ├── Reignite.Application/    # DTO-ovi, interfejsi servisa, filteri
│   ├── Reignite.Infrastructure/ # Implementacije servisa, EF Core, migracije
│   └── Reignite.Core/           # Entiteti, enumeracije
├── client/                      # Frontend (Angular 21)
│   └── src/app/
│       ├── core/                # Servisi, modeli, guards, interceptori
│       ├── features/            # Stranice (shop, cart, checkout, admin...)
│       └── shared/              # Komponente, utils
└── README.md
```
