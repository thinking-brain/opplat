---
updated_at: 2026-03-23T00:04:41Z
focus_area: Admin API boundary fully refactored; admin owns tenant catalog only, not users. Users belong to tenant-owned databases. Subscription tracking via MaxUsers/CurrentUserCount. Connection strings factored to DatabaseName+SchemaName. All validation passed; ready for next phase (tenant selector UI, Keycloak hardening, dead env var cleanup, user count sync pattern).
active_issues: []
---

# What We're Focused On

Admin API boundary refactor complete: removed admin-owned user CRUD entirely; users now belong to tenant-owned databases with admin tracking only subscription-relevant metadata (MaxUsers, CurrentUserCount). Tenant records store DatabaseName/SchemaName instead of full connection strings (credentials resolved at runtime via vault/config). AdminTenantInfo schema updated; all user DTOs, handlers, endpoints removed. Frontend aligned: removed UsersPage, updated tenant forms, updated types. Backend compiles, all 65 tests pass, frontend builds/lints clean, docker-compose valid. Next: tenant selector UI for client apps, Keycloak client hardening (confidential + secret), dead env var cleanup from docker-compose, implement user count sync (tenant → admin callback or periodic polling).


