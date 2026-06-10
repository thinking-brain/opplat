# Opplat

Multi-platform business management system for café and restaurant operations. Opplat provides comprehensive tools for managing sales, inventory, cash register operations, and multi-location business workflows with multi-tenant architecture.

## Tech Stack

- **Backend**: ASP.NET Core net10.0, Entity Framework Core, PostgreSQL, SignalR, OIDC Bearer auth, dedicated admin BFF/API, Finbuckle.MultiTenant
- **Frontend**: React 18, Vite, TypeScript, Material-UI (MUI), Axios, React Router 6
- **Infrastructure**: Docker, Docker Compose, nginx

## Prerequisites

- **Docker Desktop** (for Docker Compose quick start)
- **.NET 10 SDK** (for local backend development)
- **Node.js 18+** (for local frontend development)
- **PostgreSQL** (for local development without Docker)

## Quick Start with Docker Compose

```bash
# 1. Copy the Docker defaults into .env
# PowerShell:
Copy-Item .env.docker .env
# bash:
cp .env.docker .env

# 2. Review the local defaults in .env
# - KEYCLOAK_PORT=8180
# - KEYCLOAK_REALM=opplat
# - KEYCLOAK_ADMIN_USERNAME=admin
# - KEYCLOAK_ADMIN_PASSWORD=admin

# 3. Validate the merged compose file before starting
docker compose config

# 4. Start all services
docker compose up -d

# 5. Check startup
docker compose ps
```

## Quick Start with .NET Aspire

Aspire is the recommended inner-loop setup for local development. It keeps the four .NET backends as normal projects, provisions PostgreSQL and Keycloak as local containers, and now also orchestrates the two Vite apps with their usual dev scripts.

```bash
# From the repo root
dotnet run --project .\src\Opplat.AppHost\Opplat.AppHost.csproj
```

Expected local resources:

- Client app: `http://localhost:3200`
- Admin app: `http://localhost:3201`
- Main API: `http://localhost:8080`
- Sales API: `http://localhost:8083`
- Inventory API: `http://localhost:8082`
- Admin API: `http://localhost:8084`
- Keycloak realm: `http://localhost:8180/realms/opplat`
- PostgreSQL: `localhost:5432`

If you need to run the SPAs outside Aspire, the same `npm run dev` scripts still work:

```bash
cd .\src\opplat-react
npm run dev

cd ..\opplat-admin
npm run dev
```

Current local-development limitations:

- Aspire still relies on Docker Desktop for PostgreSQL and Keycloak containers.
- The AppHost is for local orchestration only; Docker Compose remains the documented production-style topology.
- The AppHost keeps the Vite apps in development mode; production-style frontend topology still comes from Docker Compose/nginx.

### Expected Local URLs

When you run `docker compose up` normally, Docker Compose loads both `docker compose.yml` and `docker compose.override.yml`.

- Client app (hot reload): `http://localhost:3200`
- Admin app (hot reload): `http://localhost:3201`
- Client app (base nginx container): `http://localhost:3100`
- Admin app (base nginx container): `http://localhost:3101`
- Main API: `http://localhost:8080`
- Sales API: `http://localhost:8083`
- Inventory API: `http://localhost:8082`
- Keycloak realm: `http://localhost:8180/realms/opplat`
- Keycloak admin console: `http://localhost:8180/admin/`
- PostgreSQL: `localhost:5432`

### Development Mode with Hot Reload

Use `docker compose.override.yml` for development with hot reload. It is picked up automatically by `docker compose up`.

```bash
# Start with override (frontend hot reload enabled on 3200/3201)
docker compose up -d

# View logs
docker compose logs -f

# Stop all services
docker compose down
```

The override file adds Vite dev servers on `http://localhost:3200` (client) and `http://localhost:3201` (admin). The base compose ports `3100/3101` remain published unless you explicitly run `docker compose -f docker compose.yml up`.

### Cheapest deployment model

If your priority is the lowest possible client-facing infrastructure cost, use the current Docker Compose topology on a single host:

- one small VM or container host
- one shared PostgreSQL instance
- no Kubernetes
- no paid API gateway
- `Opplat.Api.Main` only for shared bearer-auth, license, and menu endpoints
- dedicated `admin-api` for admin auth/session/BFF endpoints
- `Sales` and `Inventory` traffic sent directly to their own services

This keeps the microservice split while avoiding extra runtime components that add hosting cost.

