const runtimeEnv = typeof window !== 'undefined'
  ? window.__OPPLAT_RUNTIME_CONFIG__ ?? {}
  : {};

const readConfig = (key: keyof ImportMetaEnv, fallback: string): string => {
  const runtimeValue = runtimeEnv[key];
  const buildValue = import.meta.env[key];
  return runtimeValue?.trim() || buildValue?.trim() || fallback;
};

const trimTrailingSlash = (value: string): string => value.replace(/\/+$/, '');
const adminApiBaseUrl = trimTrailingSlash(readConfig('VITE_ADMIN_API_URL', ''));

export const appConfig = {
  appName: readConfig('VITE_APP_NAME', 'Opplat Admin'),
  adminApiUrl: adminApiBaseUrl,
  keycloakUrl: trimTrailingSlash(readConfig('VITE_KEYCLOAK_URL', 'http://localhost:8180')),
  keycloakRealm: readConfig('VITE_KEYCLOAK_REALM', 'opplat'),
};
