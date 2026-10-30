using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Efakturon.Infrastructure.Data;
using Efakturon.Domain.Entities;

var optionsBuilder = new DbContextOptionsBuilder<EfakturonDbContext>();
optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5434;Database=efakturon;Username=efakturon;Password=efakturon_dev_password");

var tenantContext = new TenantContext { IsSuperAdmin = true };
using var context = new EfakturonDbContext(optionsBuilder.Options, tenantContext);

Console.WriteLine("Seeding data...");

// Create default tenant
var tenant = new Tenant
{
    Id = Guid.NewGuid(),
    Name = "Efakturon Demo",
    Subdomain = "demo",
    IsActive = true
};
context.Tenants.Add(tenant);
await context.SaveChangesAsync();

Console.WriteLine($"Created tenant: {tenant.Name}");

// Create default admin user
var userManager = new UserManager<ApplicationUser>(
    new UserStore<ApplicationUser>(context),
    new OptionsWrapper<IdentityOptions>(new IdentityOptions()),
    new PasswordHasher<ApplicationUser>(),
    new IUserValidator<ApplicationUser>[0],
    new IPasswordValidator<ApplicationUser>[0],
    new LowerInvariantLookupNormalizer(),
    new CapitalizationComparer<string>(),
    new HashSet<string>(),
    new ILogger<UserManager<ApplicationUser>>(),
    new ServiceCollection().BuildServiceProvider()
);

var adminUser = new ApplicationUser
{
    Id = Guid.NewGuid().ToString(),
    UserName = "admin@efakturon.sk",
    Email = "admin@efakturon.sk",
    EmailConfirmed = true,
    FirstName = "Admin",
    LastName = "User",
    TenantId = tenant.Id
};

var result = await userManager.CreateAsync(adminUser, "Admin123!");
if (result.Succeeded)
{
    await userManager.AddToRoleAsync(adminUser, "TenantAdmin");
    Console.WriteLine("Created admin user: admin@efakturon.sk");
}
else
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"User creation error: {error.Description}");
    }
}

Console.WriteLine("Seeding complete!");
