using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Efakturon.Infrastructure.Data.Migrations;

/// <summary>
/// Ručne vytvorená počiatočná migrácia pre Efakturon DbContext.
/// Vygenerovaná manuálne kvôli .NET version kompatibilite.
/// </summary>
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Create sequences for PostgreSQL
        migrationBuilder.CreateSequence<long>("tenant_id_seq", schema: "public");
        migrationBuilder.CreateSequence<long>("company_id_seq", schema: "public");
        migrationBuilder.CreateSequence<long>("product_id_seq", schema: "public");
        migrationBuilder.CreateSequence<long>("invoice_id_seq", schema: "public");
        migrationBuilder.CreateSequence<long>("invoice_item_id_seq", schema: "public");
        migrationBuilder.CreateSequence<long>("warehouse_id_seq", schema: "public");
        migrationBuilder.CreateSequence<long>("stock_movement_id_seq", schema: "public");
        migrationBuilder.CreateSequence<long>("payment_id_seq", schema: "public");
        migrationBuilder.CreateSequence<long>("user_invitation_id_seq", schema: "public");
        migrationBuilder.CreateSequence<long>("audit_log_id_seq", schema: "public");
        migrationBuilder.CreateSequence<long>("invoice_sequence_id_seq", schema: "public");

        // Tenants table
        migrationBuilder.CreateTable(
            name: "tenants",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: Guid.NewGuid()),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                subdomain = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_tenants", x => x.id);
            });

        // AspNetRoles table (Identity)
        migrationBuilder.CreateTable(
            name: "aspnetroles",
            columns: table => new
            {
                id = table.Column<string>(type: "text", nullable: false),
                name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                normalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                concurrencyStamp = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_aspnetroles", x => x.id);
            });

        // AspNetUsers table (Identity)
        migrationBuilder.CreateTable(
            name: "aspnetusers",
            columns: table => new
            {
                id = table.Column<string>(type: "text", nullable: false),
                tenantId = table.Column<Guid>(type: "uuid", nullable: true),
                firstName = table.Column<string>(type: "text", nullable: true),
                lastName = table.Column<string>(type: "text", nullable: true),
                userName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                normalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                normalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                emailConfirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                passwordHash = table.Column<string>(type: "text", nullable: true),
                securityStamp = table.Column<string>(type: "text", nullable: true),
                concurrencyStamp = table.Column<string>(type: "text", nullable: true),
                phoneNumber = table.Column<string>(type: "text", nullable: true),
                phoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                twoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                lockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                lockoutEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                accessFailedCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_aspnetusers", x => x.id);
                table.ForeignKey(
                    name: "fk_aspnetusers_tenants_tenantid",
                    column: x => x.tenantId,
                    principalTable: "tenants",
                    principalColumn: "id");
            });

        // Companies table
        migrationBuilder.CreateTable(
            name: "companies",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: Guid.NewGuid()),
                tenantId = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                ico = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                dic = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                dicPriloha = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                address = table.Column<string>(type: "text", nullable: true),
                city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                postalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                bankAccount = table.Column<string>(type: "text", nullable: true),
                swift = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                note = table.Column<string>(type: "text", nullable: true),
                createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_companies", x => x.id);
            });

        // Products table
        migrationBuilder.CreateTable(
            name: "products",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: Guid.NewGuid()),
                tenantId = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                description = table.Column<string>(type: "text", nullable: true),
                unit = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "ks"),
                priceWithoutVat = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                defaultVatRate = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 20m),
                isActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_products", x => x.id);
            });

        // Warehouses table
        migrationBuilder.CreateTable(
            name: "warehouses",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: Guid.NewGuid()),
                tenantId = table.Column<Guid>(type: "uuid", nullable: false),
                name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                address = table.Column<string>(type: "text", nullable: true),
                isActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_warehouses", x => x.id);
            });

        // InvoiceSequences table
        migrationBuilder.CreateTable(
            name: "invoice_sequences",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: Guid.NewGuid()),
                tenantId = table.Column<Guid>(type: "uuid", nullable: false),
                year = table.Column<int>(type: "integer", nullable: false),
                prefix = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "FV"),
                currentNumber = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                lastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_invoice_sequences", x => x.id);
            });

        // UserInvitations table
        migrationBuilder.CreateTable(
            name: "user_invitations",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: Guid.NewGuid()),
                tenantId = table.Column<Guid>(type: "uuid", nullable: false),
                email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                token = table.Column<string>(type: "text", nullable: false),
                expiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                usedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                createdBy = table.Column<string>(type: "text", nullable: false),
                createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_user_invitations", x => x.id);
            });

        // AuditLogs table
        migrationBuilder.CreateTable(
            name: "audit_logs",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: Guid.NewGuid()),
                tenantId = table.Column<Guid>(type: "uuid", nullable: false),
                userId = table.Column<string>(type: "text", nullable: true),
                action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                entityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                entityId = table.Column<string>(type: "text", nullable: true),
                oldValues = table.Column<string>(type: "text", nullable: true),
                newValues = table.Column<string>(type: "text", nullable: true),
                ipAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_audit_logs", x => x.id);
            });

        // Invoices table
        migrationBuilder.CreateTable(
            name: "invoices",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: Guid.NewGuid()),
                tenantId = table.Column<Guid>(type: "uuid", nullable: false),
                invoiceNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                supplierId = table.Column<Guid>(type: "uuid", nullable: true),
                customerId = table.Column<Guid>(type: "uuid", nullable: true),
                issueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                dueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                status = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                note = table.Column<string>(type: "text", nullable: true),
                internalNote = table.Column<string>(type: "text", nullable: true),
                totalWithoutVat = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                totalVat = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                totalWithVat = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "EUR"),
                createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_invoices", x => x.id);
                table.ForeignKey(
                    name: "fk_invoices_companies_customerid",
                    column: x => x.customerId,
                    principalTable: "companies",
                    principalColumn: "id");
                table.ForeignKey(
                    name: "fk_invoices_companies_supplierid",
                    column: x => x.supplierId,
                    principalTable: "companies",
                    principalColumn: "id");
            });

        // InvoiceItems table
        migrationBuilder.CreateTable(
            name: "invoice_items",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: Guid.NewGuid()),
                invoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                productId = table.Column<Guid>(type: "uuid", nullable: true),
                description = table.Column<string>(type: "text", nullable: false),
                quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                unitPriceWithoutVat = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                discountPercent = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                vatRate = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 20m),
                createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_invoice_items", x => x.id);
                table.ForeignKey(
                    name: "fk_invoice_items_invoices_invoiceid",
                    column: x => x.invoiceId,
                    principalTable: "invoices",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_invoice_items_products_productid",
                    column: x => x.productId,
                    principalTable: "products",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        // Payments table
        migrationBuilder.CreateTable(
            name: "payments",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: Guid.NewGuid()),
                invoiceId = table.Column<Guid>(type: "uuid", nullable: false),
                amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                paymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                method = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                reference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                note = table.Column<string>(type: "text", nullable: true),
                createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_payments", x => x.id);
                table.ForeignKey(
                    name: "fk_payments_invoices_invoiceid",
                    column: x => x.invoiceId,
                    principalTable: "invoices",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        // StockMovements table
        migrationBuilder.CreateTable(
            name: "stock_movements",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false, defaultValue: Guid.NewGuid()),
                productId = table.Column<Guid>(type: "uuid", nullable: false),
                warehouseId = table.Column<Guid>(type: "uuid", nullable: true),
                invoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                type = table.Column<int>(type: "integer", nullable: false),
                quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                reference = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                note = table.Column<string>(type: "text", nullable: true),
                createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_stock_movements", x => x.id);
                table.ForeignKey(
                    name: "fk_stock_movements_invoices_invoiceid",
                    column: x => x.invoiceId,
                    principalTable: "invoices",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_stock_movements_products_productid",
                    column: x => x.productId,
                    principalTable: "products",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "fk_stock_movements_warehouses_warehouseid",
                    column: x => x.warehouseId,
                    principalTable: "warehouses",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Restrict);
            });

        // AspNetRoleClaims table
        migrationBuilder.CreateTable(
            name: "aspnetroleclaims",
            columns: table => new
            {
                id = table.Column<int>(type: "serial", nullable: false),
                roleId = table.Column<string>(type: "text", nullable: false),
                claimType = table.Column<string>(type: "text", nullable: true),
                claimValue = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_aspnetroleclaims", x => x.id);
                table.ForeignKey(
                    name: "fk_aspnetroleclaims_aspnetroles_roleid",
                    column: x => x.roleId,
                    principalTable: "aspnetroles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        // AspNetUserClaims table
        migrationBuilder.CreateTable(
            name: "aspnetuserclaims",
            columns: table => new
            {
                id = table.Column<int>(type: "serial", nullable: false),
                userId = table.Column<string>(type: "text", nullable: false),
                claimType = table.Column<string>(type: "text", nullable: true),
                claimValue = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_aspnetuserclaims", x => x.id);
                table.ForeignKey(
                    name: "fk_aspnetuserclaims_aspnetusers_userid",
                    column: x => x.userId,
                    principalTable: "aspnetusers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        // AspNetUserLogins table
        migrationBuilder.CreateTable(
            name: "aspnetuserlogins",
            columns: table => new
            {
                loginProvider = table.Column<string>(type: "text", nullable: false),
                providerKey = table.Column<string>(type: "text", nullable: false),
                providerDisplayName = table.Column<string>(type: "text", nullable: true),
                userId = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_aspnetuserlogins", x => new { x.loginProvider, x.providerKey });
                table.ForeignKey(
                    name: "fk_aspnetuserlogins_aspnetusers_userid",
                    column: x => x.userId,
                    principalTable: "aspnetusers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        // AspNetUserRoles table
        migrationBuilder.CreateTable(
            name: "aspnetuserroles",
            columns: table => new
            {
                userId = table.Column<string>(type: "text", nullable: false),
                roleId = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_aspnetuserroles", x => new { x.userId, x.roleId });
                table.ForeignKey(
                    name: "fk_aspnetuserroles_aspnetroles_roleid",
                    column: x => x.roleId,
                    principalTable: "aspnetroles",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "fk_aspnetuserroles_aspnetusers_userid",
                    column: x => x.userId,
                    principalTable: "aspnetusers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        // AspNetUserTokens table
        migrationBuilder.CreateTable(
            name: "aspnetusertokens",
            columns: table => new
            {
                userId = table.Column<string>(type: "text", nullable: false),
                loginProvider = table.Column<string>(type: "text", nullable: false),
                name = table.Column<string>(type: "text", nullable: false),
                value = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("pk_aspnetusertokens", x => new { x.userId, x.loginProvider, x.name });
                table.ForeignKey(
                    name: "fk_aspnetusertokens_aspnetusers_userid",
                    column: x => x.userId,
                    principalTable: "aspnetusers",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        // Create indexes
        migrationBuilder.CreateIndex(
            name: "ix_tenants_subdomain",
            table: "tenants",
            column: "subdomain",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_aspnetusers_email",
            table: "aspnetusers",
            column: "email",
            unique: true,
            filter: "email IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "ix_aspnetusers_tenantid",
            table: "aspnetusers",
            column: "tenantid");

        migrationBuilder.CreateIndex(
            name: "ix_aspnetroleclaims_roleid",
            table: "aspnetroleclaims",
            column: "roleid");

        migrationBuilder.CreateIndex(
            name: "ix_aspnetuserclaims_userid",
            table: "aspnetuserclaims",
            column: "userid");

        migrationBuilder.CreateIndex(
            name: "ix_aspnetuserlogins_userid",
            table: "aspnetuserlogins",
            column: "userid");

        migrationBuilder.CreateIndex(
            name: "ix_aspnetuserroles_roleid",
            table: "aspnetuserroles",
            column: "roleid");

        migrationBuilder.CreateIndex(
            name: "ix_invoices_tenantid_invoicenumber",
            table: "invoices",
            columns: new[] { "tenantid", "invoicenumber" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_invoiceitems_invoiceid",
            table: "invoice_items",
            column: "invoiceid");

        migrationBuilder.CreateIndex(
            name: "ix_invoiceitems_productid",
            table: "invoice_items",
            column: "productid");

        migrationBuilder.CreateIndex(
            name: "ix_invoice_sequences_tenantid_year",
            table: "invoice_sequences",
            columns: new[] { "tenantid", "year" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_payments_invoiceid",
            table: "payments",
            column: "invoiceid");

        migrationBuilder.CreateIndex(
            name: "ix_stock_movements_invoiceid",
            table: "stock_movements",
            column: "invoiceid");

        migrationBuilder.CreateIndex(
            name: "ix_stock_movements_productid",
            table: "stock_movements",
            column: "productid");

        migrationBuilder.CreateIndex(
            name: "ix_stock_movements_warehouseid",
            table: "stock_movements",
            column: "warehouseid");

        migrationBuilder.CreateIndex(
            name: "ix_user_invitations_email",
            table: "user_invitations",
            column: "email");

        migrationBuilder.CreateIndex(
            name: "ix_user_invitations_token",
            table: "user_invitations",
            column: "token",
            unique: true);

        // Seed roles
        migrationBuilder.InsertData(
            table: "aspnetroles",
            columns: new[] { "id", "name", "normalizedName", "concurrencyStamp" },
            values: new object[,]
            {
                { "00000000-0000-0000-0000-000000000001", "SuperAdmin", "SUPERADMIN", null },
                { "00000000-0000-0000-0000-000000000002", "TenantAdmin", "TENANTADMIN", null },
                { "00000000-0000-0000-0000-000000000003", "Accountant", "ACCOUNTANT", null },
                { "00000000-0000-0000-0000-000000000004", "User", "USER", null }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "aspnetroleclaims");
        migrationBuilder.DropTable(name: "aspnetuserclaims");
        migrationBuilder.DropTable(name: "aspnetuserlogins");
        migrationBuilder.DropTable(name: "aspnetuserroles");
        migrationBuilder.DropTable(name: "aspnetusertokens");
        migrationBuilder.DropTable(name: "invoice_items");
        migrationBuilder.DropTable(name: "payments");
        migrationBuilder.DropTable(name: "stock_movements");
        migrationBuilder.DropTable(name: "user_invitations");
        migrationBuilder.DropTable(name: "audit_logs");
        migrationBuilder.DropTable(name: "invoice_sequences");
        migrationBuilder.DropTable(name: "warehouses");
        migrationBuilder.DropTable(name: "products");
        migrationBuilder.DropTable(name: "invoices");
        migrationBuilder.DropTable(name: "aspnetroles");
        migrationBuilder.DropTable(name: "aspnetusers");
        migrationBuilder.DropTable(name: "companies");
        migrationBuilder.DropTable(name: "tenants");

        migrationBuilder.DropSequence(name: "tenant_id_seq");
        migrationBuilder.DropSequence(name: "company_id_seq");
        migrationBuilder.DropSequence(name: "product_id_seq");
        migrationBuilder.DropSequence(name: "invoice_id_seq");
        migrationBuilder.DropSequence(name: "invoice_item_id_seq");
        migrationBuilder.DropSequence(name: "warehouse_id_seq");
        migrationBuilder.DropSequence(name: "stock_movement_id_seq");
        migrationBuilder.DropSequence(name: "payment_id_seq");
        migrationBuilder.DropSequence(name: "user_invitation_id_seq");
        migrationBuilder.DropSequence(name: "audit_log_id_seq");
        migrationBuilder.DropSequence(name: "invoice_sequence_id_seq");
    }
}
