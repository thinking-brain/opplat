import { User, WebStorageStateStore, type UserManagerSettings } from 'oidc-client-ts';
import { appConfig, buildBrowserUrl } from '../runtimeConfig';

const requiredScopes = ['openid'] as const;
const oidcUserStorageKey = `oidc.user:${appConfig.authAuthority}:${appConfig.authClientId}`;

const buildScope = (configuredScope: string): string => {
  const scopes = new Set(configuredScope.split(/\s+/).filter(Boolean));

  requiredScopes.forEach((scope) => scopes.add(scope));

  return Array.from(scopes).join(' ');
};

const readString = (value: unknown): string | null =>
  typeof value === 'string' && value.trim().length > 0 ? value : null;

const clearStaleOidcEntries = (storage: Storage): void => {
  const keys = Array.from({ length: storage.length }, (_, index) => storage.key(index)).filter(
    (key): key is string => Boolean(key),
  );

  keys.forEach((key) => {
    if (key.startsWith('oidc.user:') && key !== oidcUserStorageKey) {
      storage.removeItem(key);
      return;
    }

    if (!key.startsWith('oidc.') || key.startsWith('oidc.user:')) {
      return;
    }

    const rawValue = storage.getItem(key);
    if (!rawValue) {
      return;
    }

    try {
      const parsedValue = JSON.parse(rawValue) as Record<string, unknown>;
      const authority = readString(parsedValue.authority);
      const clientId = readString(parsedValue.client_id) ?? readString(parsedValue.clientId);

      const mismatchedAuthority = authority !== null && authority !== appConfig.authAuthority;
      const mismatchedClient = clientId !== null && clientId !== appConfig.authClientId;

      if (mismatchedAuthority || mismatchedClient) {
        storage.removeItem(key);
      }
    } catch {
      // Ignore non-JSON OIDC entries; they are not state objects we can safely classify.
    }
  });
};

const clearStaleOidcStorage = (): void => {
  if (typeof globalThis === 'undefined') {
    return;
  }

  clearStaleOidcEntries(globalThis.localStorage);
  clearStaleOidcEntries(globalThis.sessionStorage);
};

clearStaleOidcStorage();

export const oidcSettings: UserManagerSettings = {
  authority: appConfig.authAuthority,
  client_id: appConfig.authClientId,
  redirect_uri: buildBrowserUrl(appConfig.authRedirectPath),
  silent_redirect_uri: buildBrowserUrl(appConfig.authSilentRedirectPath),
  post_logout_redirect_uri: buildBrowserUrl(appConfig.authLogoutRedirectPath),
  response_type: 'code',
  scope: buildScope(appConfig.authScope),
  loadUserInfo: false,
  automaticSilentRenew: true,
  monitorSession: true,
  userStore: new WebStorageStateStore({ store: globalThis.localStorage }),
  ...(appConfig.useAudienceQueryParam ? { extraQueryParams: { audience: appConfig.authAudience } } : {}),
};

const getReturnTo = (user?: User | null): string => {
  const state = user?.state;
  if (state && typeof state === 'object' && 'returnTo' in state) {
    const returnTo = (state as { returnTo?: unknown }).returnTo;
    if (typeof returnTo === 'string' && returnTo.startsWith('/')) {
      return returnTo;
    }
  }

  return '/';
};

export const onSigninCallback = (user?: User): void => {
  if (globalThis.self !== globalThis.top) {
    return;
  }

  globalThis.location.replace(getReturnTo(user));
};

const readStorage = (): Storage | null => {
  if (typeof globalThis === 'undefined') {
    return null;
  }

  return globalThis.localStorage;
};

export const getOidcUser = async (): Promise<User | null> => {
  const storage = readStorage();
  const serializedUser = storage?.getItem(oidcUserStorageKey);
  if (!serializedUser) {
    return null;
  }

  try {
    return User.fromStorageString(serializedUser);
  } catch {
    storage?.removeItem(oidcUserStorageKey);
    return null;
  }
};

export const removeOidcUser = async (): Promise<void> => {
  readStorage()?.removeItem(oidcUserStorageKey);
};
