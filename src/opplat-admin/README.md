# Opplat Admin App

Standalone React 18 admin SPA for cross-tenant operations.

## Scope

- Dedicated admin API integration only (`/admin/*`)
- Non-authenticated React shell for tenants, users, dashboard and settings
- Runtime env injection for Docker/Nginx deployments

## Local setup

```bash
cd src/opplat-admin
npm install
npm run dev
```

The admin app runs on `http://localhost:3201`.

When Aspire launches this SPA, it uses the `dev:aspire` script, keeps the same port, and injects
the local `admin-api` proxy target automatically.

For local host development, the Vite dev proxy should target the dedicated admin API on
`http://localhost:8084`. In Docker dev, `docker-compose.override.yml` points that proxy at the
`admin-api` service.

## Shell behavior

- The SPA no longer owns login, logout, callback or session bootstrap behavior.
- API requests go directly to the admin API surface.
- Leave `VITE_ADMIN_API_URL` empty when you want same-origin proxying; set it only when the SPA
  must call the admin API origin directly.
