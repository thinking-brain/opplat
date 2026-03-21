# Opplat Admin App

Standalone React 18 admin SPA for cross-tenant operations.

## Scope

- OIDC login for platform admins
- Tenant CRUD shell wired to `/admin/tenants`
- Cross-tenant user management wired to `/admin/users`
- System settings wired to `/admin/settings`
- Runtime env injection for Docker/Nginx deployments

## Local setup

```bash
cd src/opplat-admin
npm install
npm run dev
```

The admin app runs on `http://localhost:3001`.
