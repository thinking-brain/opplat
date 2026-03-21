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

  useEffect(() => {
    if (isAuthenticated && !loading && !hasRedirected.current) {
      hasRedirected.current = true;
      navigate('/', { replace: true });
      const fallbackTimer = setTimeout(() => {
        window.location.replace('/');
      }, 100);
      return () => clearTimeout(fallbackTimer);
    }
  }, [isAuthenticated, loading, navigate]);

  if (isAuthenticated && !loading) {
    return null;
  }

  const handleRetry = () => {
    void login('/');
  };

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
            : 'Estamos validando tu sesión y recuperando el contexto del tenant.'}
        </Typography>
      </Stack>
    </Box>
  );
};
