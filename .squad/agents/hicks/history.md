## Core Context

### 2026-03-21 Session 8: Backend Auth Topology Confirmation & CORS Diagnosis

**Role in Session 8:** Validated that backend auth validation is independent of Keycloak client topology. Confirmed:
1. Backend validates `audience=opplat-api` regardless of which client issued the token
2. Both `opplat-client` and `opplat-admin` clients share the same audience; no backend changes needed for two-client model
3. Two-client model does not introduce per-client authorization paths in backend
4. Admin bootstrap resilience (skip unreachable tenant DBs) already prevents false login failures

**Finding:** CORS issue is stale Keycloak runtime state, not backend auth contract misalignment. Both SPAs can coexist with one shared audience without backend code changes.

**Cross-Team Learning:** Hicks coordinated with Ripley (architecture), Vasquez (frontend), Hudson (infrastructure) to confirm realm topology is sound.
Validated that backend auth does not depend on Keycloak client ID; depends on issuer, \ud=opplat-api\, and normalized SuperAdmin role contract. Confirmed two-client Keycloak model is not root cause of login failures. Backend already aligned with realm topology. No backend changes needed for two-client architecture.

### Cross-Tenant Admin Bootstrap Resilience (2026-03-21 Session 6b)
Updated \GetAdminUsersQueryHandler\ to skip unreachable tenant databases instead of failing entire bootstrap. Admin dashboard now resilient to one or more tenant DB temporary unavailability.

### Admin Callback & Claim Contract Fix (2026-03-21)
Normalized Keycloak role claims across backend auth to handle both nested and flat dotted claim shapes. Updated AuthClaimTypes.cs and OidcClaimsTransformation.cs to accept both payload materializations.

## Archived Context (Prior Sessions — Phases 1–3d)

See \.squad/orchestration-log/\ for detailed session outcomes. Key milestones: Finbuckle.MultiTenant 7.0.1 implementation (2026-02-27), Sales/Inventory module extraction (2026-03-18), admin callback fix (2026-03-21 Session 6b).

## Project Context

**Project:** Opplat — Multi-platform business management system (café/restaurant)
**Requested by:** elvis.crego
**Stack:** ASP.NET Core 10.0 | EF Core | SQL Server | SignalR | JWT | React 18
**Solution root:** C:\projects\personal\opplat
**Branch:** develop

## Solution Structure

- src/Opplat.MainApp/ — ASP.NET Core Web API (net10.0)
- src/Opplat.Domain/ — Business logic (net10.0)
- src/Opplat.Infrastructure/ — Data access, EF Core (net10.0)
- src/Opplat.Shared/ — Common utilities (net10.0)
- src/opplat-react/ — Client React 18 SPA (Vite, MUI, React Router)
- src/opplat-admin/ — Admin React 18 SPA
- test/Opplat.MainApp.Test/ — xunit tests (net10.0)

## Key Architecture

- Clean Architecture (Domain / Infrastructure / MainApp)
- EF Core DbContext with ASP.NET Core Identity
- JWT Bearer auth with Keycloak OIDC
- SignalR hubs
- Swagger/OpenAPI

## Learnings

- Containerized APIs must validate Keycloak tokens against the browser-visible issuer (`Auth:Authority`) while using a separate internal discovery URL (`Auth:MetadataAddress`) for backchannel metadata/JWKS fetches.
- For local Docker Keycloak, `--hostname=<public-url> --hostname-backchannel-dynamic=true` keeps admin/browser redirects on the public host without breaking backend token validation inside the Docker network.
- For Entra-in-prod plus Keycloak-in-dev, the backend should stay on plain ASP.NET Core `AddJwtBearer` with OIDC discovery and a provider-neutral claim-normalization layer; provider-specific server packages add coupling without helping Keycloak parity.
- Keep authorization stable by normalizing provider-specific role and tenant claims into `ClaimTypes.Role`, `tenant_id`, `tenant_identifier`, and `ClaimTypes.Name` before policies run; only the IdP configuration should vary between Entra and Keycloak.
