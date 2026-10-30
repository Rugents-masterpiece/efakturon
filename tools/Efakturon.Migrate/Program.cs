using Microsoft.EntityFrameworkCore;
using Efakturon.Infrastructure.Data;

var optionsBuilder = new DbContextOptionsBuilder<EfakturonDbContext>();
optionsBuilder.UseNpgsql("Host=postgres;Port=5432;Database=efakturon;Username=efakturon;Password=efakturon_dev_password");

var tenantContext = new TenantContext { IsSuperAdmin = true };
using var context = new EfakturonDbContext(optionsBuilder.Options, tenantContext);

Console.WriteLine("Applying migrations...");
await context.Database.MigrateAsync();
Console.WriteLine("Migrations applied successfully!");
