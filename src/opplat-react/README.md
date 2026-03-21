# Opplat Client App

Tenant-facing React 18 SPA for products, sales, inventory, and users.

## Highlights

- React 18 + Vite + TypeScript strict mode
- Material UI v5 and React Router v6
- Provider-agnostic OIDC with `react-oidc-context` / `oidc-client-ts`
- Tenant-aware API requests using route prefixing plus `X-Tenant-Identifier`
- Runtime env injection for Docker via `runtime-config.js`

## Environment

Copy `.env.example` to `.env.local` and adjust as needed:

```bash
cd src/opplat-react
copy .env.example .env.local
```

Key variables:

- `VITE_AUTH_AUTHORITY` - Auth0 or Keycloak authority URL
- `VITE_AUTH_CLIENT_ID` - SPA client id (`opplat-client` locally)
- `VITE_AUTH_AUDIENCE` - API audience/resource identifier
- `VITE_AUTH_API_URL` - Main API base URL
- `VITE_SALES_API_URL` - Sales API base URL
- `VITE_INVENTORY_API_URL` - Inventory API base URL

## Local development

```bash
npm install
npm run dev
```

The Vite server runs on `http://localhost:3000`.

## Docker runtime config

The container writes `/usr/share/nginx/html/runtime-config.js` at startup so compose-provided env vars are available without rebuilding the image.

## Auth flow

1. User lands on `/login`
2. App redirects to Keycloak/Auth0 using OIDC authorization code flow with PKCE
3. Tenant claims are normalized from Auth0 namespaced claims or Keycloak flat claims
4. The derived tenant identifier is persisted for request routing
5. Axios adds both `Authorization: Bearer ...` and `X-Tenant-Identifier` to tenant-scoped API calls
