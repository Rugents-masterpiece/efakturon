using Efakturon.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Efakturon.Infrastructure.Data;

public interface ITenantContext
{
    Guid CurrentTenantId { get; set; }
    bool IsSuperAdmin { get; set; }
}

public class TenantContext : ITenantContext
{
    public Guid CurrentTenantId { get; set; }
    public bool IsSuperAdmin { get; set; }
}

public class EfakturonDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ITenantContext _tenantContext;

    public EfakturonDbContext(DbContextOptions<EfakturonDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<UserInvitation> UserInvitations => Set<UserInvitation>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<InvoiceSequence> InvoiceSequences => Set<InvoiceSequence>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        if (!_tenantContext.IsSuperAdmin)
        {
            var tenantId = _tenantContext.CurrentTenantId;
            
            ApplyFilter<Company>(builder, e => e.TenantId == tenantId);
            ApplyFilter<Product>(builder, e => e.TenantId == tenantId);
            ApplyFilter<Invoice>(builder, e => e.TenantId == tenantId);
            ApplyFilter<Warehouse>(builder, e => e.TenantId == tenantId);
            ApplyFilter<StockMovement>(builder, e => e.TenantId == tenantId);
            ApplyFilter<Payment>(builder, e => e.TenantId == tenantId);
            ApplyFilter<UserInvitation>(builder, e => e.TenantId == tenantId);
            ApplyFilter<AuditLog>(builder, e => e.TenantId == tenantId);
            ApplyFilter<InvoiceSequence>(builder, e => e.TenantId == tenantId);
        }

        builder.Entity<Tenant>()
            .HasIndex(t => t.Subdomain)
            .IsUnique();

        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.Email)
            .IsUnique();

        builder.Entity<Invoice>()
            .HasIndex(i => new { i.TenantId, i.InvoiceNumber })
            .IsUnique();

        builder.Entity<Invoice>()
            .HasOne(i => i.SupplierNavigation)
            .WithMany()
            .HasForeignKey(i => i.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Invoice>()
            .HasOne(i => i.CustomerNavigation)
            .WithMany()
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<InvoiceItem>()
            .HasOne(i => i.Invoice)
            .WithMany(i => i.Items)
            .HasForeignKey(i => i.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<InvoiceItem>()
            .HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Payment>()
            .HasOne(p => p.Invoice)
            .WithMany(i => i.Payments)
            .HasForeignKey(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<StockMovement>()
            .HasOne(s => s.Product)
            .WithMany()
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StockMovement>()
            .HasOne(s => s.Warehouse)
            .WithMany()
            .HasForeignKey(s => s.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<StockMovement>()
            .HasOne(s => s.Invoice)
            .WithMany()
            .HasForeignKey(s => s.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        SeedRoles(builder);
    }

    private static void ApplyFilter<T>(ModelBuilder builder, Expression<Func<T, bool>> filter) where T : TenantEntity
    {
        builder.Entity<T>().HasQueryFilter(filter);
    }

    private static void SeedRoles(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
            new IdentityRole { Name = "TenantAdmin", NormalizedName = "TENANTADMIN" },
            new IdentityRole { Name = "Accountant", NormalizedName = "ACCOUNTANT" },
            new IdentityRole { Name = "User", NormalizedName = "USER" }
        );
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!_tenantContext.IsSuperAdmin)
        {
            foreach (var entry in ChangeTracker.Entries<TenantEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.TenantId = _tenantContext.CurrentTenantId;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
