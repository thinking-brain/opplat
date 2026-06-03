import React from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Package,
  Lock,
  ShoppingCart,
  Users,
  Warehouse,
  Settings,
  TrendingUp,
  CheckCircle2,
  AlertTriangle,
} from 'lucide-react';
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
} from 'recharts';
import { useAuth } from '../auth/AuthContext';
import { hasAnyRole, hasRole, TENANT_USER_ROLE } from '../auth/roles';
import { appConfig } from '../runtimeConfig';

interface QuickAccessCard {
  title: string;
  description: string;
  icon: React.ReactNode;
  path: string;
  color: string;
  requiredRoles?: string[];
}

interface StatCard {
  title: string;
  value: string | number;
  icon: React.ReactNode;
  color: string;
}

interface SalesData {
  day: string;
  sales: number;
  transactions: number;
}

const quickAccessCards: QuickAccessCard[] = [
  {
    title: 'Products',
    description: 'Manage product catalog',
    icon: <Package size={36} />,
    path: '/products',
    color: '#1976d2',
  },
  {
    title: 'Sell',
    description: 'Create new sales',
    icon: <ShoppingCart size={36} />,
    path: '/sell',
    color: '#2e7d32',
  },
  {
    title: 'Users',
    description: 'Manage system users',
    icon: <Users size={36} />,
    path: '/users',
    color: '#ed6c02',
    requiredRoles: appConfig.accessControl.tenantUserManagementRoles,
  },
  {
    title: 'Inventory',
    description: 'Stock & movements',
    icon: <Warehouse size={36} />,
    path: '/inventory',
    color: '#7b1fa2',
  },
  {
    title: 'Settings',
    description: 'License & config',
    icon: <Settings size={36} />,
    path: '/license',
    color: '#0288d1',
  },
];

const salesData: SalesData[] = [
  { day: 'Mon', sales: 1200, transactions: 8 },
  { day: 'Tue', sales: 1900, transactions: 12 },
  { day: 'Wed', sales: 1500, transactions: 10 },
  { day: 'Thu', sales: 2200, transactions: 15 },
  { day: 'Fri', sales: 2800, transactions: 20 },
  { day: 'Sat', sales: 3200, transactions: 25 },
  { day: 'Sun', sales: 1100, transactions: 7 },
];

const statCards: StatCard[] = [
  {
    title: 'Total Sales Today',
    value: '$3,200',
    icon: <TrendingUp size={28} />,
    color: '#1976d2',
  },
  {
    title: 'Active Products',
    value: 142,
    icon: <CheckCircle2 size={28} />,
    color: '#2e7d32',
  },
  {
    title: 'Low Stock Alerts',
    value: 5,
    icon: <AlertTriangle size={28} />,
    color: '#ed6c02',
  },
];

export const HomePage: React.FC = () => {
  const { user, tenantIdentifier, roles } = useAuth();
  const navigate = useNavigate();
  const canManageTenantUsers = hasAnyRole(roles, appConfig.accessControl.tenantUserManagementRoles);
  const isTenantUser = hasRole(roles, TENANT_USER_ROLE);

  return (
    <div>
      <h1 className="text-2xl font-bold mb-3">Dashboard</h1>
      <div className="flex flex-wrap items-center gap-2 mb-4">
        <span className="text-sm text-gray-500">Bienvenido, {user?.username}.</span>
        {tenantIdentifier && (
          <span className="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium border border-blue-400 text-blue-700 bg-blue-50">
            Tenant {tenantIdentifier}
          </span>
        )}
        {roles.map((role) => (
          <span key={role} className="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-700">
            {role}
          </span>
        ))}
      </div>

      <div className="card p-4 mb-4">
        <p className="text-sm font-medium mb-1">Sesión OIDC activa</p>
        <p className="text-sm text-gray-500">
          El tenant se resuelve desde tus claims y cada llamada API incluye el header X-Tenant-Identifier además del prefijo de ruta correspondiente.
        </p>
        <p className="text-sm text-gray-500 mt-1">
          {canManageTenantUsers
            ? 'Tu rol TenantAdmin puede gestionar usuarios y permisos del tenant desde esta app.'
            : isTenantUser
              ? 'Tu rol TenantUser puede operar la app, pero la administración de usuarios está reservada para TenantAdmin.'
              : 'Tu sesión no tiene un rol de tenant reconocido todavía, así que el acceso administrativo del tenant permanecerá oculto.'}
        </p>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-4">
        {statCards.map((stat) => (
          <div key={stat.title} className="card p-4 flex items-center gap-3">
            <div style={{ color: stat.color }}>{stat.icon}</div>
            <div>
              <p className="text-2xl font-bold">{stat.value}</p>
              <p className="text-sm text-gray-500">{stat.title}</p>
            </div>
          </div>
        ))}
      </div>

      <div className="card p-4 mb-4">
        <p className="text-base font-semibold mb-3">Weekly Sales Overview</p>
        <ResponsiveContainer width="100%" height={280}>
          <BarChart data={salesData} margin={{ top: 5, right: 20, left: 0, bottom: 5 }}>
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="day" />
            <YAxis />
            <Tooltip />
            <Legend />
            <Bar dataKey="sales" fill="#1976d2" name="Sales ($)" />
            <Bar dataKey="transactions" fill="#2e7d32" name="Transactions" />
          </BarChart>
        </ResponsiveContainer>
      </div>

      <p className="text-base font-semibold mb-3">Quick Access</p>
      <div className="flex flex-wrap gap-4">
        {quickAccessCards.map((card) => {
          const canAccessCard = !card.requiredRoles || hasAnyRole(roles, card.requiredRoles);
          return (
            <button
              key={card.path}
              className="card p-4 text-left hover:shadow-md cursor-pointer w-32 flex flex-col items-center gap-2 transition-shadow disabled:opacity-50 disabled:cursor-not-allowed"
              onClick={() => navigate(card.path)}
              disabled={!canAccessCard}
            >
              <div style={{ color: card.color }}>{card.icon}</div>
              <p className="text-sm font-semibold text-center">{card.title}</p>
              <p className="text-xs text-gray-500 text-center">{card.description}</p>
              {card.requiredRoles && !canAccessCard && (
                <span className="inline-flex items-center gap-1 px-1.5 py-0.5 rounded-full text-xs border border-gray-300 text-gray-500">
                  <Lock size={10} />
                  TenantAdmin
                </span>
              )}
            </button>
          );
        })}
      </div>
    </div>
  );
};
