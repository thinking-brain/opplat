import { Navigate, Route, Routes } from 'react-router-dom';
import { AuthCallbackPage } from './auth/AuthCallbackPage';
import { ProtectedRoute } from './auth/ProtectedRoute';
import { Layout } from './components/Layout';
import { appConfig } from './runtimeConfig';
import { DashboardPage } from './pages/DashboardPage';
import { LoginPage } from './pages/LoginPage';
import { SettingsPage } from './pages/SettingsPage';
import { TenantsPage } from './pages/TenantsPage';

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/auth/callback" element={<AuthCallbackPage title="Completando inicio de sesión" />} />
      <Route path="/auth/silent-renew" element={<AuthCallbackPage title="Renovando sesión" />} />
      <Route
        path="/"
        element={
          <ProtectedRoute
            requiredRoles={appConfig.accessControl.adminPortalRoles}
            unauthorizedTitle="Portal exclusivo para SuperAdmin"
            unauthorizedMessage="El sitio administrativo solo admite cuentas SuperAdmin. La administración de usuarios de tenant se realiza desde la app cliente."
          >
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route index element={<DashboardPage />} />
        <Route path="tenants" element={<TenantsPage />} />
        <Route path="users" element={<Navigate to="/settings" replace />} />
        <Route path="settings" element={<SettingsPage />} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

export default App;
