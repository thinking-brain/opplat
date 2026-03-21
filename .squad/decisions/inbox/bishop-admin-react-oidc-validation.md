# Bishop inbox: admin react-oidc validation

- The existing frontend auth contract suite covered callback exit, recovered-session preference, and protected-route error handling, but it did **not** explicitly pin the two `react-oidc-context` configuration seams the upstream docs rely on: `onSigninCallback` being wired into `AuthProvider`, and `WebStorageStateStore` persisting the OIDC user in browser `localStorage`.
- Added focused contract coverage in `test/Opplat.MainApp.Test/Auth/FrontendAuthContractTests.cs` for both SPAs so future frontend changes cannot silently break callback payload cleanup or restored-session recovery while leaving the callback page tests green.
- Validation result: the current admin auth flow contract remains green against the smallest relevant suite (`FrontendAuthContractTests` + `KeycloakRealmContractTests`).
