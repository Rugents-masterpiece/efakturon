#!/bin/bash
set -e

echo "=== Efakturon Seed Data ==="

# Create default tenant and admin user via psql
docker exec efakturon-postgres-1 psql -U efakturon -d efakturon -c "
-- Create default tenant
INSERT INTO tenants (id, name, subdomain, is_active, created_at, updated_at)
VALUES ('$(uuidgen)', 'Efakturon Demo', 'demo', true, NOW(), NOW())
ON CONFLICT DO NOTHING;

-- Get tenant ID
DO \$\$
DECLARE
    tenant_id UUID;
BEGIN
    SELECT id INTO tenant_id FROM tenants WHERE subdomain = 'demo' LIMIT 1;
    
    IF tenant_id IS NOT NULL THEN
        -- Create admin user
        INSERT INTO aspnetusers (id, username, normalizedusername, email, normalizedemail, emailconfirmed, passwordhash, first_name, last_name, tenant_id, accessfailedcount)
        VALUES (
            uuid_generate_v4(),
            'admin@efakturon.sk',
            'ADMIN@EFAKTURON.SK',
            'admin@efakturon.sk',
            'ADMIN@EFAKTURON.SK',
            true,
            'AQAAAAIAAYagAAAAEJxK3vF8F5F8F5F8F5F8F5F8F5F8F5F8F5F8F5F8F5F8F5F8F5F8F5F8F5F8==',
            'Admin',
            'User',
            tenant_id,
            0
        ) ON CONFLICT DO NOTHING;
        
        RAISE NOTICE 'Seed data created successfully';
    END IF;
END \$\$;
" 2>&1

echo "=== Seed Complete ==="
