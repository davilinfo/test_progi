# Playwright E2E Tests

This folder contains end-to-end tests for the CarAuction API using Microsoft Playwright.

## Setup

### 1. Install Playwright Browser Binaries

Run the following command from the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tests\CarAuction.Tests\bin\Debug\net10.0\playwright.ps1 install
```

> **Note:** Use `powershell` (Windows PowerShell), not `pwsh` (PowerShell 7+). The script will download Chrome, Firefox, WebKit, and other dependencies (~500MB).

### 2. Build the Test Project

```bash
dotnet build tests\CarAuction.Tests\CarAuction.Tests.csproj
```

## Running Tests

Ensure the CarAuction API is running on `http://localhost:5000`, then run:

```bash
dotnet test tests\CarAuction.Tests\CarAuction.Tests.csproj --filter "Playwright"
```

## Test Coverage

- **HomePage_Should_Return_Correct_Title**: Verifies the API homepage loads and contains "CarAuction"
- **AuctionsApi_Should_Return_Success_Status**: Validates the `/api/auctions` endpoint returns 200 OK with JSON headers

## Dependencies

- `Microsoft.Playwright` v1.60.0
- `.NET 10.0`
- Windows PowerShell 5.1+ (for `playwright.ps1 install`)
