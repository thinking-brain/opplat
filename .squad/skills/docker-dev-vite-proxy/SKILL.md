# SKILL: Docker Dev Mode Vite Proxy Configuration

## When to Apply
When running a Vite dev server inside a Docker container that needs to proxy API requests to another containerized service, and localhost resolution differs between host and container network contexts.

## Problem Pattern
- Vite dev server runs in container at port X (for Opplat admin, 3201)
- Docker Compose may map a different host/container port pair, or the app may run on a different host port outside Docker
- Dev server needs to proxy requests to an API service
- Hardcoding `localhost:8080` in proxy config breaks because:
  - Inside container, `localhost` = the container itself (127.0.0.1)
  - The API runs on a separate container reachable via Docker bridge network at `api:8080`
  - Result: ECONNREFUSED → proxy returns HTTP 500

## Solution

### 1. Vite Config: Support Environment-Based Proxy Target
Use `loadEnv()` to read an overridable environment variable:

```typescript
// vite.config.ts
import { defineConfig, loadEnv } from 'vite';

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '');
  
  // Fallback chain: explicit override → API URL → default
  const proxyTarget = env.VITE_DEV_PROXY_TARGET || env.VITE_API_URL || 'http://localhost:8080';
  
  const proxyOptions = {
    target: proxyTarget,
    changeOrigin: false,  // Preserve browser host for cookies/redirects
  };

  return {
    plugins: [react()],
    server: {
      port: 3201,
      proxy: {
        '/api': proxyOptions,
        '/auth': proxyOptions,
        // ... other endpoints
      },
    },
  };
});
```

**Key:** `changeOrigin: false` is critical for OIDC auth/BFF login — it preserves the browser's origin so cookies and redirect URIs stay on the SPA origin (for admin, `localhost:3201`), not the proxy target.

### 2. Docker Compose Override: Container-Aware Network Resolution
Set the environment variable to use Docker's internal DNS for the API container:

```yaml
# docker-compose.override.yml
services:
  admin-frontend:
    build:
      context: src/opplat-admin
      dockerfile: Dockerfile.dev
    volumes:
      - ./src/opplat-admin:/app
    ports:
      - "3201:3201"
    environment:
      # For Docker dev: proxy to api service on bridge network
      - VITE_DEV_PROXY_TARGET=http://api:8080
      # For local/non-Docker dev: fallback to localhost
      - VITE_API_URL=http://localhost:8080
```

### 3. Network Behavior
| Context | Proxy Target | Resolution |
|---------|--------------|-----------|
| Host machine (non-Docker) | `localhost:8080` | Host API (if running locally) |
| Docker dev (override) | `api:8080` | api container on opplat-network bridge |
| Production (nginx) | `http://api:8080` | Baked into Dockerfile; no env needed |

### 4. Validation
```bash
# Verify syntax
docker-compose config --quiet

# Test after deploy
curl -i http://localhost:3201/admin/session/current-user
# Expected: 401 (unauthenticated) or 200 (authenticated)
# NOT 500 or ECONNREFUSED
```

## Related Patterns
- **OIDC Auth + Finbuckle MultiTenant:** Keep `changeOrigin: false` so cookie domain and OIDC callback redirects land on SPA origin
- **Same-port local auth:** Prefer using the same internal/external port for dev auth surfaces when possible; it reduces wrong-origin redirect mistakes during login debugging
- **Production nginx:** Bake the proxy target into the Dockerfile (static), no env override needed
- **Local development:** Skip Docker; run Vite on host with `localhost:8080` fallback

## Anti-Pattern
❌ **Hardcoded proxy targets:** `target: 'http://localhost:8080'` breaks in Docker unless the API runs on the host at that exact port  
❌ **changeOrigin: true for OIDC:** Causes cookies and redirects to proxy target origin instead of SPA origin  
❌ **Per-service proxy configs:** Duplicate proxy rules for each endpoint instead of reusing a parameterized `proxyOptions` object

## Trade-Off
- Requires environment variable in docker-compose.override.yml
- Adds one more fallback in vite.config.ts
- Benefit: Same code path works for Docker dev (bridge network) and local host dev (localhost fallback)
