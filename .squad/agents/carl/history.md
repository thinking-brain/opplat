# Project Context

- **Owner:** Elvis Crego
- **Project:** opplat — Multi-platform business management system for café and restaurant operations. Multi-tenant SaaS with ASP.NET Core (.NET 10), EF Core, PostgreSQL, Finbuckle.MultiTenant, React 18, TypeScript, MUI, Keycloak (local) / Entra ID (production).
- **Stack:** ASP.NET Core (.NET 10), Entity Framework Core, PostgreSQL, Finbuckle.MultiTenant, SignalR, OIDC, React 18, Vite, TypeScript, Material-UI, Docker, nginx
- **Created:** 2026-03-31

## Key Architecture Facts

- My domain: `test/Opplat.MainApp.Test/` — backend test project
- Test run command: `dotnet test .\test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj --no-build --logger "console;verbosity=minimal"`
- Filtered test run: append `--filter "FullyQualifiedName~{keyword}"` for focused runs
- Modules 4+ have many NotImplementedException stubs — these are untested code paths
- Key test areas: tenant isolation, auth/OIDC flows, provisioning retry paths, cross-tenant data safety
- IMPLEMENTATION-PLAN.md is the coverage map — consult before writing new tests

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
