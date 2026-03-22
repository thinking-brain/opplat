---
updated_at: 2026-03-22T22:27:00Z
focus_area: Admin API MediatR + PostgreSQL migration complete; auth/session routes frozen; ready for next phase (tenant selector UI, Keycloak hardening, dead env var cleanup)
active_issues: []
---

# What We're Focused On

Admin API persistence modernization is complete: replaced in-memory AdminPortalStore with MediatR handlers backed by PostgreSQL-isolated AdminTenantIdentityDbContext. Auth pipeline (cookie/OIDC/JWT) and endpoint contracts (/admin/* routes) explicitly frozen. Separate postgres container for admin-api; Main/Sales/Inventory remain on SQL Server. All tests pass (65/65 backend/integration), docker-compose valid, builds green. Next: tenant selector UI (feature-level), Keycloak client hardening (confidential + secret), dead env var cleanup in docker-compose.

