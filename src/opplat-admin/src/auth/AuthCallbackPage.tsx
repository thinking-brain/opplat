import React, { useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { Alert, Box, Button, CircularProgress, Stack, Typography } from '@mui/material';
import { Refresh as RefreshIcon } from '@mui/icons-material';
import { useAuth } from './AuthContext';

interface AuthCallbackPageProps {
  title: string;
}

export const AuthCallbackPage: React.FC<AuthCallbackPageProps> = ({ title }) => {
  const { error, isAuthenticated, loading, login } = useAuth();
  const navigate = useNavigate();
  const hasRedirected = useRef(false);

  // Redirect authenticated users away from callback page
  useEffect(() => {
    if (isAuthenticated && !loading && !hasRedirected.current) {
      hasRedirected.current = true;
      // Use navigate for React Router integration, then hard redirect as fallback
      navigate('/', { replace: true });
      // Fallback: if navigate doesn't work within 100ms, force hard redirect
      const fallbackTimer = setTimeout(() => {
        window.location.replace('/');
      }, 100);
      return () => clearTimeout(fallbackTimer);
    }
  }, [isAuthenticated, loading, navigate]);

  // Already authenticated - show nothing while redirecting
  if (isAuthenticated && !loading) {
    return null;
  }

  const handleRetry = () => {
    // Clear any stale OIDC state and retry login
    void login('/');
  };

  // Show error with retry option when auth fails
  const showError = error && !isAuthenticated && !loading;

  return (
    <Box display="flex" justifyContent="center" alignItems="center" minHeight="100vh" px={2}>
      <Stack spacing={2} alignItems="center" maxWidth={420}>
        {showError ? (
          <>
            <Alert severity="error" sx={{ width: '100%' }}>
              {error.message}
            </Alert>
            <Button
              variant="contained"
              startIcon={<RefreshIcon />}
              onClick={handleRetry}
            >
              Reintentar autenticación
            </Button>
          </>
        ) : (
          <CircularProgress />
        )}
        <Typography variant="h6" align="center">
          {title}
        </Typography>
        <Typography variant="body2" color="text.secondary" align="center">
          {showError 
            ? 'Hubo un problema al validar tu sesión. Intenta nuevamente.'
            : 'Validando permisos y recuperando la sesión administrativa.'}
        </Typography>
      </Stack>
    </Box>
  );
};
