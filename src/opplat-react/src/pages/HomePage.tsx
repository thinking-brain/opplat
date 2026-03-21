import React from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Card,
  CardContent,
  CardActionArea,
  Chip,
  Grid,
  Paper,
  Stack,
  Typography,
} from '@mui/material';
import {
  Inventory as ProductsIcon,
  Lock as LockIcon,
  PointOfSale as SellIcon,
  People as UsersIcon,
  Warehouse as InventoryNavIcon,
  Settings as LicenseNavIcon,
  TrendingUp as TrendingUpIcon,
  CheckCircle as CheckCircleIcon,
  Warning as WarningIcon,
} from '@mui/icons-material';
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
    icon: <ProductsIcon sx={{ fontSize: 36 }} />,
    path: '/products',
    color: '#1976d2',
  },
  {
    title: 'Sell',
    description: 'Create new sales',
    icon: <SellIcon sx={{ fontSize: 36 }} />,
    path: '/sell',
    color: '#2e7d32',
  },
  {
    title: 'Users',
    description: 'Manage system users',
    icon: <UsersIcon sx={{ fontSize: 36 }} />,
    path: '/users',
    color: '#ed6c02',
    requiredRoles: appConfig.accessControl.tenantUserManagementRoles,
  },
  {
    title: 'Inventory',
    description: 'Stock & movements',
    icon: <InventoryNavIcon sx={{ fontSize: 36 }} />,
    path: '/inventory',
    color: '#7b1fa2',
  },
  {
    title: 'Settings',
    description: 'License & config',
    icon: <LicenseNavIcon sx={{ fontSize: 36 }} />,
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
    icon: <TrendingUpIcon fontSize="large" />,
    color: '#1976d2',
  },
  {
    title: 'Active Products',
    value: 142,
    icon: <CheckCircleIcon fontSize="large" />,
    color: '#2e7d32',
  },
  {
    title: 'Low Stock Alerts',
    value: 5,
    icon: <WarningIcon fontSize="large" />,
    color: '#ed6c02',
  },
];

export const HomePage: React.FC = () => {
  const { user, tenantIdentifier, roles } = useAuth();
  const navigate = useNavigate();
  const canManageTenantUsers = hasAnyRole(roles, appConfig.accessControl.tenantUserManagementRoles);
  const isTenantUser = hasRole(roles, TENANT_USER_ROLE);

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>
      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={1} mb={2}>
        <Typography variant="body1" color="text.secondary">
          Bienvenido, {user?.username}.
        </Typography>
        {tenantIdentifier && <Chip label={`Tenant ${tenantIdentifier}`} color="primary" variant="outlined" />}
        {roles.map((role) => (
          <Chip key={role} label={role} size="small" />
        ))}
      </Stack>

      <Paper sx={{ p: 2, mb: 3 }}>
        <Typography variant="subtitle1" gutterBottom>
          Sesión OIDC activa
        </Typography>
        <Typography variant="body2" color="text.secondary">
          El tenant se resuelve desde tus claims y cada llamada API incluye el header X-Tenant-Identifier además del prefijo de ruta correspondiente.
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
          {canManageTenantUsers
            ? 'Tu rol TenantAdmin puede gestionar usuarios y permisos del tenant desde esta app.'
            : isTenantUser
              ? 'Tu rol TenantUser puede operar la app, pero la administración de usuarios está reservada para TenantAdmin.'
              : 'Tu sesión no tiene un rol de tenant reconocido todavía, así que el acceso administrativo del tenant permanecerá oculto.'}
        </Typography>
      </Paper>

      <Grid container spacing={2} sx={{ mb: 3 }}>
        {statCards.map((stat) => (
          <Grid item xs={12} sm={4} key={stat.title}>
            <Card>
              <CardContent sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                <Box sx={{ color: stat.color }}>{stat.icon}</Box>
                <Box>
                  <Typography variant="h5" component="div">
                    {stat.value}
                  </Typography>
                  <Typography variant="body2" color="textSecondary">
                    {stat.title}
                  </Typography>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      <Paper sx={{ p: 2, mb: 3 }}>
        <Typography variant="h6" gutterBottom>
          Weekly Sales Overview
        </Typography>
        <ResponsiveContainer width="100%" height={280}>
          <BarChart
            data={salesData}
            margin={{ top: 5, right: 20, left: 0, bottom: 5 }}
          >
            <CartesianGrid strokeDasharray="3 3" />
            <XAxis dataKey="day" />
            <YAxis />
            <Tooltip />
            <Legend />
            <Bar dataKey="sales" fill="#1976d2" name="Sales ($)" />
            <Bar dataKey="transactions" fill="#2e7d32" name="Transactions" />
          </BarChart>
        </ResponsiveContainer>
      </Paper>

      <Typography variant="h6" gutterBottom>
        Quick Access
      </Typography>
      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2 }}>
        {quickAccessCards.map((card) => {
          const canAccessCard = !card.requiredRoles || hasAnyRole(roles, card.requiredRoles);

          return (
            <Box key={card.path} sx={{ flex: '1 1 140px', minWidth: 120, maxWidth: 180 }}>
              <Card>
                <CardActionArea
                  disabled={!canAccessCard}
                  onClick={() => navigate(card.path)}
                >
                <CardContent
                  sx={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    py: 2,
                    px: 1,
                  }}
                >
                  <Box sx={{ color: card.color, mb: 1 }}>{card.icon}</Box>
                  <Typography
                    variant="subtitle2"
                    component="div"
                    align="center"
                    sx={{ fontWeight: 600 }}
                  >
                    {card.title}
                  </Typography>
                  <Typography variant="caption" color="textSecondary" align="center">
                    {card.description}
                  </Typography>
                   {card.requiredRoles && !canAccessCard ? (
                     <Chip
                       icon={<LockIcon />}
                       label="TenantAdmin"
                       size="small"
                      variant="outlined"
                      sx={{ mt: 1 }}
                    />
                  ) : null}
                </CardContent>
                </CardActionArea>
              </Card>
            </Box>
          );
        })}
      </Box>
    </Box>
  );
};
