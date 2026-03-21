# Hicks Decision: Entra + Keycloak Backend Token Validation

**Date:** 2026-03-21  
**Agent:** Hicks (Backend)  
**Status:** Proposed

## Decision

Use provider-neutral ASP.NET Core bearer validation based on `Microsoft.AspNetCore.Authentication.JwtBearer` and OpenID Connect discovery for both Azure Entra ID (production) and Keycloak (local development).

Do **not** adopt provider-specific backend auth packages as the default path for API token validation. Keep the API on standard JWT bearer middleware, then normalize claims in one backend transformation layer so authorization policies continue to target stable Opplat claim types and roles.

## Why

1. Both Entra and Keycloak publish standard OIDC metadata and JWKS endpoints, so standard `Authority` / optional `MetadataAddress` handling is enough for signature, issuer, and lifetime validation.
2. Provider-specific packages such as `Microsoft.Identity.Web` solve Entra-only convenience scenarios, but they introduce provider assumptions that do not help local Keycloak parity.
3. Opplat already has the right architectural seam: `Program.cs` configures `AddJwtBearer`, and `OidcClaimsTransformation` normalizes claims before authorization runs.

## Recommended backend shape

Keep one `Auth` section, but make it explicit that only provider settings change:

```json
"Auth": {
  "Provider": "Entra",
  "Authority": "https://login.microsoftonline.com/<tenant-id>/v2.0",
  "MetadataAddress": null,
  "Audience": "api://<entra-app-id-or-api-app-id>",
  "ValidIssuers": [
    "https://login.microsoftonline.com/<tenant-id>/v2.0",
    "https://sts.windows.net/<tenant-id>/"
  ],
  "ClaimNamespace": "https://opplat.com",
  "AdminRole": "SuperAdmin",
  "TenantAdminRole": "TenantAdmin",
  "TenantUserRole": "TenantUser",
  "AdditionalRoleClaimTypes": [
    "roles",
    "role",
    "groups",
    "realm_access.roles"
  ]
}
```

Keycloak local dev swaps only the provider-specific values:

```json
"Auth": {
  "Provider": "Keycloak",
  "Authority": "http://localhost:8180/realms/opplat",
  "MetadataAddress": null,
  "Audience": "opplat-api",
  "ValidIssuers": [
    "http://localhost:8180/realms/opplat"
  ],
  "ClaimNamespace": "https://opplat.com",
  "AdminRole": "SuperAdmin",
  "TenantAdminRole": "TenantAdmin",
  "TenantUserRole": "TenantUser",
  "AdditionalRoleClaimTypes": [
    "roles",
    "role",
    "realm_access.roles",
    "resource_access.opplat-api.roles"
  ]
}
```

## Provider differences to expect

- **Authority**
  - Entra: `https://login.microsoftonline.com/<tenant-id>/v2.0`
  - Keycloak: `http://localhost:8180/realms/opplat`
- **MetadataAddress**
  - Usually unnecessary for Entra
  - Useful for containerized Keycloak backchannel discovery
- **Audience**
  - Entra often emits `aud` as App ID URI or client/application ID
  - Keycloak typically uses a logical API audience like `opplat-api`
- **Issuer**
  - Entra can vary between v2 issuer and legacy STS issuer forms depending on token/version/config
  - Keycloak is usually stable and realm-specific
- **Roles**
  - Entra commonly uses flat `roles`; group-based auth may require `groups`
  - Keycloak may emit flat `realm_access.roles`, nested `realm_access`, or `resource_access.{client}.roles`
- **Scopes**
  - Entra may need downstream clients to request API scopes such as `api://.../access_as_user`
  - Keycloak usually relies on client scopes/mappers and audience mappers rather than Entra-style scope naming

## Claim normalization rule

Treat provider claims as inputs only. Backend authorization should continue to rely on normalized Opplat claims:

- `ClaimTypes.Role`
- `ClaimTypes.Name`
- `tenant_id`
- `tenant_identifier`

That keeps `AdminOnly`, `TenantAdminOnly`, tenant validation middleware, and future role policies independent of the active IdP.
