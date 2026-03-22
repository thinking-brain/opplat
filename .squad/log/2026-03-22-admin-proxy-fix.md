# Session Log: Admin Dev Proxy Runtime Fix

**Session Date:** 2026-03-22  
**Requested by:** elvis.crego  
**Participants:** Bishop (QA), Ripley (Architecture), Hudson (Infrastructure)  

---

## Summary

Diagnosed and fixed HTTP 500 error from `http://localhost:3201/admin/session/current-user` in admin auth flow.

### Problem

Admin dev stack reported HTTP 500 from `/admin/session/current-user` endpoint. Build succeeds, full auth integration test suite passes (56/56 tests green), backend auth code is sound.

### Root Cause (Ripley)

**Docker dev-mode proxy networking gap.** Not auth design or backend code.

- `docker-compose.override.yml` maps host:3201 → admin container:3001 (Vite dev server)
- Vite proxy fallback URL `http://localhost:8080` works in browser but fails in-container
- Inside container, `localhost:8080` = container itself; API runs at `api:8080` on Docker bridge
- Connection refused → Vite returns HTTP 500
- Production Dockerfile correctly proxies to `http://api:8080`; gap is dev-only

### Solution (Hudson)

1. Added `VITE_DEV_PROXY_TARGET=http://api:8080` to admin-frontend environment in `docker-compose.override.yml`
2. Aligned `src/opplat-admin/Dockerfile.dev` EXPOSE port to `3001`

### Validation (Bishop)

Pre-fix: Auth test suite already covered sparse-claim and full-claim session payloads (56/56 passing).  
Post-fix: Expected behavior — `/admin/session/current-user` returns `401` (unauthenticated) or `200` (authenticated), not `500`.

### Key Insight

Vite's `loadEnv()` merges `process.env` (Docker Compose vars) with `.env` files. The `VITE_API_URL` env var carries a browser-facing URL correct for browser but wrong for in-container proxy. The `VITE_DEV_PROXY_TARGET` env var exists to decouple these — must be set explicitly for Docker dev mode.

---

## Files Changed

- `docker-compose.override.yml` — Added VITE_DEV_PROXY_TARGET env var
- `src/opplat-admin/Dockerfile.dev` — Updated EXPOSE port

## Next Steps

Container restart validates fix. HTTP request testing confirms `/admin/session/current-user` endpoint behavior aligns with auth state (not 500).
