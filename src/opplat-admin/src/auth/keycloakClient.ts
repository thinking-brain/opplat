import Keycloak from 'keycloak-js';
import { appConfig } from '../runtimeConfig';

export const keycloak = new Keycloak({
  url: appConfig.keycloakUrl,
  realm: appConfig.keycloakRealm,
  clientId: 'opplat-admin',
});

// Singleton promise — keycloak.init() must only be called once.
// React StrictMode mounts components twice in development, which would
// otherwise call init() a second time, throwing "already initialized"
// and triggering an endless login redirect loop.
let _initPromise: Promise<boolean> | null = null;

export const initKeycloak = (): Promise<boolean> => {
  if (_initPromise === null) {
    _initPromise = keycloak.init({
      onLoad: 'check-sso',
      pkceMethod: 'S256',
      silentCheckSsoRedirectUri: `${window.location.origin}/silent-check-sso.html`,
    });
  }
  return _initPromise;
};