### Production Build

To run production builds without hot reload:

```bash
# Temporarily disable override
docker compose -f docker compose.yml up -d
```

## Running Locally (Without Docker)

### Backend

The repository now includes four independently runnable ASP.NET Core backends:

- `src/Opplat.Api.Main` - existing monolith / composition root
- `src/Opplat.Api.Admin` - dedicated admin auth/session API host
- `src/Services/Sales/Opplat.Services.Sales.Api` - Sales microservice host
- `src/Services/Inventory/Opplat.Services.Inventory.Api` - Inventory microservice host

Run any service locally with:

```bash
cd src/Opplat.Api.Main
dotnet restore
dotnet run
# API runs at https://localhost:5001 / http://localhost:5000
```

For the new service hosts:

```bash
cd src/Opplat.Api.Admin
dotnet run

cd src/Services/Sales/Opplat.Services.Sales.Api
dotnet run

cd src/Services/Inventory/Opplat.Services.Inventory.Api
dotnet run
```

**Note**: Update `appsettings.Development.json` with your local PostgreSQL connection string. For host-machine development, keep `Auth:Authority` on `http://localhost:8180/realms/opplat`. In Docker Compose, the APIs also validate against that public issuer, but fetch OIDC discovery from the internal Keycloak URL through `Auth__MetadataAddress` so browser redirects and backend token validation stay aligned.

Recommended local auth section:

```json
"Auth": {
  "Authority": "http://localhost:8180/realms/opplat",
  "MetadataAddress": null,
  "Audience": "opplat-api",
  "ClaimNamespace": "https://opplat.com",
  "AdminRole": "SuperAdmin",
  "TenantAdminRole": "TenantAdmin",
  "TenantUserRole": "TenantUser"
}
```

### Frontend

```bash
cd src/opplat-react
npm install
npm run dev
# Frontend runs at http://localhost:3200
```

Create `src/opplat-react/.env.local` with:
```
VITE_API_URL=http://localhost:8080
VITE_AUTH_API_URL=http://localhost:8080
VITE_SALES_API_URL=http://localhost:8083
VITE_INVENTORY_API_URL=http://localhost:8082
VITE_AUTH_AUTHORITY=http://localhost:8180/realms/opplat
VITE_AUTH_CLIENT_ID=opplat-client
VITE_AUTH_AUDIENCE=opplat-api
VITE_AUTH_USE_AUDIENCE_QUERY_PARAM=false
VITE_AUTH_SCOPE=openid profile email offline_access
```

`VITE_API_URL` remains the shared fallback, but the cheapest microservice setup should point `VITE_AUTH_API_URL`, `VITE_SALES_API_URL`, and `VITE_INVENTORY_API_URL` at the dedicated services.

When you run through Aspire, the AppHost injects these values for both SPAs, so `.env.local` is
only needed for standalone frontend runs.

Create `src/opplat-admin/.env.local` with:
```
VITE_ADMIN_API_URL=
VITE_BFF_BASE_URL=
VITE_DEV_PROXY_TARGET=http://localhost:8084
```

Leave the admin API and BFF base URLs empty when you want the admin SPA to stay same-origin and
reach the dedicated `admin-api` through Vite/Nginx proxying.

For Azure Entra ID production, keep the same React auth stack and swap configuration only:

```ini
VITE_AUTH_AUTHORITY=https://login.microsoftonline.com/<tenant-id>/v2.0
VITE_AUTH_CLIENT_ID=<spa-app-client-id>
VITE_AUTH_AUDIENCE=
VITE_AUTH_USE_AUDIENCE_QUERY_PARAM=false
VITE_AUTH_SCOPE=openid profile email offline_access api://<api-app-id>/access_as_user
```

Use `VITE_AUTH_SCOPE` for Entra API permissions. Keep `VITE_AUTH_AUDIENCE` / `VITE_AUTH_USE_AUDIENCE_QUERY_PARAM=true` only for providers that expect a non-standard `audience` authorize-query parameter.

## Project Structure

