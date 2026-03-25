# SKILL: Identity Provider Regression Gates

## When to Apply
When testing Entra/Graph integration seams without calling live Azure services.

## Pattern

### Graph client tests
Prefer the concrete transport seam the production code already uses.

- If the service wraps `HttpClient`, use a custom `HttpMessageHandler` to capture method, URI, headers, body, and retry attempts.
- Use scripted response sequences to prove 429/503 retry behavior and final error mapping.
- Assert request shape for create-user, enable/disable, delete, and password-reset payloads directly from serialized JSON.

### Session contract tests
Use `TestServer` auth handlers to exercise both bearer and cookie flows.

- Register a bearer test scheme under `JwtBearerDefaults.AuthenticationScheme` when the endpoint explicitly calls `AuthenticateAsync("Bearer")`.
- Lock the stable identity surface (`userId` / `objectId`) against Entra `oid` claims.
- Assert whether `accessToken` is present or null for each auth mode instead of assuming one universal behavior.

### Claim normalization tests
Keep provider-neutral normalization tests separate from session tests.

- Assert `sub -> oid` backfill when `oid` is missing.
- Assert an existing `oid` is preserved without duplicates.

## Validation
Run a focused filter over the identity-related test classes after changes, for example:

`dotnet test test\Opplat.MainApp.Test\Opplat.MainApp.Test.csproj --filter "FullyQualifiedName~GraphUserServiceTests|FullyQualifiedName~AdminApiSessionEndpointTests|FullyQualifiedName~OidcClaimsTransformationTests"`
