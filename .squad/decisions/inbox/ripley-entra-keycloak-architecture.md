## Decision

Standardize Opplat on a provider-neutral OIDC stack for both Azure Entra ID and Keycloak:

- backend bearer auth: `Microsoft.AspNetCore.Authentication.JwtBearer` with OIDC discovery (`Authority`, optional `MetadataAddress`, `Audience`)
- frontend SPA auth: `react-oidc-context` over `oidc-client-ts`
- two SPA public clients/apps in every provider (`opplat-client`, `opplat-admin`) with the same API audience/resource
- role normalization into the internal contract `SuperAdmin`, `TenantAdmin`, `TenantUser`
- application-owned tenant membership mapping, with token tenant claims treated as optional enrichment rather than the primary source of truth

## Why

Entra and Keycloak both speak standard OIDC/OAuth 2.0 for the flows Opplat uses. The current backend and both SPAs are already closest to the right seam: generic OIDC libraries plus claim normalization. Swapping to Entra-specific SDKs such as `msal-react` or `Microsoft.Identity.Web` would improve Entra ergonomics but would immediately create a provider split, increase conditional code, and weaken local-dev parity with Keycloak.

The cross-provider mismatch is not login protocol; it is business tenancy. Keycloak can easily emit `tenant_id` / `tenant_identifier` from user attributes, but Entra should not become the system of record for Opplat tenant assignment. Keeping tenant membership in Opplat avoids custom Entra claims policies, group-overage issues, and brittle provider-specific token shaping.

## Consequences

- Minimal provider-specific code: switch issuer/client config, not auth libraries.
- Production-ready bearer validation for both providers via discovery and JWKS.
- Keep using app/realm roles for authorization, but avoid using Entra groups as the tenant model.
- Preserve the current two-SPA separation for redirect URI isolation and browser-session isolation.
- If Keycloak still emits `tenant_id` / `tenant_identifier`, the backend can accept and normalize them, but production architecture should not require Entra to reproduce that exact token shape.
