import React from 'react';
import { Navigate, useLocation, useNavigate } from 'react-router-dom';
import { LogIn, Shield } from 'lucide-react';
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
  const navigate = useNavigate();
  const state = location.state as LoginLocationState | null;
  const returnTo = state?.from
    ? `${state.from.pathname}${state.from.search}${state.from.hash}`
    : '/';

  if (isAuthenticated) {
    return <Navigate to={returnTo} replace />;
  }

  return (
    <div className="flex items-center justify-center min-h-screen px-4 bg-gray-50">
      <div className="card p-8 w-full max-w-sm">
        <div className="flex flex-col items-center gap-4 text-center">
          <Shield size={56} className="text-blue-600" />
          <div>
            <h1 className="text-2xl font-bold">{appConfig.appName}</h1>
            <p className="text-sm text-gray-500 mt-1">
              Inicia sesión con tu proveedor OIDC. El tenant se deduce automáticamente desde tus claims.
            </p>
          </div>
          {error && (
            <div className="w-full bg-red-50 border border-red-300 text-red-700 px-4 py-3 rounded-md text-sm">
              {error.message}
            </div>
          )}
          <div className="w-full text-left">
            <p className="text-xs text-gray-500 font-medium">Authority</p>
            <p className="text-sm text-gray-700">{appConfig.authAuthority}</p>
          </div>
          <button
            className="btn-primary w-full flex items-center justify-center gap-2 py-3 text-base"
            disabled={loading}
            onClick={() => { void login(returnTo); }}
          >
            {loading ? (
              <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white" />
            ) : (
              <LogIn size={18} />
            )}
            {loading ? 'Redirigiendo...' : 'Entrar con OIDC'}
          </button>
          <div className="w-full flex items-center gap-3">
            <div className="flex-1 border-t border-gray-200" />
            <span className="text-xs text-gray-400">Don't have an account?</span>
            <div className="flex-1 border-t border-gray-200" />
          </div>
          <button
            className="btn-secondary w-full py-3"
            onClick={() => navigate('/register')}
          >
            Create Account
          </button>
        </div>
      </div>
    </div>
  );
};
