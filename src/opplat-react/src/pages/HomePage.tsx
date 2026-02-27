import React from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Card,
  CardContent,
  CardActionArea,
  Typography,
  Grid,
  Paper,
} from '@mui/material';
import {
  Inventory as ProductsIcon,
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

interface QuickAccessCard {
  title: string;
  description: string;
  icon: React.ReactNode;
  path: string;
  color: string;
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
  const { user } = useAuth();
  const navigate = useNavigate();

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>
      <Typography variant="body1" color="textSecondary" paragraph>
        Bienvenido, {user?.username}!
      </Typography>

      {/* Summary stat cards */}
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

      {/* Sales bar chart */}
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

      {/* Quick access — smaller cards */}
      <Typography variant="h6" gutterBottom>
        Quick Access
      </Typography>
      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2 }}>
        {quickAccessCards.map((card) => (
          <Box key={card.path} sx={{ flex: '1 1 140px', minWidth: 120, maxWidth: 180 }}>
            <Card>
              <CardActionArea onClick={() => navigate(card.path)}>
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
                </CardContent>
              </CardActionArea>
            </Card>
          </Box>
        ))}
      </Box>
    </Box>
  );
};
