---
name: "frontend-runtime-config"
description: "Conventions for SPA service URL configuration in Opplat"
domain: "frontend"
confidence: "high"
source: "Carl QA analysis, 2026-04-15"
---

## Context

Opplat runs two React/Vite SPAs under Aspire and also supports standalone local runs. URL config is easy to break because each app can read values from runtime injection, Vite env, and hardcoded fallbacks.

## Patterns

### Resolve service URLs once per SPA

Each SPA should expose a small, explicit `appConfig` surface with the final resolved base URLs it actually needs. Avoid nested fallback chains that make one service silently inherit another service's port.

### Prefer Aspire-derived values for Aspire runs

When the app runs under Aspire, `src\Opplat.AppHost\Program.cs` should provide the frontend env values from the real backend resource endpoints/ports rather than duplicated localhost literals. If a backend port changes in Aspire, the SPA config should follow automatically.

Use Aspire `GetEndpoint(...)` references as the handoff point. That same pattern should also drive related backend browser-origin settings (for example Admin BFF allowed origins) so frontend ports and backend allowlists cannot drift apart.

### Make same-origin mode explicit

For the admin app, same-origin proxy mode is valid, but it should be represented clearly in config/code. Do not rely on an unexplained empty-string URL convention unless the consuming code treats that as a named, intentional mode.

### Keep runtime override precedence stable

`window.__OPPLAT_RUNTIME_CONFIG__` is the runtime override layer and should remain the highest-precedence source when present. Build-time `import.meta.env` should be the fallback, not the other way around.

## QA Checks

- Client SPA can load subscription plans and post registration under Aspire
- Client SPA auth redirects still point at the expected browser origin
- Admin SPA can call `/admin/*` through same-origin proxy mode
- Admin SPA also works when given an explicit absolute admin API base URL
- Port changes in Aspire do not require manual SPA fallback edits

## Anti-Patterns

- Hardcoding one port in Aspire and a different fallback port in the SPA for the same backend
- Letting unrelated services inherit a shared `VITE_API_URL` implicitly
- Hiding same-origin behavior behind undocumented empty strings
