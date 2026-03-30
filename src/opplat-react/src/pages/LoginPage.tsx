import React from 'react';
import { Link as RouterLink, Navigate, useLocation } from 'react-router-dom';
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  CircularProgress,
  Container,
  Link,
  Stack,
  Typography,
} from '@mui/material';
import { Login as LoginIcon, Security as SecurityIcon } from '@mui/icons-material';
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
        <Card sx={{ width: '100%', maxWidth: 400 }}>
          <CardContent sx={{ p: 4 }}>
            <Stack spacing={3} alignItems="center" textAlign="center">
              <SecurityIcon color="primary" sx={{ fontSize: 56 }} />
              <Box>
                <Typography variant="h4" component="h1" gutterBottom>
                  {appConfig.appName}
                </Typography>
                <Typography variant="body1" color="text.secondary">
                  Inicia sesión con tu proveedor OIDC. El tenant se deduce automáticamente desde tus claims.
                </Typography>
              </Box>
              {error && <Alert severity="error" sx={{ width: '100%' }}>{error.message}</Alert>}
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
              <Typography variant="body2" color="text.secondary">
                Don't have an account?{' '}
                <Link component={RouterLink} to="/register">
                  Register
                </Link>
              </Typography>
            </Stack>
          </CardContent>
        </Card>
      </Box>
    </Container>
  );
};
