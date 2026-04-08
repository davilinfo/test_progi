# Car Auction Price Calculator

A full-stack car auction application where buyers can browse vehicles, participate in auctions, and calculate the total purchase price including all fees.

- **Backend**: .NET 10 Web API — vertical slice, MediatR, EF Core (in-memory), JWT auth
- **Frontend**: Vue.js 3 — Pinia, Vite, Fetch API
- **Database**: In-memory (resets on every restart; seeded with sample data)

---

## Prerequisites

| Tool | Minimum version |
|---|---|
| .NET SDK | 10.0 |
| Node.js | 18.x |
| Docker + Docker Compose | 24.x (optional) |

---

## Running Locally (development)

### 1. Start the backend

```bash
cd backend
dotnet restore
dotnet run --project src/CarAuction.Api
```

The API starts on:
- HTTP: `http://localhost:65501`
- HTTPS: `https://localhost:65500`
- Swagger UI: `http://localhost:65501/swagger`

### 2. Start the frontend

Open a second terminal:

```bash
cd frontend
npm install
npm run dev
```

Frontend available at: `http://localhost:5173`

> The Vite dev server proxies `/api` requests to `http://localhost:8080`. For local dev (not Docker), update `vite.config.js` proxy target to `http://localhost:65501`.

---

## Running with Docker

```bash
docker compose up --build
```

| Service | URL |
|---|---|
| Frontend | http://localhost:3000 |
| API | http://localhost:8080 |
| Swagger UI | http://localhost:8080/swagger |

---

## Running Tests

```bash
cd backend
dotnet test
```

Expected output: **24 passed, 0 failed**.

Run a specific test class:

```bash
dotnet test --filter "FullyQualifiedName~FeeCalculationTests"
```

---

## Seed Users

The application is pre-loaded with the following accounts on every startup. All users share the same password:

**Password: `Password123!`**

| Email | Role | Description |
|---|---|---|
| `admin@carauction.com` | Admin | Full access to all endpoints |
| `seller1@carauction.com` | Seller | AutoDeal Inc. — owns vehicles 1, 3, 5 |
| `seller2@carauction.com` | Seller | Premium Cars LLC — owns vehicles 2, 4 |
| `buyer1@carauction.com` | Buyer | Alice Johnson (age 34) |
| `buyer2@carauction.com` | Buyer | Bob Williams (age 42) |

---

## Seed Auctions

Three active auctions are ready to bid on immediately:

| Auction | Vehicle | Base Price | Status |
|---|---|---|---|
| Auction 1 | Toyota Corolla 2019 (Common) | $8,000 | **Active** |
| Auction 2 | BMW 7 Series 2022 (Luxury) | $45,000 | **Active** |
| Auction 3 | Honda Civic 2020 (Common) | $6,500 | **Active** |
| Auction 4 | Mercedes S-Class 2021 (Luxury) | $60,000 | Inactive (ended) |
| Auction 5 | Ford Focus 2018 (Common) | $4,000 | Inactive (ended) |

---

## How to Participate in an Auction

### Using the Web Interface

1. Open `http://localhost:5173` (or `http://localhost:3000` for Docker).
2. Click **Login** and sign in as a buyer (e.g. `buyer1@carauction.com` / `Password123!`).
3. Navigate to **Auctions** to see all active auctions.
4. Click an auction to view vehicle details and the current price.
5. Enter your bid amount and click **Place Bid**. The new price is reflected immediately.
6. Use the **Fee Calculator** tab to see a breakdown of all fees before bidding.

### Using Swagger UI

1. Open `http://localhost:65501/swagger` (or `http://localhost:8080/swagger` for Docker).

2. **Authenticate** — expand `POST /api/auth/login`, click **Try it out**, and submit:
   ```json
   {
     "email": "buyer1@carauction.com",
     "password": "Password123!"
   }
   ```
   Copy the `token` value from the response.

3. Click the **Authorize** button (top right), paste `Bearer <your-token>`, and confirm.

4. **Browse auctions** — `GET /api/auctions` returns all auctions with vehicle and fee details.

5. **Place a bid** — `POST /api/auctions/{id}/bid`:
   ```json
   {
     "price": 9000
   }
   ```
   Use one of the active auction IDs from step 4.

6. **Check fees** — `GET /api/fees/calculate?basePrice=9000&vehicleType=Common` returns the full fee breakdown.

### Seller workflow (using Swagger)

1. Login as `seller1@carauction.com` and authorize.
2. **Create a vehicle** — `POST /api/vehicles`:
   ```json
   {
     "plate": "NEW-999",
     "type": "Common",
     "year": 2023,
     "model": "VW Golf"
   }
   ```
3. **Create an auction** — `POST /api/auctions`:
   ```json
   {
     "vehicleId": "<vehicle-id-from-step-2>",
     "basePrice": 12000,
     "endDate": "2026-12-31T00:00:00Z"
   }
   ```
4. **Close the auction** — `POST /api/auctions/{id}/close` when bidding is complete.

---

## Fee Calculation

All fees are computed from `basePrice` and `vehicleType`:

| Fee | Common | Luxury |
|---|---|---|
| Basic buyer fee | 10% · price, min $10, max $50 | 10% · price, min $25, max $200 |
| Special seller fee | 2% · price | 4% · price |
| Association fee | $5 (≤$500) / $10 (≤$1,000) / $15 (≤$3,000) / $20 (>$3,000) | same |
| Storage fee | $100 (fixed) | $100 (fixed) |

**Example — Common vehicle at $8,000:**

| Fee | Amount |
|---|---|
| Basic buyer fee | $50.00 (capped) |
| Special seller fee | $160.00 |
| Association fee | $20.00 |
| Storage fee | $100.00 |
| **Total** | **$8,330.00** |

The fee calculator on the frontend updates live as you type — no form submission needed.

---

## Project Structure

```
progi/
├── backend/
│   ├── src/CarAuction.Api/
│   │   ├── Controllers/          ← thin controllers (auth + dispatch)
│   │   ├── Features/             ← vertical slices (Vehicle, Auction, Buyer, Seller, Fee, Auth)
│   │   └── Shared/               ← entities, DTOs, services, EF context, seeder
│   └── tests/CarAuction.Tests/   ← xUnit + Moq + FluentAssertions
├── frontend/
│   └── src/
│       ├── views/                ← HomeView, LoginView, AuctionsView, CalculatorView
│       ├── stores/               ← Pinia stores (auth, auction, vehicle, fee)
│       └── components/           ← NavBar, FeeCalculator, AuctionCard
├── docker-compose.yml
└── CLAUDE.md
```

<img width="1359" height="713" alt="image" src="https://github.com/user-attachments/assets/f509d179-304f-41d4-8e10-1ec81ed0ad6f" />
<img width="1353" height="711" alt="image" src="https://github.com/user-attachments/assets/3418c210-e307-4885-b24f-0c66cd2bfcf8" />
<img width="1360" height="711" alt="image" src="https://github.com/user-attachments/assets/539317d5-b836-42c3-a7e1-66f4218052a1" />
<img width="1363" height="718" alt="image" src="https://github.com/user-attachments/assets/87282c1d-6710-4f23-99c0-15b7b4e53d64" />
<img width="1362" height="713" alt="image" src="https://github.com/user-attachments/assets/132a0fd3-6765-480a-aba9-33282f8256c8" />
<img width="1365" height="723" alt="image" src="https://github.com/user-attachments/assets/9d58369e-2a16-4e3b-9f7e-4f1a22927c2c" />
<img width="1361" height="716" alt="image" src="https://github.com/user-attachments/assets/ad23e811-0962-43f8-842a-a0da70113a4a" />






