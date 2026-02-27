import { Routes, Route, Navigate } from 'react-router-dom';
import { LoginPage } from './pages/LoginPage';
import { HomePage } from './pages/HomePage';
import { ProductsPage } from './pages/ProductsPage';
import { SellPage } from './pages/SellPage';
import { UsersPage } from './pages/UsersPage';
import { InventoryPage } from './pages/InventoryPage';
import { LicensePage } from './pages/LicensePage';
import { Layout } from './components/Layout';
import { ProtectedRoute } from './auth/ProtectedRoute';

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/"
        element={
          <ProtectedRoute>
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route index element={<HomePage />} />
        <Route path="products" element={<ProductsPage />} />
        <Route path="sell" element={<SellPage />} />
        <Route path="users" element={<UsersPage />} />
        <Route path="inventory" element={<InventoryPage />} />
        <Route path="license" element={<LicensePage />} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

export default App;
