### 2026-03-22T00-48-57Z — Hudson Vite Proxy Repair

| Field | Value |
|-------|-------|
| **Agent routed** | Hudson (DevOps/Infra) |
| **Why chosen** | Vite proxy configuration missing `/auth/bff/admin/` and OIDC callback paths; cookie/origin alignment required for dev |
| **Mode** | `sync` |
| **Why this mode** | Repair is blocking Vasquez's frontend work and Ripley's re-review |
| **Files authorized to read** | `src/opplat-admin/vite.config.ts`, `src/opplat-admin/.env.example`, Ripley's defect analysis |
| **File(s) agent must produce** | Updated Vite proxy config covering `/admin`, `/auth`, `/signin-oidc-admin`, `/signout-callback-oidc-admin` with `changeOrigin: false` for cookie scope |
| **Outcome** | Completed — Vite proxy now covers all BFF and OIDC callback paths with correct origin preservation |
