# Project Context

- **Owner:** Elvis Crego
- **Project:** opplat — Multi-platform business management system for café and restaurant operations. Multi-tenant SaaS with ASP.NET Core (.NET 10), EF Core, PostgreSQL, Finbuckle.MultiTenant, React 18, TypeScript, MUI, Keycloak (local) / Entra ID (production).
- **Stack:** ASP.NET Core (.NET 10), Entity Framework Core, PostgreSQL, Finbuckle.MultiTenant, SignalR, OIDC, React 18, Vite, TypeScript, Material-UI, Docker, nginx
- **Created:** 2026-03-31

## Key Architecture Facts

- My domain: AdminApi and MainApp endpoint handlers, EF Core models, DbContexts, migrations
- AdminApi: `Features/Admin/Commands/` and `Features/Admin/Queries/` for CQRS-style handlers
- AdminApi DbContexts: `AdminTenantCatalogDbContext` (Module 2+ models), `LegacyTenants` DbSet for backward compatibility
- NpgsqlConnection with SSL mode normalization used across provisioning services
- Build with: `dotnet build .\opplat.slnx -m:1 -v minimal`
- Focused build: `dotnet build .\src\Opplat.AdminApi\Opplat.AdminApi.csproj -nologo`

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
