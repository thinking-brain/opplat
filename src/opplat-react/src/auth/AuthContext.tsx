import React, { createContext, useContext, useEffect, useMemo, type ReactNode } from 'react';
import { AuthProvider as OidcProvider, useAuth as useOidcAuth } from 'react-oidc-context';
import { useLocation } from 'react-router-dom';
import type { User } from '../types';
import { appConfig } from '../runtimeConfig';
import { buildAppUser, getTenantIdentifierFromUser, persistTenantIdentifier } from './claims';
import { oidcUserManager, onSigninCallback } from './oidc';

interface AuthContextType {
  user: User | null;
  token: string | null;
  tenantId: string | null;
  tenantIdentifier: string | null;
  roles: string[];
  isAuthenticated: boolean;
  login: (returnTo?: string) => Promise<void>;
  logout: () => Promise<void>;
  loading: boolean;
  error: Error | null;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

const InnerAuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const oidc = useOidcAuth();
  const location = useLocation();

  const user = useMemo(() => (oidc.user ? buildAppUser(oidc.user) : null), [oidc.user]);
  const tenantIdentifier = user?.tenantIdentifier ?? getTenantIdentifierFromUser(oidc.user) ?? null;
  const tenantId = user?.tenantId ?? null;

  useEffect(() => {
    persistTenantIdentifier(tenantIdentifier);
  }, [tenantIdentifier]);

  const login = async (returnTo?: string): Promise<void> => {
    const target = returnTo ?? `${location.pathname}${location.search}${location.hash}`;
    await oidc.signinRedirect({ state: { returnTo: target } });
  };

  const logout = async (): Promise<void> => {
    persistTenantIdentifier(null);

    try {
      if (oidc.user) {
        await oidc.signoutRedirect();
        return;
      }
    } catch {
      // Fall back to a local sign-out when the provider does not expose end_session_endpoint.
    }

    await oidc.removeUser();
    window.location.assign(appConfig.authLogoutRedirectPath);
  };

  const hasResolvedUser = Boolean(oidc.user && !oidc.user.expired);
  const isAuthenticated = oidc.isAuthenticated || hasResolvedUser;
  const isNavigating = Boolean(oidc.activeNavigator) && !isAuthenticated;

  const value = {
    user,
    token: oidc.user?.access_token ?? null,
    tenantId,
    tenantIdentifier,
    roles: user?.roles ?? [],
    isAuthenticated,
    login,
    logout,
    loading: oidc.isLoading || isNavigating,
    error: oidc.error ?? null,
  } satisfies AuthContextType;

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => (
  <OidcProvider userManager={oidcUserManager} onSigninCallback={onSigninCallback}>
    <InnerAuthProvider>{children}</InnerAuthProvider>
  </OidcProvider>
);
