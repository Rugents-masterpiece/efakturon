# Efakturon

Multi-tenant SaaS for e-invoicing (Peppol BIS 3.0, UBL, QR payments) built on **ASP.NET Core 8+**, **MudBlazor**, **EF Core**, **PostgreSQL**. Clean Architecture with Domain-Driven Design.

## Features

- **Multi-tenancy**: Row-level security via global query filters, tenant resolution from claims
- **Invoice Management**: Create, edit, issue, cancel, pay, PDF/HTML/UBL export
- **Peppol BIS 3.0**: Native UBL/XML generation, validation-ready
- **QR Payments**: Pay by Square, PayMe 2.0, EET (Slovak fiscal)
- **Stock & Warehouse**: Products, warehouses, stock movements, reservations
- **Auth**: ASP.NET Core Identity + roles (SuperAdmin, TenantAdmin, Accountant, User)
- **Audit Log**: Full entity change tracking
- **Super Admin Panel**: Tenant management, impersonation
- **Tenant Admin Panel**: Users, invitations, settings

## Architecture (Clean + DDD)

```
src/
├── Efakturon.Domain          # Entities, Value Objects, Domain Events
│   ├── Entities/             # Tenant, Company, Invoice, Product, Warehouse, StockMovement, Payment, UserInvitation, AuditLog, InvoiceSequence
│   └── ValueObjects/         # Money, VatRate, InvoiceNumber
├── Efakturon.Application     # Interfaces, Services, DTOs
│   ├── Interfaces/           # IInvoiceCalculationService, IInvoiceSequenceService, IStockService
│   └── Services/             # InvoiceCalculationService, InvoiceSequenceService, StockService
├── Efakturon.Infrastructure  # EF Core, Exports, Migrations
│   ├── Data/                 # EfakturonDbContext, TenantEntity (global filters)
│   ├── Exports/              # ExportService (HTML, PDF via QuestPDF, UBL XML)
│   └── Services/             # StockService, InvoiceSequenceService
├── Efakturon.Web             # MudBlazor Server-Side
│   ├── Components/           # Pages, Layout, Shared
│   └── Program.cs            # DI, Auth, Multi-tenancy middleware
└── tools/
    ├── Efakturon.Migrate     # CLI: dotnet ef migrations
    ├── Efakturon.Seed        # CLI: seed demo data
    └── Efakturon.Migrations  # Migration assembly
```

## Prerequisites

- .NET 8 SDK
- PostgreSQL 15+
- Docker (optional)

## Installation

```bash
git clone https://github.com/Rugents-masterpiece/efakturon.git
cd efakturon

# Configure connection string
cp src/Efakturon.Web/appsettings.Development.json src/Efakturon.Web/appsettings.json
# Edit appsettings.json → ConnectionStrings:DefaultConnection

# Apply migrations
dotnet run --project tools/Efakturon.Migrate

# Seed demo data (optional)
dotnet run --project tools/Efakturon.Seed

# Run web app
dotnet run --project src/Efakturon.Web
```

Access at `https://localhost:5001` (or configured port).

## Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=efakturon;Username=postgres;Password=***"
  },
  "Jwt": {
    "Issuer": "Efakturon",
    "Audience": "EfakturonClient",
    "Key": "***"
  },
  "Peppol": {
    "ParticipantId": "0196:YOUR_ID",
    "Scheme": "0196"
  }
}
```

## Docker Deployment

```bash
docker compose up -d --build

# Services:
# - efakturon-web (port 8080)
# - postgres (port 5432)
```

## Project Structure

| Layer | Purpose |
|-------|---------|
| `Domain` | Pure C#, no dependencies — entities, value objects, domain logic |
| `Application` | Interfaces, services, DTOs — depends only on Domain |
| `Infrastructure` | EF Core, exports, external services — depends on Application |
| `Web` | MudBlazor UI, DI, middleware — depends on all |

## Key Domain Entities

| Entity | Description |
|--------|-------------|
| `Tenant` | Isolated data scope, settings, subscription |
| `Company` | Legal entity within tenant (supplier/buyer) |
| `Invoice` | Header + lines, calculation, state machine (Draft→Issued→Paid→Cancelled) |
| `InvoiceItem` | Line with VAT, discounts, charges |
| `Product` | Catalog item with VAT rate, unit |
| `Warehouse` | Physical/logical stock location |
| `StockMovement` | IN/OUT/RESERVE/RELEASE with traceability |
| `Payment` | Payment records, QR generation |
| `InvoiceSequence` | Thread-safe numbering per tenant/company |
| `UserInvitation` | Tenant user onboarding |
| `AuditLog` | Immutable change log (entity, action, before/after, user) |

## Multi-tenancy

- `ITenantContext` resolved from JWT claims (not headers)
- `TenantEntity` base class + global query filter in `EfakturonDbContext`
- `SaveChanges` auto-sets `TenantId` on new entities
- SuperAdmin bypasses filter for cross-tenant ops

## Exports

| Format | Library | Use Case |
|--------|---------|----------|
| HTML | Razor templates | Email, preview |
| PDF | QuestPDF | Print, archive |
| UBL XML | Custom serializer | Peppol BIS 3.0, SAPI-SK |
| QR Code | QRCoder | Pay by Square, PayMe 2.0 |

## Development

```bash
# Watch mode
dotnet watch --project src/Efakturon.Web run

# Tests
dotnet test

# Migrations
dotnet ef migrations add Name --project src/Efakturon.Infrastructure --startup-project tools/Efakturon.Migrate
dotnet ef database update --project tools/Efakturon.Migrate
```

## Security

- **No credentials in repo** — all secrets via `appsettings.json` (gitignored) or env vars
- Row-level security via EF Core global filters
- IDOR protection: all queries scoped to `TenantId`
- Rate limiting on auth endpoints
- Security headers (CSP, HSTS, X-Frame-Options)

## License

MIT
