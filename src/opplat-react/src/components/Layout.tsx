import React, { useState } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import {
  Home,
  Package,
  ShoppingCart,
  Warehouse,
  Users,
  Tag,
  Layers,
  Menu,
  X,
} from 'lucide-react';
import { useAuth } from '../auth/AuthContext';
import { hasAnyRole } from '../auth/roles';
import { appConfig } from '../runtimeConfig';

interface NavItem {
  label: string;
  path: string;
  icon: React.ReactNode;
  requiredRoles?: string[];
}

const navItems: NavItem[] = [
  { label: 'Home', path: '/', icon: <Home size={20} /> },
  { label: 'Products', path: '/products', icon: <Package size={20} /> },
  { label: 'Sell', path: '/sell', icon: <ShoppingCart size={20} /> },
  { label: 'Inventory', path: '/inventory', icon: <Warehouse size={20} /> },
  { label: 'Almacenes', path: '/inventory/warehouses', icon: <Warehouse size={20} /> },
  { label: 'Clasificaciones', path: '/inventory/classifications', icon: <Tag size={20} /> },
  { label: 'Grupos', path: '/inventory/groups', icon: <Layers size={20} /> },
  {
    label: 'Users',
    path: '/users',
    icon: <Users size={20} />,
    requiredRoles: appConfig.accessControl.tenantUserManagementRoles,
  },
];

export const Layout: React.FC = () => {
  const [mobileOpen, setMobileOpen] = useState(false);
  const { user, logout, tenantIdentifier, roles } = useAuth();
  const navigate = useNavigate();

  const handleNavigation = (path: string) => {
    navigate(path);
    setMobileOpen(false);
  };

  const drawerContent = (
    <div className="flex flex-col h-full bg-white border-r border-gray-200">
      <div className="flex items-center h-16 px-4 border-b border-gray-200">
        <span className="text-lg font-semibold text-gray-800">{appConfig.appName}</span>
      </div>
      <nav className="flex-1 overflow-y-auto py-2">
        {navItems
          .filter((item) => !item.requiredRoles || hasAnyRole(roles, item.requiredRoles))
          .map((item) => (
            <button
              key={item.path}
              onClick={() => handleNavigation(item.path)}
              className="w-full flex items-center gap-3 px-4 py-3 text-gray-700 hover:bg-gray-100 transition-colors text-left"
            >
              {item.icon}
              <span className="text-sm">{item.label}</span>
            </button>
          ))}
      </nav>
    </div>
  );

  return (
    <div className="flex h-screen bg-gray-50">
      {/* Desktop sidebar */}
      <aside className="hidden sm:flex flex-col w-60 flex-shrink-0">
        {drawerContent}
      </aside>

      {/* Mobile drawer overlay */}
      {mobileOpen && (
        <div className="fixed inset-0 z-40 flex sm:hidden">
          <div
            className="fixed inset-0 bg-black/50"
            onClick={() => setMobileOpen(false)}
          />
          <aside className="relative z-50 w-60 flex-shrink-0">
            {drawerContent}
          </aside>
        </div>
      )}

      {/* Main area */}
      <div className="flex-1 flex flex-col min-w-0">
        {/* Top bar */}
        <header className="h-16 bg-blue-600 text-white flex items-center px-4 gap-3 shadow flex-shrink-0">
          <button
            className="sm:hidden p-1 rounded hover:bg-blue-700 transition-colors"
            onClick={() => setMobileOpen(!mobileOpen)}
            aria-label="open drawer"
          >
            {mobileOpen ? <X size={22} /> : <Menu size={22} />}
          </button>
          <span className="flex-1 font-semibold">{appConfig.appName}</span>
          <div className="flex items-center gap-2">
            {tenantIdentifier && (
              <span className="bg-pink-500 text-white text-xs px-2 py-1 rounded-full">
                Tenant: {tenantIdentifier}
              </span>
            )}
            <span className="text-sm hidden sm:block">{user?.username}</span>
            <button
              className="text-sm border border-white/50 px-3 py-1 rounded hover:bg-blue-700 transition-colors"
              onClick={() => { void logout(); }}
            >
              Logout
            </button>
          </div>
        </header>

        {/* Page content */}
        <main className="flex-1 overflow-y-auto p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
};

