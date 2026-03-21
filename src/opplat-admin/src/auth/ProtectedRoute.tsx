import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { Alert, Box, Button, CircularProgress, Stack, Typography } from '@mui/material';
import { useAuth } from './AuthContext';
import { hasAnyRole } from './roles';

interface ProtectedRouteProps {
  children: React.ReactNode;
  requiredRoles?: string[];
  unauthorizedTitle?: string;
  unauthorizedMessage?: string;
}

export const ProtectedRoute: React.FC<ProtectedRouteProps> = ({
  children,
  requiredRoles = [],
  unauthorizedTitle = 'Acceso restringido',
  unauthorizedMessage = 'No tienes permisos para abrir esta sección.',
}) => {
  const { isAuthenticated, loading, error, login, logout, roles } = useAuth();
  const location = useLocation();

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh">
        <CircularProgress />
      </Box>
    );
  }

  if (error && !isAuthenticated) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh" px={2}>
        <Stack spacing={2} maxWidth={480}>
          <Alert severity="error">{error.message}</Alert>
          <Button
            variant="contained"
            onClick={() => {
              void login(`${location.pathname}${location.search}${location.hash}`);
            }}
          >
            Reintentar autenticación
          </Button>
        </Stack>
      </Box>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }

  if (requiredRoles.length > 0 && !hasAnyRole(roles, requiredRoles)) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh" px={2}>
        <Stack spacing={2} maxWidth={560}>
          <Typography variant="h5">{unauthorizedTitle}</Typography>
          <Alert severity="warning">{unauthorizedMessage}</Alert>
          <Typography variant="body2" color="text.secondary">
            Roles actuales: {roles.length > 0 ? roles.join(', ') : 'sin roles asignados'}.
          </Typography>
          <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1.5}>
            <Button variant="contained" onClick={() => { void logout(); }}>
              Cerrar sesión
            </Button>
            <Button
              variant="outlined"
              onClick={() => {
                void login(`${location.pathname}${location.search}${location.hash}`);
              }}
            >
              Reintentar con otra cuenta
            </Button>
          </Stack>
        </Stack>
      </Box>
    );
  }

  return <>{children}</>;
};
