# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with this repository.

## Project Overview

Car auction price calculator — a full-stack application allowing buyers to compute the total purchase price of a vehicle (Common or Luxury) at auction, including all dynamically-computed fees.

- **Backend**: .NET 10 Web API (vertical slice, MediatR, EF Core in-memory, JWT auth, Docker)
- **Frontend**: Vue.js 3 + Pinia + Fetch API

---

## Commands

### Backend

```bash
# Restore and build
cd backend
dotnet restore
dotnet build

# Run (development) — listens on http://localhost:65501 and https://localhost:65500
dotnet run --project src/CarAuction.Api

# Run all tests (24 tests, all passing)
dotnet test

# Run a single test class
dotnet test --filter "FullyQualifiedName~FeeCalculationTests"

# Run via Docker (API on :8080, frontend on :3000)
docker compose up --build
```

### Frontend

```bash
cd frontend
npm install
npm run dev       # Vite dev server on http://localhost:5173
npm run build     # Production build
npm run preview   # Preview production build
```

---

## Architecture

### Backend — Vertical Slice

Each feature owns its full stack (request → validation → handler → repository → response). Do not create horizontal layers (Services/, Controllers/ monoliths).

```
src/CarAuction.Api/
├── Controllers/                 ← thin controllers, auth + dispatch only
│   ├── AuthController.cs
│   ├── AuctionsController.cs
│   ├── BuyersController.cs
│   ├── SellersController.cs
│   ├── VehiclesController.cs
│   └── FeesController.cs
├── Features/
│   ├── Vehicle/
│   │   ├── Repository/
│   │   │   ├── IVehicleRepository.cs
│   │   │   └── VehicleRepository.cs
│   │   ├── Create/
│   │   │   ├── CreateVehicleCommand.cs
│   │   │   ├── Handler.cs
│   │   │   └── Validator.cs
│   │   ├── Update/, Delete/, GetById/, GetAll/
│   ├── Auction/    (Create, PlaceBid, Close, GetById, GetAll)
│   ├── Buyer/      (Create, Update, Delete, GetById, GetAll)
│   ├── Seller/     (Create, Update, Delete, GetById, GetAll)
│   ├── Auth/       (Login)
│   └── Fee/
│       └── Calculate/           ← core fee computation lives here
└── Shared/
    ├── Model/                   ← DTOs / response models
    ├── Entity/                  ← EF Core entities
    ├── Enums/
    ├── Exceptions/
    ├── Services/
    │   └── FeeCalculationService.cs
    └── Infrastructure/
        ├── AppDbContext.cs
        ├── DataSeeder.cs
        ├── LoggingBehavior.cs
        └── ValidationBehavior.cs
```

**Controllers are thin** — they only validate auth, dispatch the MediatR command/query, and return the result. No business logic in controllers.

**Handlers** contain business logic. Every handler gets a logger injected; log on entry, validation failures, and domain exceptions.

### Key design patterns

| Pattern | Usage |
|---|---|
| MediatR 12.4.1 | All commands and queries dispatched via `IMediator.Send()` |
| Repository | Each aggregate (Vehicle, Auction, Buyer, Seller) has its own `IXRepository` / `XRepository` pair |
| Fluent Validation 11.3.0 | Each command/query has a paired `Validator` registered via `AddValidatorsFromAssembly` |
| Result / problem details | Return `IActionResult` with correct HTTP verbs and status codes; use `ProblemDetails` for errors |

### Database (EF Core in-memory)

- One `AppDbContext` per HTTP request (registered `Scoped`).
- No lazy loading — always `.Include()` explicitly.
- Seed on startup: fee tables (BuyerFee, SellerFee, AssociationFee, StorageFee), 2 sellers, 2 buyers, 5 vehicles, 5 auctions (3 active / 2 inactive), 5 users.
- **Data resets on every restart** (in-memory database).

### Security

- JWT Bearer authentication; roles: `Buyer`, `Seller`, `Admin`.
- Seller: CRUD own seller data; create/update vehicles (only when auction is inactive or none); initiate auctions.
- Buyer: read vehicles; set auction bid prices; register own buyer data.
- Admin: full access to all endpoints.
- CORS allows any origin (`AllowAnyOrigin`).
- Password hashing: SHA-256 with salt `CarAuctionSalt2024` (use bcrypt/Argon2 in production).

### Known implementation notes

- **Namespace shadowing fix**: Feature folder names (`Features.Vehicle`, `Features.Buyer`, etc.) shadow entity type names imported via `global using CarAuction.Api.Shared.Entity;`. All affected repository and handler files use per-file type aliases:
  ```csharp
  using Vehicle = CarAuction.Api.Shared.Entity.Vehicle;
  ```
- **Swagger schema collision fix**: Two records named `VehicleResponse` exist in different feature namespaces. `Program.cs` uses `CustomSchemaIds` to resolve this:
  ```csharp
  options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
  ```
- **Package conflict**: `Microsoft.AspNetCore.OpenApi` and `Swashbuckle.AspNetCore 7.2.0` have conflicting transitive `Microsoft.OpenApi` dependencies. Only Swashbuckle is included in the `.csproj` — do not re-add `Microsoft.AspNetCore.OpenApi`.

---

## NuGet Packages (backend)

| Package | Version |
|---|---|
| MediatR | 12.4.1 |
| FluentValidation.DependencyInjectionExtensions | 11.3.0 |
| Microsoft.EntityFrameworkCore.InMemory | 10.0.0 |
| Microsoft.AspNetCore.Authentication.JwtBearer | 10.0.0 |
| Swashbuckle.AspNetCore | 7.2.0 |
| xUnit | (via Microsoft.NET.Test.Sdk) |
| Moq | latest |
| FluentAssertions | latest |