```
opplat/
├── src/
│   ├── Opplat.Api.Main/          # ASP.NET Core API (net10.0)
│   │   ├── Areas/               # Feature areas (CashRegister, Inventory, Sales)
│   │   ├── Controllers/         # Auth, License, API controllers
│   │   ├── Hubs/                # SignalR real-time hubs
│   │   ├── Migrations/          # EF Core database migrations
│   │   ├── Dockerfile           # API container definition
│   │   └── appsettings.json     # Configuration
│   ├── Opplat.Domain/           # Domain entities and models
│   ├── Opplat.Infrastructure/   # Data access, repositories, services
│   ├── Opplat.Microservices.Shared/ # Shared hosting primitives for service hosts
│   ├── Services/
│   │   ├── Admin/
│   │   │   └── Opplat.Services.Admin.Api/      # Dedicated admin auth/session API host
│   │   ├── Sales/
│   │   │   └── Opplat.Services.Sales.Api/      # Sales microservice host
│   │   └── Inventory/
│   │       └── Opplat.Services.Inventory.Api/  # Inventory microservice host
│   ├── Opplat.Shared/           # Shared utilities and DTOs
│   ├── opplat-react/            # React 18 frontend (current)
│   │   ├── src/                 # React components and pages
│   │   ├── Dockerfile           # Frontend container definition
│   │   └── vite.config.ts       # Vite configuration
│   └── opplat-vue/              # Legacy Vue 2 frontend (reference only)
├── test/                        # Test projects
├── docker compose.yml           # Production Docker Compose
├── docker compose.override.yml  # Development overrides
├── .env.docker                  # Example environment variables
└── opplat.sln                   # Solution file
```

## Environment Variables Reference

| Variable | Description | Default | Required |
|----------|-------------|---------|----------|
| `POSTGRES_DB` | PostgreSQL bootstrap database name | `opplat_admin` | No |
| `POSTGRES_USER` | PostgreSQL bootstrap username | `postgres` | No |
| `POSTGRES_PASSWORD` | PostgreSQL password reused by local services | `Admin123*` | Yes |
| `POSTGRES_PORT` | Published PostgreSQL host port | `5432` | No |
| `KEYCLOAK_PORT` | Published Keycloak host port | `8180` | No |
| `KEYCLOAK_REALM` | Imported realm name used by discovery URLs | `opplat` | No |
| `KEYCLOAK_ADMIN_USERNAME` | Keycloak bootstrap admin username | `admin` | No |
| `KEYCLOAK_ADMIN_PASSWORD` | Keycloak bootstrap admin password | `admin` | No |
| `API_PORT` | Published main API host port | `8080` | No |
| `SALES_API_PORT` | Published sales API host port | `8083` | No |
| `INVENTORY_API_PORT` | Published inventory API host port | `8082` | No |
| `FRONTEND_PORT` | Published base client app host port | `3100` | No |
| `ADMIN_FRONTEND_PORT` | Published base admin app host port | `3101` | No |
| `VITE_API_URL` | Backend API URL for frontend | `http://localhost:8080` | Yes |
| `ConnectionStrings__DefaultConnection` | Main database connection string | *(see docker compose.yml)* | Yes |
| `ConnectionStrings__MainConnection` | Main database connection string | *(see docker compose.yml)* | Yes |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core environment | `Development` | No |
| `ASPNETCORE_URLS` | ASP.NET Core listening URLs | `http://+:8080` | No |
| `Auth__Authority` | Public OIDC issuer used to validate browser-issued tokens | `http://localhost:8180/realms/opplat` | Yes |
| `Auth__MetadataAddress` | Internal discovery URL used by containerized APIs for Keycloak backchannel access | `http://keycloak:8180/realms/opplat/.well-known/openid-configuration` | Yes |
| `Auth__Audience` | OIDC API audience | `opplat-api` | Yes |
| `Auth__AdminRole` | Platform admin realm role required by `/admin` endpoints | `SuperAdmin` | No |
| `Auth__TenantAdminRole` | Tenant admin realm role required by tenant user-management endpoints | `TenantAdmin` | No |
| `Auth__TenantUserRole` | Tenant user realm role used by tenant-facing authorization flows | `TenantUser` | No |
| `Auth__AdminBff__ClientId` | OIDC client used by the admin BFF login flow | `opplat-admin-bff` | For admin BFF |
| `Auth__AdminBff__ClientSecret` | Optional confidential-client secret for the admin BFF | empty | No |
| `Auth__AdminBff__AllowedOrigins__*` | Explicit admin SPA origins allowed to send credentialed cookie requests | `http://localhost:3001`, `3101`, `3201`, `5174` | For admin BFF |
| `Auth__AdminBff__UseAudienceQueryParam` | Adds a non-standard `audience` authorize query parameter for providers that require it | `false` | No |
| `VITE_AUTH_SCOPE` | SPA scope request sent to Keycloak | `openid` | No |

