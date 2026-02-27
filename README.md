# Opplat

Multi-platform business management system for café and restaurant operations. Opplat provides comprehensive tools for managing sales, inventory, cash register operations, and multi-location business workflows with multi-tenant architecture.

## Tech Stack

- **Backend**: ASP.NET Core net10.0, Entity Framework Core, SQL Server, SignalR, JWT Bearer Authentication, Finbuckle.MultiTenant
- **Frontend**: React 18, Vite, TypeScript, Material-UI (MUI), Axios, React Router 6
- **Infrastructure**: Docker, Docker Compose, nginx

## Prerequisites

- **Docker Desktop** (for Docker Compose quick start)
- **.NET 10 SDK** (for local backend development)
- **Node.js 18+** (for local frontend development)
- **SQL Server** (for local development without Docker)

## Quick Start with Docker Compose

```bash
# 1. Copy the example env file
cp .env.docker .env

# 2. Edit .env and set your passwords/secrets
# (or use the defaults for local dev)

# 3. Start all services
docker-compose up -d

# Services will be available at:
# - API:      http://localhost:8080
# - Frontend: http://localhost:3000
# - SQL:      localhost:1433
```

### Development Mode with Hot Reload

Use `docker-compose.override.yml` for development with hot reload:

```bash
# Start with override (frontend hot reload enabled)
docker-compose up -d

# View logs
docker-compose logs -f

# Stop all services
docker-compose down
```

The override file configures the frontend to run in Vite dev mode with live reload.

### Production Build

To run production builds without hot reload:

```bash
# Temporarily disable override
docker-compose -f docker-compose.yml up -d
```

## Running Locally (Without Docker)

### Backend

```bash
cd src/Opplat.MainApp
dotnet restore
dotnet run
# API runs at https://localhost:5001 / http://localhost:5000
```

**Note**: Update `appsettings.Development.json` with your local SQL Server connection string.

### Frontend

```bash
cd src/opplat-react
npm install
npm run dev
# Frontend runs at http://localhost:5173
```

Create `src/opplat-react/.env.local` with:
```
VITE_API_URL=http://localhost:5000
```

## Project Structure

```
opplat/
├── src/
│   ├── Opplat.MainApp/          # ASP.NET Core API (net10.0)
│   │   ├── Areas/               # Feature areas (CashRegister, Inventory, Sales)
│   │   ├── Controllers/         # Auth, License, API controllers
│   │   ├── Hubs/                # SignalR real-time hubs
│   │   ├── Migrations/          # EF Core database migrations
│   │   ├── Dockerfile           # API container definition
│   │   └── appsettings.json     # Configuration
│   ├── Opplat.Domain/           # Domain entities and models
│   ├── Opplat.Infrastructure/   # Data access, repositories, services
│   ├── Opplat.Shared/           # Shared utilities and DTOs
│   ├── opplat-react/            # React 18 frontend (current)
│   │   ├── src/                 # React components and pages
│   │   ├── Dockerfile           # Frontend container definition
│   │   └── vite.config.ts       # Vite configuration
│   └── opplat-vue/              # Legacy Vue 2 frontend (reference only)
├── test/                        # Test projects
├── docker-compose.yml           # Production Docker Compose
├── docker-compose.override.yml  # Development overrides
├── .env.docker                  # Example environment variables
└── opplat.sln                   # Solution file
```

## Environment Variables Reference

| Variable | Description | Default | Required |
|----------|-------------|---------|----------|
| `SA_PASSWORD` | SQL Server SA password | `Admin123*` | Yes |
| `JWT_SECRET` | JWT signing key for authentication | `SuperSecretKey12345` | Yes |
| `VITE_API_URL` | Backend API URL for frontend | `http://localhost:8080` | Yes |
| `ConnectionStrings__DefaultConnection` | Main database connection string | *(see docker-compose.yml)* | Yes |
| `ConnectionStrings__MainConnection` | Main database connection string | *(see docker-compose.yml)* | Yes |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET Core environment | `Development` | No |
| `ASPNETCORE_URLS` | ASP.NET Core listening URLs | `http://+:8080` | No |
| `Authorization__Password` | JWT secret password | `${JWT_SECRET}` | Yes |
| `Authorization__Issuer` | JWT token issuer | `opplat.com` | Yes |
| `Authorization__Audience` | JWT token audience | `opplat.com` | Yes |

## Multi-Tenancy

Opplat uses **Finbuckle.MultiTenant** for complete tenant isolation with per-tenant databases:

- **Tenant resolution**: Via route prefix (e.g., `/{tenant}/api/...`)
- **Database isolation**: Each tenant has its own database with independent connection string
- **Configuration**: Tenants configured in `appsettings.json` under `Finbuckle:MultiTenant:Stores:ConfigurationStore`
- **Default tenants**: `mojocafe`, `demo`, `test`

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

### Tenant Databases

When running with Docker Compose, the following databases are automatically configured:

- `opplat-main` - Main application database
- `opplat-mojocafe` - MojoCafe tenant database
- `opplat-demo` - Demo tenant database
- `opplat-test` - Test tenant database

## Database Migrations

### Apply Migrations with Docker

```bash
# Run migrations in the API container
docker-compose exec api dotnet ef database update

# Create a new migration
docker-compose exec api dotnet ef migrations add MigrationName
```

### Apply Migrations Locally

```bash
cd src/Opplat.MainApp
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

Available hubs are defined in `src/Opplat.MainApp/Hubs/`.

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

### SQL Server Connection Issues

If API cannot connect to SQL Server:

1. Verify SQL Server container is healthy: `docker-compose ps`
2. Check SA password in `.env` matches SQL Server requirements
3. View API logs: `docker-compose logs api`

### Frontend Cannot Reach API

1. Ensure `VITE_API_URL` points to the correct API address
2. Check CORS configuration in `Program.cs`
3. Verify API is running: `curl http://localhost:8080/health`

### Port Conflicts

If ports 3000, 8080, or 1433 are already in use, modify the port mappings in `docker-compose.yml`:

```yaml
ports:
  - "8081:8080"  # Change host port (left side)
```

## License

[Specify License]

## Contributors

- Elvis Crego (elvis.crego)

---

**Need help?** Check the [Wiki](../../wiki) or open an issue.
