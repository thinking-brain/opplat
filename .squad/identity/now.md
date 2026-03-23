---
updated_at: 2026-03-23T10:18:10Z
focus_area: Evaluating and potentially flattening module application logic into the global Opplat.Application project while preserving module folder structure, MediatR wiring, and thin hosts.
active_issues: []
---

# What We're Focused On

Revisiting the application-layer structure after the refactor to determine whether module application logic should live under the global Opplat.Application project instead of separate module application projects. The goal is to keep module-specific folders for discoverability while preserving class-library boundaries, MediatR-driven use cases, thin hosts, and current route/auth/multitenancy behavior.