## Multi-Tenancy

Opplat uses **Finbuckle.MultiTenant** for complete tenant isolation with per-tenant databases:

- **Tenant resolution**: Via route prefix (e.g., `/{tenant}/api/...`)
- **Database isolation**: Each tenant has its own database with independent connection string
- **Configuration**: Tenants seed from `appsettings.json` and persist in `src/Opplat.Api.Main/Data/tenant-catalog.json`
- **Default tenants**: `mojocafe`, `demo`, `test`

## Authentication

`Opplat.Api.Main` now validates bearer tokens through OIDC discovery, while the dedicated admin API owns the admin cookie/BFF contract:

- **Production / shared environments**: point `Auth__Authority` at Auth0
- **Local Docker development**: point `Auth__Authority` at Keycloak (included in docker compose.yml)
- **No local JWT issuance**: Clients must authenticate with the configured identity provider (Auth0 or Keycloak)
- **Tenant validation**: the API cross-checks `tenant_id` / `tenant_identifier` token claims against Finbuckle route or `X-Tenant-Identifier` resolution
- **Role validation**: admin and tenant policies are backed by Keycloak realm roles (`SuperAdmin`, `TenantAdmin`, `TenantUser`)

The dedicated `admin-api` exposes the **admin-first BFF session layer** for `opplat-admin`:

- `GET /auth/bff/admin/login?returnUrl=<admin-url>` starts the server-managed OIDC login flow
- `POST /auth/bff/admin/logout` clears the backend admin session cookie
- `GET /admin/session/current-user` returns the current authenticated SuperAdmin session payload
- `GET /admin/session/csrf` issues the antiforgery token required for mutating cookie-authenticated admin requests
- Admin cookie/session ownership no longer lives in `Opplat.Api.Main`

### Admin BFF configuration contract

`Auth:AdminBff` is provider-neutral on purpose and is consumed by the dedicated admin API. Keep Keycloak local and Azure Entra production mostly as config swaps:

```json
"Auth": {
  "Authority": "http://localhost:8180/realms/opplat",
  "Audience": "opplat-api",
  "AdminBff": {
    "ClientId": "opplat-admin-bff",
    "ClientSecret": "",
    "Scopes": [ "openid", "profile", "email" ],
    "UsePkce": true,
    "UseAudienceQueryParam": false,
    "AllowedOrigins": [ "http://localhost:3201" ]
  }
}
```

- Use `ClientSecret` when the provider expects a confidential web app.
- Keep `UseAudienceQueryParam=false` for Keycloak and Entra; enable it only for providers such as Auth0 that expect `audience` on the authorize request.
- `AllowedOrigins` must be an explicit allow-list because credentialed cookie requests cannot use wildcard CORS.

### Keycloak Role and Claim Mapping

The local Keycloak realm intentionally uses **realm roles** for Opplat authorization instead of client-specific roles:

- `SuperAdmin` -> platform administration and admin SPA access
- `TenantAdmin` -> tenant user-management and elevated client-app access
- `TenantUser` -> standard tenant app access

The backend reads those roles from Keycloak token claims such as `realm_access.roles` and normalizes them into ASP.NET Core role claims before authorization policies run.

Both SPA clients also inherit two Opplat-specific default client scopes from Keycloak:

- `opplat-tenancy` -> injects `tenant_id` and `tenant_identifier`
- `opplat-api-audience` -> injects `aud=opplat-api`

That keeps the frontend config simple while ensuring the APIs receive the tenant and audience claims they validate.

Local development intentionally keeps **two public SPA clients** in Keycloak:

- `opplat-client` for the tenant-facing client app (`3100/3200/5173`)
- `opplat-admin` for the SuperAdmin portal (`3101/3201/5174`)

That split is deliberate, not a bug: each SPA has its own redirect URIs and browser origins, while both still share the same realm roles and Opplat client scopes.

### Keycloak Local Development Setup

Keycloak is automatically started with Docker Compose at `http://localhost:8180`. The container:

