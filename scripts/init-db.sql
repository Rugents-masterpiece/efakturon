-- Efakturon Database Schema
-- Run this against the efakturon database

-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Tenants table
CREATE TABLE IF NOT EXISTS tenants (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(200) NOT NULL,
    subdomain VARCHAR(50) NOT NULL UNIQUE,
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- AspNetRoles table (Identity)
CREATE TABLE IF NOT EXISTS aspnetroles (
    id TEXT PRIMARY KEY,
    name VARCHAR(256),
    normalizedName VARCHAR(256),
    concurrencyStamp TEXT
);

-- AspNetUsers table (Identity)
CREATE TABLE IF NOT EXISTS aspnetusers (
    id TEXT PRIMARY KEY,
    tenantId UUID REFERENCES tenants(id),
    firstName TEXT,
    lastName TEXT,
    userName VARCHAR(256),
    normalizedUserName VARCHAR(256),
    email VARCHAR(256),
    normalizedEmail VARCHAR(256),
    emailConfirmed BOOLEAN NOT NULL DEFAULT false,
    passwordHash TEXT,
    securityStamp TEXT,
    concurrencyStamp TEXT,
    phoneNumber TEXT,
    phoneNumberConfirmed BOOLEAN NOT NULL DEFAULT false,
    twoFactorEnabled BOOLEAN NOT NULL DEFAULT false,
    lockoutEnd TIMESTAMPTZ,
    lockoutEnabled BOOLEAN NOT NULL DEFAULT true,
    accessFailedCount INT NOT NULL DEFAULT 0
);

-- Companies table
CREATE TABLE IF NOT EXISTS companies (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenantId UUID NOT NULL,
    name VARCHAR(200) NOT NULL,
    ico VARCHAR(20),
    dic VARCHAR(20),
    dicPriloha VARCHAR(20),
    address TEXT,
    city VARCHAR(100),
    postalCode VARCHAR(10),
    country VARCHAR(100),
    bankAccount TEXT,
    swift VARCHAR(20),
    email VARCHAR(100),
    phone VARCHAR(20),
    website VARCHAR(200),
    note TEXT,
    createdAt TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Products table
CREATE TABLE IF NOT EXISTS products (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenantId UUID NOT NULL,
    name VARCHAR(200) NOT NULL,
    code VARCHAR(50),
    description TEXT,
    unit VARCHAR(10) NOT NULL DEFAULT 'ks',
    priceWithoutVat NUMERIC(18,2) NOT NULL,
    defaultVatRate NUMERIC(5,2) NOT NULL DEFAULT 20,
    isActive BOOLEAN NOT NULL DEFAULT true,
    createdAt TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Warehouses table
CREATE TABLE IF NOT EXISTS warehouses (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenantId UUID NOT NULL,
    name VARCHAR(100) NOT NULL,
    address TEXT,
    isActive BOOLEAN NOT NULL DEFAULT true,
    createdAt TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- InvoiceSequences table
CREATE TABLE IF NOT EXISTS invoice_sequences (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenantId UUID NOT NULL,
    year INT NOT NULL,
    prefix VARCHAR(10) NOT NULL DEFAULT 'FV',
    currentNumber INT NOT NULL DEFAULT 1,
    lastUpdatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- UserInvitations table
CREATE TABLE IF NOT EXISTS user_invitations (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenantId UUID NOT NULL,
    email VARCHAR(200) NOT NULL,
    role VARCHAR(50) NOT NULL,
    token TEXT NOT NULL,
    expiresAt TIMESTAMPTZ NOT NULL,
    usedAt TIMESTAMPTZ,
    createdBy TEXT NOT NULL,
    createdAt TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- AuditLogs table
CREATE TABLE IF NOT EXISTS audit_logs (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenantId UUID NOT NULL,
    userId TEXT,
    action VARCHAR(50) NOT NULL,
    entityType VARCHAR(50) NOT NULL,
    entityId TEXT,
    oldValues TEXT,
    newValues TEXT,
    ipAddress VARCHAR(45),
    createdAt TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Invoices table
CREATE TABLE IF NOT EXISTS invoices (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    tenantId UUID NOT NULL,
    invoiceNumber VARCHAR(20) NOT NULL,
    supplierId UUID REFERENCES companies(id),
    customerId UUID REFERENCES companies(id),
    issueDate TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    dueDate TIMESTAMPTZ NOT NULL,
    status INT NOT NULL DEFAULT 0,
    note TEXT,
    internalNote TEXT,
    totalWithoutVat NUMERIC(18,2) NOT NULL DEFAULT 0,
    totalVat NUMERIC(18,2) NOT NULL DEFAULT 0,
    totalWithVat NUMERIC(18,2) NOT NULL DEFAULT 0,
    currency VARCHAR(3) NOT NULL DEFAULT 'EUR',
    createdAt TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- InvoiceItems table
CREATE TABLE IF NOT EXISTS invoice_items (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    invoiceId UUID NOT NULL REFERENCES invoices(id) ON DELETE CASCADE,
    productId UUID REFERENCES products(id),
    description TEXT NOT NULL,
    quantity NUMERIC(18,4) NOT NULL,
    unitPriceWithoutVat NUMERIC(18,2) NOT NULL,
    discountPercent NUMERIC(5,2) NOT NULL DEFAULT 0,
    vatRate NUMERIC(5,2) NOT NULL DEFAULT 20,
    createdAt TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- Payments table
CREATE TABLE IF NOT EXISTS payments (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    invoiceId UUID NOT NULL REFERENCES invoices(id) ON DELETE CASCADE,
    amount NUMERIC(18,2) NOT NULL,
    paymentDate TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    method VARCHAR(50) NOT NULL,
    reference VARCHAR(50),
    note TEXT,
    createdAt TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- StockMovements table
CREATE TABLE IF NOT EXISTS stock_movements (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    productId UUID NOT NULL REFERENCES products(id),
    warehouseId UUID REFERENCES warehouses(id),
    invoiceId UUID REFERENCES invoices(id),
    type INT NOT NULL,
    quantity NUMERIC(18,4) NOT NULL,
    reference VARCHAR(50),
    note TEXT,
    createdAt TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- AspNetRoleClaims table
CREATE TABLE IF NOT EXISTS aspnetroleclaims (
    id SERIAL PRIMARY KEY,
    roleId TEXT NOT NULL REFERENCES aspnetroles(id) ON DELETE CASCADE,
    claimType TEXT,
    claimValue TEXT
);

-- AspNetUserClaims table
CREATE TABLE IF NOT EXISTS aspnetuserclaims (
    id SERIAL PRIMARY KEY,
    userId TEXT NOT NULL REFERENCES aspnetusers(id) ON DELETE CASCADE,
    claimType TEXT,
    claimValue TEXT
);

-- AspNetUserLogins table
CREATE TABLE IF NOT EXISTS aspnetuserlogins (
    loginProvider TEXT NOT NULL,
    providerKey TEXT NOT NULL,
    providerDisplayName TEXT,
    userId TEXT NOT NULL REFERENCES aspnetusers(id) ON DELETE CASCADE,
    PRIMARY KEY (loginProvider, providerKey)
);

-- AspNetUserRoles table
CREATE TABLE IF NOT EXISTS aspnetuserroles (
    userId TEXT NOT NULL REFERENCES aspnetusers(id) ON DELETE CASCADE,
    roleId TEXT NOT NULL REFERENCES aspnetroles(id) ON DELETE CASCADE,
    PRIMARY KEY (userId, roleId)
);

-- AspNetUserTokens table
CREATE TABLE IF NOT EXISTS aspnetusertokens (
    userId TEXT NOT NULL REFERENCES aspnetusers(id) ON DELETE CASCADE,
    loginProvider TEXT NOT NULL,
    name TEXT NOT NULL,
    value TEXT,
    PRIMARY KEY (userId, loginProvider, name)
);

-- Indexes
CREATE UNIQUE INDEX IF NOT EXISTS ix_tenants_subdomain ON tenants(subdomain);
CREATE UNIQUE INDEX IF NOT EXISTS ix_aspnetusers_email ON aspnetusers(email) WHERE email IS NOT NULL;
CREATE INDEX IF NOT EXISTS ix_aspnetusers_tenantid ON aspnetusers(tenantId);
CREATE INDEX IF NOT EXISTS ix_aspnetroleclaims_roleid ON aspnetroleclaims(roleId);
CREATE INDEX IF NOT EXISTS ix_aspnetuserclaims_userid ON aspnetuserclaims(userId);
CREATE INDEX IF NOT EXISTS ix_aspnetuserlogins_userid ON aspnetuserlogins(userId);
CREATE INDEX IF NOT EXISTS ix_aspnetuserroles_roleid ON aspnetuserroles(roleId);
CREATE UNIQUE INDEX IF NOT EXISTS ix_invoices_tenantid_invoicenumber ON invoices(tenantId, invoiceNumber);
CREATE INDEX IF NOT EXISTS ix_invoiceitems_invoiceid ON invoice_items(invoiceId);
CREATE INDEX IF NOT EXISTS ix_invoiceitems_productid ON invoice_items(productId);
CREATE UNIQUE INDEX IF NOT EXISTS ix_invoice_sequences_tenantid_year ON invoice_sequences(tenantId, year);
CREATE INDEX IF NOT EXISTS ix_payments_invoiceid ON payments(invoiceId);
CREATE INDEX IF NOT EXISTS ix_stock_movements_invoiceid ON stock_movements(invoiceId);
CREATE INDEX IF NOT EXISTS ix_stock_movements_productid ON stock_movements(productId);
CREATE INDEX IF NOT EXISTS ix_stock_movements_warehouseid ON stock_movements(warehouseId);
CREATE INDEX IF NOT EXISTS ix_user_invitations_email ON user_invitations(email);
CREATE UNIQUE INDEX IF NOT EXISTS ix_user_invitations_token ON user_invitations(token);

-- Seed roles
INSERT INTO aspnetroles (id, name, normalizedName, concurrencyStamp) VALUES
    ('00000000-0000-0000-0000-000000000001', 'SuperAdmin', 'SUPERADMIN', NULL),
    ('00000000-0000-0000-0000-000000000002', 'TenantAdmin', 'TENANTADMIN', NULL),
    ('00000000-0000-0000-0000-000000000003', 'Accountant', 'ACCOUNTANT', NULL),
    ('00000000-0000-0000-0000-000000000004', 'User', 'USER', NULL)
ON CONFLICT DO NOTHING;

-- Record migration
INSERT INTO __efmigrationshistory (migrationid, productversion)
VALUES ('20260722000000_InitialCreate', '8.0.0')
ON CONFLICT DO NOTHING;
