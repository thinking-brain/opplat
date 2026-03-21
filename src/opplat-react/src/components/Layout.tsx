import React, { useState } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import {
  AppBar,
  Box,
  Button,
  Chip,
  CssBaseline,
  Drawer,
  IconButton,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Stack,
  Toolbar,
  Typography,
} from '@mui/material';
import {
  Home as HomeIcon,
  Inventory as ProductsIcon,
  Menu as MenuIcon,
  PointOfSale as SellIcon,
  People as UsersIcon,
  Settings as SettingsIcon,
  Warehouse as InventoryIcon,
} from '@mui/icons-material';
import { useAuth } from '../auth/AuthContext';
import { hasAnyRole, hasRole, TENANT_ADMIN_ROLE, TENANT_USER_ROLE } from '../auth/roles';
import { appConfig } from '../runtimeConfig';

const drawerWidth = 240;

interface NavItem {
  label: string;
  path: string;
  icon: React.ReactNode;
  requiredRoles?: string[];
}

const navItems: NavItem[] = [
  { label: 'Home', path: '/', icon: <HomeIcon /> },
  { label: 'Products', path: '/products', icon: <ProductsIcon /> },
  { label: 'Sell', path: '/sell', icon: <SellIcon /> },
  { label: 'Inventory', path: '/inventory', icon: <InventoryIcon /> },
  {
    label: 'Users',
    path: '/users',
    icon: <UsersIcon />,
    requiredRoles: appConfig.accessControl.tenantUserManagementRoles,
  },
  { label: 'License / Settings', path: '/license', icon: <SettingsIcon /> },
];

export const Layout: React.FC = () => {
  const [mobileOpen, setMobileOpen] = useState(false);
  const { user, logout, tenantIdentifier, roles } = useAuth();
  const navigate = useNavigate();
  const accessLabel = hasRole(roles, TENANT_ADMIN_ROLE)
    ? TENANT_ADMIN_ROLE
    : hasRole(roles, TENANT_USER_ROLE)
      ? TENANT_USER_ROLE
      : roles.length > 0
        ? roles.join(', ')
        : 'Sin roles';

  const handleDrawerToggle = () => {
    setMobileOpen(!mobileOpen);
  };

  const handleNavigation = (path: string) => {
    navigate(path);
    setMobileOpen(false);
  };

  const drawer = (
    <div>
      <Toolbar>
        <Typography variant="h6" noWrap component="div">
          {appConfig.appName}
        </Typography>
      </Toolbar>
      <List>
        {navItems.filter((item) => !item.requiredRoles || hasAnyRole(roles, item.requiredRoles)).map((item) => (
          <ListItem key={item.path} disablePadding>
            <ListItemButton onClick={() => handleNavigation(item.path)}>
              <ListItemIcon>{item.icon}</ListItemIcon>
              <ListItemText primary={item.label} />
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </div>
  );

  return (
    <Box sx={{ display: 'flex' }}>
      <CssBaseline />
      <AppBar
        position="fixed"
        sx={{
          width: { sm: `calc(100% - ${drawerWidth}px)` },
          ml: { sm: `${drawerWidth}px` },
        }}
      >
        <Toolbar>
          <IconButton
            color="inherit"
            aria-label="open drawer"
            edge="start"
            onClick={handleDrawerToggle}
            sx={{ mr: 2, display: { sm: 'none' } }}
          >
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
            {appConfig.appName}
          </Typography>
          <Stack direction="row" spacing={1} alignItems="center">
            {tenantIdentifier && (
              <Chip
                label={`Tenant: ${tenantIdentifier}`}
                color="secondary"
                variant="filled"
                sx={{ color: 'white' }}
              />
            )}
            <Chip
              label={accessLabel}
              color="secondary"
              variant="outlined"
            />
            <Typography variant="body1">{user?.username}</Typography>
            <Button color="inherit" onClick={() => { void logout(); }}>
              Logout
            </Button>
          </Stack>
        </Toolbar>
      </AppBar>
      <Box
        component="nav"
        sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}
      >
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={handleDrawerToggle}
          ModalProps={{
            keepMounted: true,
          }}
          sx={{
            display: { xs: 'block', sm: 'none' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
        >
          {drawer}
        </Drawer>
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: 'none', sm: 'block' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
          open
        >
          {drawer}
        </Drawer>
      </Box>
      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          width: { sm: `calc(100% - ${drawerWidth}px)` },
        }}
      >
        <Toolbar />
        <Outlet />
      </Box>
    </Box>
  );
};