1. mounts `docker/keycloak/keycloak.conf` into `/opt/keycloak/conf/keycloak.conf`
2. mounts `docker/keycloak/opplat-realm.json` into `/opt/keycloak/data/import/opplat-realm.json`
3. starts with `start-dev --import-realm`
4. becomes healthy only after `http://localhost:8180/realms/opplat/.well-known/openid-configuration` returns `200`

That means the API services wait for Keycloak realm import to finish before starting.

#### Keycloak Admin Console

Use the bootstrap admin from `.env` for the Keycloak console itself:

| Field | Value |
|-------|-------|
| **URL** | `http://localhost:8180/admin/` |
| **Username** | `admin` (`KEYCLOAK_ADMIN_USERNAME`) |
| **Password** | `admin` (`KEYCLOAK_ADMIN_PASSWORD`) |

#### Default Opplat SuperAdmin User

Inside the imported `opplat` realm, a separate **SuperAdmin** application user is pre-configured:

| Field | Value |
|-------|-------|
| **Username** | `superadmin` |
| **Password** | `SuperAdmin123!` |
| **Email** | `superadmin@opplat.local` |
| **Role** | SuperAdmin |

### User Roles and Permissions

Opplat uses three role levels:

1. **SuperAdmin** — Platform-wide administrator
   - Only role with access to the **Admin Site** (`http://localhost:3201` in hot-reload dev, `http://localhost:3101` in base compose)
   - Can manage all system configuration
   - Can manage tenant catalog/configuration
   - Does **not** manage tenant user permissions from the admin site

2. **TenantAdmin** — Tenant administrator (assigned per tenant)
   - Access to the **Client App** (`http://localhost:3200` in hot-reload dev, `http://localhost:3100` in base compose) for their tenant
   - Can manage users and permissions **within the Client App's admin section**
   - Permissions management is **decentralized to the client app, not the admin site**
   - Can assign `TenantAdmin` or `TenantUser` roles within their tenant

3. **TenantUser** — Regular tenant user
    - Access to the **Client App** for their tenant
    - Standard end-user functionality
    - Cannot modify permissions or tenant settings

### Frontend auth flows and role gates

- **Admin SPA (`/opplat-admin`)** now uses a BFF session flow. The browser starts login at `/bff/auth/login`, restores identity from `/bff/auth/session`, and logs out through `/bff/auth/logout`. It no longer stores OIDC tokens or decodes JWT claims in the browser.
- **Client SPA (`/opplat-react`)** still uses OIDC authorization code flow with PKCE and normalizes claims from the returned tokens before routing users into the app.
- **Keycloak default client scopes**: the local realm already attaches `roles`, `opplat-tenancy`, and `opplat-api-audience`, so tenant claims and the API audience arrive from Keycloak configuration instead of custom frontend query parameters.

#### Role-based frontend access

1. **Admin SPA (`/opplat-admin`)**
    - Requires the `SuperAdmin` realm role for the root route and all nested pages.
    - Non-SuperAdmin accounts are authenticated but stopped at the frontend gate with an explicit access denied screen.
    - Tenant user/permission management stays out of the admin SPA and belongs in the client app.

2. **Client SPA (`/opplat-react`)**
   - Standard tenant workflows remain available to authenticated tenant users.
   - The `/users` route, the Users navigation entry, and the dashboard quick-access card are gated to `TenantAdmin`.
   - A `TenantUser` can sign in and operate the app, but cannot open tenant user management.

### Test Users

The following test users are pre-configured for local development:

| Username | Password | Tenant | Roles | Primary app |
|----------|----------|--------|-------|-------------|
| `superadmin` | `SuperAdmin123!` | platform | `SuperAdmin` | Admin site |
| `tenant-admin@mojocafe` | `TenantAdmin123!` | `mojocafe` | `TenantAdmin`, `TenantUser` | Client app |
| `tenant-admin@demo` | `TenantAdmin123!` | `demo` | `TenantAdmin`, `TenantUser` | Client app |
| `tenant-admin@test` | `TenantAdmin123!` | `test` | `TenantAdmin`, `TenantUser` | Client app |
| `user@mojocafe` | `TenantUser123!` | `mojocafe` | `TenantUser` | Client app |
| `user@demo` | `TenantUser123!` | `demo` | `TenantUser` | Client app |
| `user@test` | `TenantUser123!` | `test` | `TenantUser` | Client app |

### Accessing the Apps

**Client App** (tenant users and admins):
```
http://localhost:3200   # Vite dev server from docker compose.override.yml
http://localhost:3100   # Base nginx container from docker compose.yml
```

