import React, { useEffect, useState } from 'react';
import { Alert, Box, Button, CircularProgress, Stack, Typography } from '@mui/material';
import { authApi } from '../api/auth.api';
import type { TenantAccessContext } from '../types';
import { persistTenantIdentifier } from './claims';
import { useAuth } from './AuthContext';

interface TenantAccessGateProps {
  children: React.ReactNode;
}

const getErrorMessage = (error: unknown): string =>
  error instanceof Error ? error.message : 'No se pudo resolver el tenant de la sesión actual.';

export const TenantAccessGate: React.FC<TenantAccessGateProps> = ({ children }) => {
  const { isAuthenticated, logout } = useAuth();
  const [tenantContext, setTenantContext] = useState<TenantAccessContext | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;

    const loadTenantContext = async (): Promise<void> => {
      if (!isAuthenticated) {
        if (active) {
          setTenantContext(null);
          setLoading(false);
          setError(null);
        }
        return;
      }

      setLoading(true);
      setError(null);

      try {
        const response = await authApi.getTenantContext();
        if (!active) {
          return;
        }

        if (response.tenantIdentifier) {
          persistTenantIdentifier(response.tenantIdentifier);
        } else if (!response.isResolved) {
          persistTenantIdentifier(null);
        }

        setTenantContext(response);
      } catch (loadError) {
        if (!active) {
          return;
        }

        setError(getErrorMessage(loadError));
        setTenantContext(null);
      } finally {
        if (active) {
          setLoading(false);
        }
      }
    };

    void loadTenantContext();

    return () => {
      active = false;
    };
  }, [isAuthenticated]);

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh" px={2}>
        <Stack spacing={2} maxWidth={560}>
          <Typography variant="h5">No se pudo validar el tenant</Typography>
          <Alert severity="error">{error}</Alert>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5}>
            <Button variant="contained" onClick={() => { window.location.reload(); }}>
              Reintentar
            </Button>
            <Button variant="outlined" onClick={() => { void logout(); }}>
              Cerrar sesión
            </Button>
          </Stack>
        </Stack>
      </Box>
    );
  }

  if (tenantContext && !tenantContext.isResolved) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh" px={2}>
        <Stack spacing={2} maxWidth={560}>
          <Typography variant="h5">Tu usuario no tiene tenant asignado</Typography>
          <Alert severity="warning">{tenantContext.message}</Alert>
          <Typography color="text.secondary">
            Necesitas una asignacion de tenant antes de poder usar la aplicacion.
          </Typography>
          <Button variant="outlined" onClick={() => { void logout(); }}>
            Cerrar sesión
          </Button>
        </Stack>
      </Box>
    );
  }

  if (tenantContext && !tenantContext.isActive) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh" px={2}>
        <Stack spacing={2} maxWidth={560}>
          <Typography variant="h5">Tenant inactivo</Typography>
          <Alert severity="warning">{tenantContext.message}</Alert>
          <Typography color="text.secondary">
            {tenantContext.tenantName ?? tenantContext.tenantIdentifier ?? 'Tu tenant'} esta temporalmente inactivo.
            Contacta al administrador principal para reactivar el acceso.
          </Typography>
          <Button variant="outlined" onClick={() => { void logout(); }}>
            Cerrar sesión
          </Button>
        </Stack>
      </Box>
    );
  }

  return <>{children}</>;
};
