# SKILL: Convert a React admin SPA into a non-authenticated shell

## When to apply

Use this when a frontend auth integration is blocking delivery and the team needs the React app to remain usable while auth is handled manually or externally.

## Goal

Remove SPA-owned auth flows without breaking navigation, layout, or existing admin API integration.

## Checklist

1. Remove auth providers, guards, login pages, callback routes, and logout UI.
2. Repoint the route tree at normal app pages so the shell remains navigable.
3. Strip Axios request/response auth helpers such as session restore, CSRF bootstrap, and forced login redirects.
4. Keep neutral client state that is still useful without auth, such as selected tenant filters.
5. Remove auth-specific Vite/Nginx proxy paths and runtime env variables.
6. Update README and deployment config so the shell clearly documents that auth is external.
7. Validate with the app's existing lint/build commands.

## Opplat admin specifics

- Keep `/admin/*` proxying in Vite and Nginx.
- Preserve `withCredentials` on the admin Axios client so manual cookie-based auth can still work when needed.
- Keep tenant selection in localStorage because it supports admin UX independently of session state.
