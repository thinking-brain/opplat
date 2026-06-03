import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
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
      <div className="flex justify-center items-center min-h-screen">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
      </div>
    );
  }

  if (error && !isAuthenticated) {
    return (
      <div className="flex justify-center items-center min-h-screen px-4">
        <div className="flex flex-col gap-3 max-w-md w-full">
          <div className="bg-red-50 border border-red-300 text-red-700 px-4 py-3 rounded-md text-sm">
            {error.message}
          </div>
          <button
            className="btn-primary"
            onClick={() => {
              void login(`${location.pathname}${location.search}${location.hash}`);
            }}
          >
            Reintentar autenticación
          </button>
        </div>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location }} />;
  }

  if (requiredRoles.length > 0 && !hasAnyRole(roles, requiredRoles)) {
    return (
      <div className="flex justify-center items-center min-h-screen px-4">
        <div className="flex flex-col gap-3 max-w-sm w-full">
          <h2 className="text-xl font-semibold">{unauthorizedTitle}</h2>
          <div className="bg-yellow-50 border border-yellow-300 text-yellow-800 px-4 py-3 rounded-md text-sm">
            {unauthorizedMessage}
          </div>
          <p className="text-sm text-gray-500">
            Roles actuales: {roles.length > 0 ? roles.join(', ') : 'sin roles asignados'}.
          </p>
          <div className="flex flex-col sm:flex-row gap-2">
            <button className="btn-primary" onClick={() => { void logout(); }}>
              Cerrar sesión
            </button>
            <button
              className="btn-secondary"
              onClick={() => {
                void login(`${location.pathname}${location.search}${location.hash}`);
              }}
            >
              Reintentar con otra cuenta
            </button>
          </div>
        </div>
      </div>
    );
  }

  return <>{children}</>;
};
