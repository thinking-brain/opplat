import React, { useEffect, useState } from 'react';
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Stack,
  Typography,
} from '@mui/material';
import { AuthContext, type AuthUser } from './AuthContext';
import { initKeycloak, keycloak } from './keycloakClient';

const SUPER_ADMIN_ROLE = 'SuperAdmin';

type AuthStatus = 'loading' | 'authenticated' | 'forbidden' | 'error';

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [status, setStatus] = useState<AuthStatus>('loading');
  const [user, setUser] = useState<AuthUser | null>(null);

  useEffect(() => {
    initKeycloak()
      .then((authenticated) => {
        if (!authenticated) {
          // Not authenticated — redirect to Keycloak login
          void keycloak.login({ redirectUri: window.location.href });
          return;
        }

        if (!keycloak.hasRealmRole(SUPER_ADMIN_ROLE)) {
          setStatus('forbidden');
          return;
        }

        const token = keycloak.idTokenParsed;
        setUser({
          name: token?.name ?? token?.preferred_username ?? 'Admin',
          email: token?.email ?? '',
          username: token?.preferred_username ?? '',
        });
        setStatus('authenticated');
      })
      .catch(() => {
        // init() failed for a non-auth reason — show error instead of
        // redirecting to login (which would create an infinite loop)
        setStatus('error');
      });
  }, []);

  if (status === 'loading') {
    return (
      <Box display="flex" alignItems="center" justifyContent="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }

  if (status === 'error') {
    return (
      <Box display="flex" alignItems="center" justifyContent="center" minHeight="100vh">
        <Stack spacing={2} alignItems="center" maxWidth={400}>
          <Typography variant="h5" fontWeight={700}>Error de autenticación</Typography>
          <Alert severity="error">
            No se pudo inicializar la sesión. Verifica que el servicio de autenticación esté disponible.
          </Alert>
          <Button variant="outlined" onClick={() => window.location.reload()}>
            Reintentar
          </Button>
        </Stack>
      </Box>
    );
  }

  if (status === 'forbidden') {
    return (
      <Box display="flex" alignItems="center" justifyContent="center" minHeight="100vh">
        <Stack spacing={2} alignItems="center" maxWidth={400}>
          <Typography variant="h5" fontWeight={700}>Acceso denegado</Typography>
          <Alert severity="error">
            Solo los usuarios con rol <strong>SuperAdmin</strong> pueden acceder a este portal.
          </Alert>
          <Button variant="outlined" onClick={() => void keycloak.logout({ redirectUri: window.location.origin })}>
            Cerrar sesión
          </Button>
        </Stack>
      </Box>
    );
  }

  const logout = (): void => {
    void keycloak.logout({ redirectUri: window.location.origin });
  };

  const getToken = async (): Promise<string> => {
    await keycloak.updateToken(30);
    return keycloak.token ?? '';
  };

  return (
    <AuthContext.Provider value={{ user: user!, logout, getToken }}>
      {children}
    </AuthContext.Provider>
  );
};
