import { SUPER_ADMIN_ROLE } from './auth/roles';

const runtimeEnv = typeof window !== 'undefined'
  ? window.__OPPLAT_RUNTIME_CONFIG__ ?? {}
  : {};

const readConfig = (key: keyof ImportMetaEnv, fallback: string): string => {
  const runtimeValue = runtimeEnv[key];
  const buildValue = import.meta.env[key];
  return runtimeValue?.trim() || buildValue?.trim() || fallback;
};

const readBooleanConfig = (key: keyof ImportMetaEnv, fallback: boolean): boolean => {
  const configuredValue = readConfig(key, '').toLowerCase();
  if (configuredValue === 'true') {
    return true;
  }

  if (configuredValue === 'false') {
    return false;
  }

  return fallback;
};

const ensurePath = (value: string, fallback: string): string => {
  const candidate = value.trim() || fallback;
  return candidate.startsWith('/') ? candidate : `/${candidate}`;
};

const splitConfigList = (value: string, fallback: string[]): string[] => {
  const configuredValues = value
    .split(/[\s,]+/)
    .map((item) => item.trim())
    .filter(Boolean);

  return [...new Set([...fallback, ...configuredValues])];
};

const authAuthority = readConfig('VITE_AUTH_AUTHORITY', 'http://localhost:8180/realms/opplat');
const authAudience = readConfig('VITE_AUTH_AUDIENCE', 'opplat-api');
const authScopes = splitConfigList(readConfig('VITE_AUTH_SCOPE', 'openid profile email offline_access'), ['openid']);
const isKeycloakAuthority = /\/realms\/[^/]+$/i.test(authAuthority);
const isEntraAuthority = /:\/\/(?:[^/]+\.)?(login\.microsoftonline\.com|ciamlogin\.com)\//i.test(authAuthority);
const defaultUseAudienceQueryParam = authAudience.length > 0 && !isKeycloakAuthority && !isEntraAuthority;

export const appConfig = {
  appName: readConfig('VITE_APP_NAME', 'Opplat Admin'),
  apiUrl: readConfig('VITE_API_URL', 'http://localhost:8080'),
  adminApiUrl: readConfig('VITE_ADMIN_API_URL', readConfig('VITE_API_URL', 'http://localhost:8080')),
  authAuthority,
  authClientId: readConfig('VITE_AUTH_CLIENT_ID', 'opplat-admin'),
  authAudience,
  authScope: authScopes.join(' '),
  authScopes,
  useAudienceQueryParam: readBooleanConfig('VITE_AUTH_USE_AUDIENCE_QUERY_PARAM', defaultUseAudienceQueryParam),
  authRedirectPath: ensurePath(readConfig('VITE_AUTH_REDIRECT_PATH', '/auth/callback'), '/auth/callback'),
  authSilentRedirectPath: ensurePath(readConfig('VITE_AUTH_SILENT_REDIRECT_PATH', '/auth/silent-renew'), '/auth/silent-renew'),
  authLogoutRedirectPath: ensurePath(readConfig('VITE_AUTH_LOGOUT_REDIRECT_PATH', '/login'), '/login'),
  accessControl: {
    adminPortalRoles: [SUPER_ADMIN_ROLE],
  },
};

export const buildBrowserUrl = (path: string): string => new URL(path, window.location.origin).toString();