**Admin Site** (SuperAdmins only):
```
http://localhost:3201   # Vite dev server from docker compose.override.yml
http://localhost:3101   # Base nginx container from docker compose.yml
```

## Admin API

The main backend now exposes admin endpoints under `/admin` for:

- tenant catalog management (`/admin/tenants`)
- cross-tenant user listing (`/admin/users`)
- tenant-scoped user management (`/admin/tenants/{tenantIdentifier}/users`) using the `X-Tenant-Identifier` header for platform-side inspection only; tenant permission changes belong in the client app

### Accessing Tenant APIs

Prefix all API routes with the tenant identifier:

```bash
# MojoCafe tenant
curl http://localhost:8080/mojocafe/api/products

# Demo tenant
curl http://localhost:8080/demo/api/products

# Test tenant
curl http://localhost:8080/test/api/products
```

The new microservice hosts reuse the same tenant configuration model. They are exposed separately through Docker Compose at:

- `http://localhost:8083` - Sales service
- `http://localhost:8082` - Inventory service

This microservice slice now shares the PostgreSQL development topology used by Docker Compose and Aspire so the services can be run independently without a separate legacy database provider.

### Tenant Databases

When running with Docker Compose, the following databases are automatically configured:

- `opplat-main` - Main application database
- `opplat-mojocafe` - MojoCafe tenant database
- `opplat-demo` - Demo tenant database
- `opplat-test` - Test tenant database

## Database Migrations

### Generate migrations

`dotnet ef migrations add Initial --project src/Opplat.Infrastructure --startup-project src/Opplat.Api.Admin --context AdminTenantCatalogDbContext --output-dir Persistance/Migrations/Administration`

### Apply Migrations with Docker

```bash
# Run migrations in the API container
docker compose exec api dotnet ef database update

# Create a new migration
docker compose exec api dotnet ef migrations add MigrationName
```

### Apply Migrations Locally

```bash
cd src/Opplat.Api.Main
dotnet ef database update
dotnet ef migrations add MigrationName
```

## API Documentation

Swagger UI is available when running in Development mode:

```
http://localhost:8080/swagger
```

## SignalR Real-Time Communication

Opplat uses SignalR for real-time features. WebSocket endpoint:

```
http://localhost:8080/hubs/{hubname}
```

Available hubs are defined in `src/Opplat.Api.Main/Hubs/`.

## Development Notes

### Project Evolution

1. **Phase 1 (Complete)**: .NET upgrade from net6.0 to net10.0
2. **Phase 2 (In Progress)**: Frontend migration from Vue 2 to React 18
3. **Phase 3 (In Progress)**: Multi-tenancy implementation with Finbuckle

The `src/opplat-vue/` directory contains the legacy Vue 2 frontend for reference.

### Authentication

- **Method**: JWT Bearer tokens
- **Login endpoint**: `POST /{tenant}/api/auth/login`
- **Token validation**: Issuer and Audience must match `opplat.com`
- **Token expiration**: Configurable in `appsettings.json`

Include the JWT token in request headers:
```
Authorization: Bearer <your-token>
```

## Troubleshooting

### PostgreSQL Connection Issues

If an API cannot connect to PostgreSQL:

1. Verify the PostgreSQL container is healthy: `docker compose ps`
2. Check `POSTGRES_PASSWORD` in `.env` matches the container configuration
3. View API logs: `docker compose logs api`

### Frontend Cannot Reach API

1. Ensure `VITE_API_URL` points to the correct API address
2. Check CORS configuration in `Program.cs`
3. Verify the main API is running: `curl http://localhost:8080/docs/v1/docs.json`
4. Verify supporting services if needed: `curl http://localhost:8083/health` and `curl http://localhost:8082/health`

### Port Conflicts

If ports 8180, 8080, 8083, 8082, 3100/3200, 3101/3201, or 5432 are already in use, change the corresponding values in `.env` and re-run `docker compose config`:

```env
KEYCLOAK_PORT=8181
POSTGRES_PORT=5433
API_PORT=8085
SALES_API_PORT=8086
INVENTORY_API_PORT=8087
FRONTEND_PORT=3110
ADMIN_FRONTEND_PORT=3111
```

## License

[Specify License]

## Contributors

- Elvis Crego (elvis.crego)

---

**Need help?** Check the [Wiki](../../wiki) or open an issue.
