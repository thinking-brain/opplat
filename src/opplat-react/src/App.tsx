import { Navigate, Route, Routes } from 'react-router-dom';
import { AuthCallbackPage } from './auth/AuthCallbackPage';
import { Layout } from './components/Layout';
import { ProtectedRoute } from './auth/ProtectedRoute';
import { TenantAccessGate } from './auth/TenantAccessGate';
import { HomePage } from './pages/HomePage';
import { InventoryPage } from './pages/InventoryPage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { ProductsPage } from './pages/ProductsPage';
import { ProductClassificationsPage } from './pages/ProductClassificationsPage';
import { ProductGroupsPage } from './pages/ProductGroupsPage';
import { SellPage } from './pages/SellPage';
import { UsersPage } from './pages/UsersPage';
import { WarehousesPage } from './pages/WarehousesPage';
import { appConfig } from './runtimeConfig';

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/auth/callback" element={<AuthCallbackPage title="Completando inicio de sesión" />} />
      <Route path="/auth/silent-renew" element={<AuthCallbackPage title="Renovando sesión" />} />
      <Route
        path="/"
        element={
          <ProtectedRoute>
            <TenantAccessGate>
              <Layout />
            </TenantAccessGate>
          </ProtectedRoute>
        }
      >
        <Route index element={<HomePage />} />
        <Route path="products" element={<ProductsPage />} />
        <Route path="sell" element={<SellPage />} />
        <Route
          path="users"
          element={(
            <ProtectedRoute
              requiredRoles={appConfig.accessControl.tenantUserManagementRoles}
              unauthorizedTitle="Sección reservada para TenantAdmin"
              unauthorizedMessage="Solo un TenantAdmin puede administrar usuarios y permisos del tenant desde esta app."
            >
              <UsersPage />
            </ProtectedRoute>
          )}
        />
        <Route path="inventory" element={<InventoryPage />} />
        <Route path="inventory/warehouses" element={<WarehousesPage />} />
        <Route path="inventory/classifications" element={<ProductClassificationsPage />} />
        <Route path="inventory/groups" element={<ProductGroupsPage />} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

export default App;
