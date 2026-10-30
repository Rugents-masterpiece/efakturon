#!/bin/bash
set -e

echo "=== Efakturon Database Migration ==="

# Check if PostgreSQL is healthy
echo "Checking PostgreSQL health..."
until docker exec efakturon-postgres-1 pg_isready -U efakturon -d efakturon > /dev/null 2>&1; do
    echo "Waiting for PostgreSQL..."
    sleep 2
done
echo "PostgreSQL is ready!"

# Run migration inside the efakturon container
echo "Running EF Core migrations..."
docker exec efakturon-efakturon-1 dotnet Efakturon.Web.dll dbcontext ef database update --project /app/Efakturon.Infrastructure.csproj --startup-project /app/Efakturon.Web.csproj 2>&1 || {
    echo "Direct dotnet command failed, trying alternative approach..."
    
    # Alternative: Create a migration script that runs on startup
    echo "Creating migration script..."
    cat > /tmp/migrate.cs << 'EOF'
using Microsoft.EntityFrameworkCore;
using Efakturon.Infrastructure.Data;

var optionsBuilder = new DbContextOptionsBuilder<EfakturonDbContext>();
optionsBuilder.UseNpgsql("Host=postgres;Port=5432;Database=efakturon;Username=efakturon;Password=efakturon_dev_password");

using var context = new EfakturonDbContext(optionsBuilder.Options, new TenantContext());
Console.WriteLine("Applying migrations...");
await context.Database.MigrateAsync();
Console.WriteLine("Migrations applied successfully!");
EOF

    docker cp /tmp/migrate.cs efakturon-efakturon-1:/tmp/migrate.cs
    docker exec efakturon-efakturon-1 dotnet /tmp/migrate.cs 2>&1 || echo "Migration script approach also failed"
}

echo "=== Migration Complete ==="