> Do NOT add `Microsoft.AspNetCore.OpenApi` — it conflicts with Swashbuckle.

---

## Domain — Fee Calculation

All fees are calculated against `basePrice` and `vehicleType`.

| Fee | Common | Luxury |
|---|---|---|
| Basic buyer fee | 10% · price, min $10, max $50 | 10% · price, min $25, max $200 |
| Special seller fee | 2% · price | 4% · price |
| Association fee | $5 (1–500) / $10 (501–1000) / $15 (1001–3000) / $20 (>3000) | same brackets |
| Storage fee | $100 (fixed) | $100 (fixed) |

**Verification test cases** (unit tests must match these exactly):

| Base Price | Type | Basic | Special | Association | Storage | Total |
|---|---|---|---|---|---|---|
| $398.00 | Common | $39.80 | $7.96 | $5.00 | $100 | $550.76 |
| $50.00 | Common | $10.00 | $1.00 | $5.00 | $100 | $166.00 |
| $1000.00 | Common | $50.00 | $20.00 | $10.00 | $100 | $1180.00 |
| $1800.00 | Luxury | $180.00 | $72.00 | $15.00 | $100 | $2167.00 |

Fee rules are **seeded into the database** and read dynamically at calculation time — hardcoding fee percentages in application code is not allowed.

---

## Entities

```
vehicle         (id, plate, type[Common|Luxury], year, model, photo)
buyer           (id, name, age, phone, email)
seller          (id, name, phone, email)
auction         (id, buyer_id, seller_id, vehicle_id, base_price, price,
                 status[active|inactive], start_date, end_date)
user            (id, email, password_hash, role[Admin|Seller|Buyer], seller_id?, buyer_id?)
buyer_fee       (id, fee_percentage, fee_common_min, fee_common_max, fee_luxury_min, fee_luxury_max)
seller_fee      (id, fee_common, fee_luxury)
association_fee (id, fee, min_range, max_range)
storage_fee     (id, fee)
```

---

## API Endpoints

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/login` | None | Returns JWT token |
| GET | `/api/fees/calculate` | None | Calculate fees for a base price + vehicle type |
| GET | `/api/vehicles` | None | List all vehicles |
| GET | `/api/vehicles/{id}` | None | Get vehicle by ID |
| POST | `/api/vehicles` | Seller/Admin | Create vehicle |
| PUT | `/api/vehicles/{id}` | Seller/Admin | Update vehicle |
| DELETE | `/api/vehicles/{id}` | Seller/Admin | Delete vehicle |
| GET | `/api/auctions` | None | List all auctions |
| GET | `/api/auctions/{id}` | None | Get auction by ID |
| POST | `/api/auctions` | Seller/Admin | Create auction |
| POST | `/api/auctions/{id}/bid` | Buyer/Admin | Place a bid |
| POST | `/api/auctions/{id}/close` | Seller/Admin | Close auction |
| GET | `/api/buyers` | Admin | List all buyers |
| GET | `/api/buyers/{id}` | Buyer/Admin | Get buyer by ID |
| POST | `/api/buyers` | Buyer/Admin | Create buyer |
| PUT | `/api/buyers/{id}` | Buyer/Admin | Update buyer |
| DELETE | `/api/buyers/{id}` | Buyer/Admin | Delete buyer |
| GET | `/api/sellers` | Admin | List all sellers |
| GET | `/api/sellers/{id}` | Seller/Admin | Get seller by ID |
| POST | `/api/sellers` | Seller/Admin | Create seller |
| PUT | `/api/sellers/{id}` | Seller/Admin | Update seller |
| DELETE | `/api/sellers/{id}` | Seller/Admin | Delete seller |

---

## API Conventions

- Routes follow REST conventions: `GET /api/vehicles`, `POST /api/vehicles`, `GET /api/vehicles/{id}`, etc.
- All endpoints return a consistent response envelope or `ProblemDetails` on error.
- Use correct status codes: `200 OK`, `201 Created` (with `Location` header), `204 No Content`, `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`, `404 Not Found`.
- Swagger/OpenAPI enabled at `/swagger`; all endpoints documented and callable from the Swagger UI.

---

## Frontend

- Vue 3 Composition API (`<script setup>`).
- Pinia store per feature domain (`useAuctionStore`, `useVehicleStore`, `useAuthStore`, `useFeeStore`).
- All HTTP calls go through store **actions** using the native `fetch` API — no axios.
- The fee calculator view recomputes totals reactively on every change to base price or vehicle type (400 ms debounce, no submit button required).
- Vite dev server proxies `/api` → `http://localhost:8080` (Docker) or configure `VITE_API_BASE_URL` for local dev.
- No SSR; plain Vite SPA.

---

## Unit Tests

Framework: **xUnit + Moq + FluentAssertions**, pattern: **Arrange / Act / Assert**.

Test project: `backend/tests/CarAuction.Tests/`

Test projects mirror the feature structure. Each handler, validator, and fee calculator has dedicated tests. Repositories are mocked with Moq (`Mock<IVehicleRepository>`).

Focus areas:
- `FeeCalculationService` / handler — covers all four verification test cases above.
- Validator tests — both valid and invalid inputs.
- Handler tests — happy path + edge cases (not found, unauthorized).

Current status: **24 / 24 tests passing**.

---

## Docker

- `Dockerfile` for the .NET API (`mcr.microsoft.com/dotnet/aspnet:10.0` runtime, `sdk:10.0` build stage).
- `docker-compose.yml` at repo root brings up:
  - `carauction-api` → port `8080`
  - `carauction-frontend` (Nginx) → port `3000`
- Environment variable `ASPNETCORE_ENVIRONMENT=Development` enables Swagger in the container.
- JWT secret set via `JwtSettings__SecretKey` env var in docker-compose.
