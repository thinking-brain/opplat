# Session Log: Admin Auth Simplification — Approved Rollout

**Date:** 2026-03-22T10:31:12Z  
**Topic:** Admin auth simplification rollout  
**Status:** COMPLETE

## Summary
Admin auth contract simplified across backend, frontend, and tests. Tenant removed from auth boundary. Post-login redirect pinned to `localhost:3201`. All core guardrails locked in with executable tests.

## Key Decisions
1. **No tenant at auth level** — Admin authenticates as SuperAdmin. Tenant is operational context (headers/routes), not identity.
2. **DefaultOrigin fallback** — Admin redirects now reliably land on `http://localhost:3201` via explicit backend config.
3. **Tenant-free session DTO** — `AdminSessionUserDto` stripped of `TenantId` and `TenantIdentifier`.
4. **Tenant-free frontend auth context** — `AuthContext` and types no longer carry tenant state.
5. **Legacy client-id compat** — `Auth:ClientIdAdmin` env vars transparently mapped to `Auth:AdminBff:ClientId` for current deployments.
6. **Origin regression test** — Integration test pinned to guarantee post-login landing on `3201`, not `3001`.

## Acceptance Criteria ✅
- Session endpoint returns tenant-free user object
- Post-login redirects land on `http://localhost:3201`
- Frontend auth context exposes no tenant fields
- All tests pass with updated assertions
- Tenant CRUD endpoints still work with `X-Tenant-Identifier` header

## Future Work
- Make `opplat-admin` Keycloak client confidential + add secret
- Add tenant selector UI component (feature-level, not auth)
- Clean up dead docker-compose env vars
- Consider `/auth/bff/admin/switch-tenant` if session-level tenant context ever needed
