using Efakturon.Application.Interfaces;
using Efakturon.Application.Services;
using Efakturon.Domain.Entities;
using Efakturon.Infrastructure.Data;
using Efakturon.Infrastructure.Exports;
using Efakturon.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// MudBlazor
builder.Services.AddMudServices();

// DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Chýba ConnectionStrings__DefaultConnection.");

builder.Services.AddDbContextFactory<EfakturonDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identity (AddIdentity already calls AddAuthentication internally)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<EfakturonDbContext>()
.AddDefaultTokenProviders();

// Configure Identity cookie options
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "EfakturonAuth";
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(4);
    options.SlidingExpiration = true;
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SuperAdmin", policy => policy.RequireRole("SuperAdmin"));
    options.AddPolicy("TenantAdmin", policy => policy.RequireRole("TenantAdmin", "SuperAdmin"));
    options.AddPolicy("Accountant", policy => policy.RequireRole("Accountant", "TenantAdmin", "SuperAdmin"));
});

// Tenant context
builder.Services.AddScoped<ITenantContext, TenantContext>();

// Services
builder.Services.AddScoped<IInvoiceCalculationService, InvoiceCalculationService>();
builder.Services.AddScoped<IInvoiceSequenceService, InvoiceSequenceService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IExportService, ExportService>();

// Blazor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Tenant middleware - resolve tenant from auth claims, NOT from headers
app.Use(async (context, next) =>
{
    var tenantContext = context.RequestServices.GetRequiredService<ITenantContext>();

    if (context.User.Identity?.IsAuthenticated == true)
    {
        var isAdmin = context.User.IsInRole("SuperAdmin");

        if (isAdmin)
        {
            tenantContext.IsSuperAdmin = true;
        }
        else
        {
            var tenantIdClaim = context.User.FindFirst("TenantId")?.Value;
            if (Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                tenantContext.CurrentTenantId = tenantId;
                tenantContext.IsSuperAdmin = false;
            }
        }
    }

    await next();
});

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EfakturonDbContext>();
    var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
    tenantContext.IsSuperAdmin = true;
    await context.Database.MigrateAsync();
    Console.WriteLine("Database migrations applied successfully!");
}

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

// Blazor
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
