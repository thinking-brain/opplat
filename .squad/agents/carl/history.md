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
- React SPA URL config is split between `src\Opplat.AppHost\Program.cs` (Aspire env injection) and each app's `src\runtimeConfig.ts`, which prefers `window.__OPPLAT_RUNTIME_CONFIG__` over `import.meta.env`.
- `src\opplat-react\src\api\auth.api.ts` routes subscription-plan and tenant-registration calls through `adminPublicAxiosClient`, so the client app also depends on the admin API base URL being correct.
- Current local port contract is inconsistent: Aspire runs `admin-api` on `http://localhost:8084`, but `src\opplat-react\src\runtimeConfig.ts` still falls back to `http://localhost:5160`, which is the standalone `Opplat.AdminApi` launchSettings port.
- The documented QA command in this history points to a missing project; the available backend test project in this repo is `test\Opplat.UnitTest\Opplat.UnitTest.csproj`.
- MainApp tenant-context regression coverage lives in `test\Opplat.UnitTest\Auth\AuthEndpointAuthorizationIntegrationTests.cs`; that harness must explicitly map `AccountEndpoints` and `AdminEndpoints` or auth endpoint assertions silently degrade into 404 noise.
- To regression-test tenant fallback paths, keep the resolved `IMultiTenantContextAccessor<AppTenantInfo>` empty and seed `IMultiTenantStore<AppTenantInfo>` separately; this is the only way to prove `/auth/account/tenant-context` survives claim-based tenant resolution when middleware has no pre-resolved tenant.

### 2026-04-15 — Team Validation & Decision Merge
- Bishop completed SPA URL simplification; verified both React apps build green
- Mother validated Aspire as SSOT; confirmed client-app regression surface and build status
- Regression coverage confirmed: frontend green, backend baseline failures unrelated to URL changes
- Decision merged to `.squad/decisions.md`; orchestration logs created
- Session log: `.squad/log/2026-04-15T19-13-58Z-url-config.md`
