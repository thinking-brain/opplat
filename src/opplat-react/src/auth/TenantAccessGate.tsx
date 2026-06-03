import React, { useEffect, useState } from 'react';
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
      <div className="flex justify-center items-center min-h-screen">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex justify-center items-center min-h-screen px-4">
        <div className="flex flex-col gap-3 max-w-md w-full">
          <h2 className="text-xl font-semibold">No se pudo validar el tenant</h2>
          <div className="bg-red-50 border border-red-300 text-red-700 px-4 py-3 rounded-md text-sm">
            {error}
          </div>
          <div className="flex flex-col sm:flex-row gap-2">
            <button className="btn-primary" onClick={() => { window.location.reload(); }}>
              Reintentar
            </button>
            <button className="btn-secondary" onClick={() => { void logout(); }}>
              Cerrar sesión
            </button>
          </div>
        </div>
      </div>
    );
  }

  if (tenantContext && !tenantContext.isResolved) {
    return (
      <div className="flex justify-center items-center min-h-screen px-4">
        <div className="flex flex-col gap-3 max-w-md w-full">
          <h2 className="text-xl font-semibold">Tu usuario no tiene tenant asignado</h2>
          <div className="bg-yellow-50 border border-yellow-300 text-yellow-800 px-4 py-3 rounded-md text-sm">
            {tenantContext.message}
          </div>
          <p className="text-sm text-gray-500">
            Necesitas una asignacion de tenant antes de poder usar la aplicacion.
          </p>
          <button className="btn-secondary" onClick={() => { void logout(); }}>
            Cerrar sesión
          </button>
        </div>
      </div>
    );
  }

  if (tenantContext && !tenantContext.isActive) {
    return (
      <div className="flex justify-center items-center min-h-screen px-4">
        <div className="flex flex-col gap-3 max-w-md w-full">
          <h2 className="text-xl font-semibold">Tenant inactivo</h2>
          <div className="bg-yellow-50 border border-yellow-300 text-yellow-800 px-4 py-3 rounded-md text-sm">
            {tenantContext.message}
          </div>
          <p className="text-sm text-gray-500">
            {tenantContext.tenantName ?? tenantContext.tenantIdentifier ?? 'Tu tenant'} esta temporalmente inactivo.
            Contacta al administrador principal para reactivar el acceso.
          </p>
          <button className="btn-secondary" onClick={() => { void logout(); }}>
            Cerrar sesión
          </button>
        </div>
      </div>
    );
  }

  return <>{children}</>;
};

