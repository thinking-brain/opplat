---
name: "tenant-context-regression-tests"
description: "How to regression-test MainApp tenant-context endpoints that depend on Finbuckle tenant resolution"
domain: "testing"
confidence: "high"
source: "earned"
---

## Context

Use this when a MainApp endpoint depends on tenant resolution through Finbuckle and can succeed either from the resolved request tenant context or by falling back to the multitenant store using claims.

## Patterns

### Map the real minimal API endpoints in the test host

For MainApp auth integration tests, explicitly map `Opplat.Api.Main.Endpoints.AccountEndpoints.MapAccountEndpoints(app)` and any companion endpoint groups the assertions need. If you skip mapping, endpoint tests can fail as 404s and hide the real regression surface.

### Stub the abstraction, not the concrete store

Register `IMultiTenantStore<AppTenantInfo>` in the test host. MainApp should consume the Finbuckle abstraction, so the regression harness should mirror that contract instead of injecting `TenantCatalogStore` directly.

### Separate accessor state from store state

To prove claim-based fallback works, keep `IMultiTenantContextAccessor<AppTenantInfo>` unresolved while seeding `IMultiTenantStore<AppTenantInfo>` with a tenant. That exercises the exact edge case where middleware has no pre-resolved tenant but `/auth/account/tenant-context` must still recover from claims.

## Examples

- `test\Opplat.UnitTest\Auth\AuthEndpointAuthorizationIntegrationTests.cs`
- `test\Opplat.UnitTest\Architecture\MultitenancyConfigurationTests.cs`

## Anti-Patterns

- Treating 404 responses as auth/tenant regression evidence
- Injecting `TenantCatalogStore` directly in tests when production code depends on `IMultiTenantStore<AppTenantInfo>`
- Using the same fake object for both resolved tenant context and fallback store when the scenario under test requires one to be empty
