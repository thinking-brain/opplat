# Project Context

- **Owner:** Elvis Crego
- **Project:** opplat — Multi-platform business management system for café and restaurant operations. Multi-tenant SaaS with ASP.NET Core (.NET 10), EF Core, PostgreSQL, Finbuckle.MultiTenant, React 18, TypeScript, MUI, Keycloak (local) / Entra ID (production).
- **Stack:** ASP.NET Core (.NET 10), Entity Framework Core, PostgreSQL, Finbuckle.MultiTenant, SignalR, OIDC, React 18, Vite, TypeScript, Material-UI, Docker, nginx
- **Created:** 2026-03-31

## Key Architecture Facts

- My domain: auth, identity, OIDC configuration for the entire platform
- Auth: Keycloak (local dev, port 8180, realm: opplat), Entra ID (production)
- Provider-neutral seams: AuthClaimTypes, OidcClaimsNormalizer, IGraphUserService with NoOp local-dev impl
- appsettings.Development.json has Keycloak OIDC config; appsettings.json has Entra placeholders
- Claims normalization: always use AuthClaimTypes constants — never raw string claim names
- Secret locations: ClientSecret in GraphApiOptions, Keycloak admin creds in docker-compose/.env — NEVER in committed source
- Multitenant attack surface: tenant resolution middleware in MainApp is a critical security boundary
- CORS, HTTPS, and auth middleware configuration in Program.cs for both AdminApi and MainApp

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
