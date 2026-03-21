import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  CircularProgress,
  Container,
  Stack,
  Typography,
} from '@mui/material';
import { Login as LoginIcon, Shield as ShieldIcon } from '@mui/icons-material';
import { useAuth } from '../auth/AuthContext';
import { appConfig } from '../runtimeConfig';

interface LoginLocationState {
  from?: {
    pathname: string;
    search: string;
    hash: string;
  };
}

export const LoginPage: React.FC = () => {
  const { isAuthenticated, loading, login, error } = useAuth();
  const location = useLocation();
  const state = location.state as LoginLocationState | null;
  const returnTo = state?.from
    ? `${state.from.pathname}${state.from.search}${state.from.hash}`
    : '/';

  if (isAuthenticated) {
    return <Navigate to={returnTo} replace />;
  }

  return (
    <Container maxWidth="sm">
      <Box display="flex" alignItems="center" justifyContent="center" minHeight="100vh">
        <Card sx={{ width: '100%', maxWidth: 420 }}>
          <CardContent sx={{ p: 4 }}>
            <Stack spacing={3} alignItems="center" textAlign="center">
              <ShieldIcon color="primary" sx={{ fontSize: 56 }} />
              <Box>
                <Typography variant="h4" component="h1" gutterBottom>
                  {appConfig.appName}
                </Typography>
                <Typography variant="body1" color="text.secondary">
                  Inicia sesión con tu proveedor OIDC. Solo las cuentas con rol SuperAdmin pueden entrar
                  en este portal.
                </Typography>
              </Box>
              {error ? (
                <Alert severity="error" sx={{ width: '100%' }}>
                  {error.message}
                </Alert>
              ) : null}
              <Box sx={{ width: '100%', textAlign: 'left' }}>
                <Typography variant="subtitle2" color="text.secondary">
                  Authority
                </Typography>
                <Typography variant="body2">{appConfig.authAuthority}</Typography>
              </Box>
              <Button
                fullWidth
                size="large"
                variant="contained"
                disabled={loading}
                startIcon={loading ? <CircularProgress size={18} color="inherit" /> : <LoginIcon />}
                onClick={() => { void login(returnTo); }}
              >
                {loading ? 'Redirigiendo...' : 'Entrar con OIDC'}
              </Button>
            </Stack>
          </CardContent>
        </Card>
      </Box>
    </Container>
  );
};
