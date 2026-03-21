import { UserManager, WebStorageStateStore, type User, type UserManagerSettings } from 'oidc-client-ts';
import { appConfig, buildBrowserUrl } from '../runtimeConfig';

const requiredScopes = ['openid'] as const;

const buildScope = (configuredScope: string): string => {
  const scopes = new Set(configuredScope.split(/\s+/).filter(Boolean));

  requiredScopes.forEach((scope) => scopes.add(scope));

  return Array.from(scopes).join(' ');
};

const oidcSettings: UserManagerSettings = {
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
  userStore: new WebStorageStateStore({ store: window.localStorage }),
  ...(appConfig.useAudienceQueryParam ? { extraQueryParams: { audience: appConfig.authAudience } } : {}),
};

export const oidcUserManager = new UserManager(oidcSettings);

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
  if (window.self !== window.top) {
    return;
  }

  window.location.replace(getReturnTo(user));
};

export const getOidcUser = async (): Promise<User | null> => oidcUserManager.getUser();

export const removeOidcUser = async (): Promise<void> => {
  await oidcUserManager.removeUser();
};
