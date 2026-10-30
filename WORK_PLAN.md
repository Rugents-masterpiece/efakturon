# Efakturon – pracovný plán

## Todo
- [ ] Inicializácia solution a Git repozitára
- [ ] Doménové entity (Tenant, Company, Product, Invoice, InvoiceItem, Warehouse, StockMovement, Payment, UserInvitation, AuditLog, InvoiceSequence)
- [ ] Value Objects (Money, VatRate, InvoiceNumber)
- [ ] EF Core DbContext a konfigurácia
- [ ] Multi-tenancy: ITenantContext, global query filters, SaveChanges automatic TenantId
- [ ] ASP.NET Core Identity + ApplicationUser + roly (SuperAdmin, TenantAdmin, Accountant, User)
- [ ] Tenant resolution middleware (z claims, nie z HTTP headers)
- [ ] MudBlazor layout (MainLayout, App.razor, dark/light mode)
- [ ] Auth pages (Login, Register, Logout, ForgotPassword, ResetPassword)
- [ ] Dashboard
- [ ] Company CRUD
- [ ] Product CRUD
- [ ] Warehouse CRUD
- [ ] Invoice sequence service
- [ ] Invoice calculation service
- [ ] Invoice CRUD (Create, Detail, Edit, Issue, Cancel, Pay)
- [ ] Stock movement service
- [ ] HTML export
- [ ] PDF export (QuestPDF)
- [ ] UBL/Peppol XML export
- [ ] QR payment generators (Pay by Square, PayMe 2.0)
- [ ] Super Admin panel (tenant list, impersonation)
- [ ] Tenant Admin panel (users, invitations, settings)
- [ ] Audit log
- [ ] Security (rate limiting, CSRF, security headers, IDOR protection)
- [ ] Docker + docker-compose.yml
- [ ] Database migrations
- [ ] Unit tests (calculation, VAT, rounding, multi-tenant isolation, stock)
- [ ] Integration tests
- [ ] Documentation (README, docs/*.md)
- [ ] Finálny build, test, overenie

## In progress
- [ ] Inicializácia solution a Git repozitára

## Done

## Blocked
