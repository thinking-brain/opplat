---
updated_at: 2026-03-22T10:31:12Z
focus_area: Simplified admin BFF auth at localhost:3201; tenant-free admin session; tenant-aware admin management endpoints
active_issues: []
---

# What We're Focused On

Admin auth simplification complete: tenant removed from auth boundary, post-login redirects pinned to localhost:3201, admin session contract simplified, frontend auth context cleanened up. Next phase: tenant selector UI (feature-level), Keycloak client hardening (confidential + secret), dead env var cleanup in docker-compose.
