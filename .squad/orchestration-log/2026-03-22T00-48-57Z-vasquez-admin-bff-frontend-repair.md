### 2026-03-22T00-48-57Z — Vasquez Frontend Path & CSRF Repair

| Field | Value |
|-------|-------|
| **Agent routed** | Vasquez (Frontend) |
| **Why chosen** | Locked out by Ripley defects 1–4: route mismatch, query param mismatch, CSRF header hardcoding, CSRF token acquisition |
| **Mode** | `sync` |
| **Why this mode** | Blocking defect fixes required before re-review; Vasquez coordinates with Hicks/Hudson on canonical paths |
| **Files authorized to read** | `src/opplat-admin/src/auth/bff.ts`, `src/opplat-admin/src/auth/AuthContext.tsx`, backend route definitions, Vite config, Ripley's defect analysis |
| **File(s) agent must produce** | Updated `bff.ts` with canonical paths, updated `AuthContext.tsx` to fetch CSRF dynamically, `axiosClient.ts` to use backend header name |
| **Outcome** | Completed — All frontend BFF calls align with backend routes; CSRF token fetched from `/admin/session/csrf` and sent with backend-provided header name |
