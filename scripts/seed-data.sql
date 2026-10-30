-- Efakturon Seed Data
-- Run this against the efakturon database

-- Create default tenant
INSERT INTO tenants (id, name, subdomain, is_active, created_at, updated_at)
VALUES ('a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Efakturon Demo', 'demo', true, NOW(), NOW())
ON CONFLICT (subdomain) DO NOTHING;

-- Create admin user
DO $$
DECLARE
    tenant_id UUID;
    user_id TEXT;
BEGIN
    SELECT id INTO tenant_id FROM tenants WHERE subdomain = 'demo' LIMIT 1;
    
    IF tenant_id IS NOT NULL THEN
        -- Create admin user
        INSERT INTO aspnetusers (id, username, normalizedusername, email, normalizedemail, emailconfirmed, passwordhash, firstname, lastname, tenantid, accessfailedcount)
        VALUES (
            'admin-user-id-1234-5678-90ab-cdef12345678',
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
END $$;
