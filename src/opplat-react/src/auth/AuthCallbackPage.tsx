import React, { useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { RefreshCw } from 'lucide-react';
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
    <div className="flex justify-center items-center min-h-screen px-4">
      <div className="flex flex-col items-center gap-4 max-w-sm w-full">
        {showError ? (
          <>
            <div className="w-full bg-red-50 border border-red-300 text-red-700 px-4 py-3 rounded-md text-sm">
              {error.message}
            </div>
            <button
              onClick={handleRetry}
              className="flex items-center gap-2 bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 transition-colors"
            >
              <RefreshCw size={16} />
              Reintentar autenticación
            </button>
          </>
        ) : (
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
        )}
        <h2 className="text-lg font-semibold text-center">{title}</h2>
        <p className="text-sm text-gray-500 text-center">
          {showError
            ? 'Hubo un problema al validar tu sesión. Intenta nuevamente.'
            : 'Estamos validando tu sesión y recuperando el contexto del tenant.'}
        </p>
      </div>
    </div>
  );
};
