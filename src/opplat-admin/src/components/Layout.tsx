import React, { useMemo, useState } from 'react';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import {
  AppBar,
  Avatar,
  Box,
  Button,
  Chip,
  CssBaseline,
  Divider,
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
  Dashboard as DashboardIcon,
  Domain as TenantsIcon,
  Menu as MenuIcon,
  Settings as SettingsIcon,
} from '@mui/icons-material';
import { useAuth } from '../auth/AuthContext';
import { appConfig } from '../runtimeConfig';

const drawerWidth = 260;

interface NavItem {
  label: string;
  path: string;
  icon: React.ReactNode;
  description: string;
}

const navItems: NavItem[] = [
  { label: 'Dashboard', path: '/', icon: <DashboardIcon />, description: 'Resumen administrativo' },
  { label: 'Tenants', path: '/tenants', icon: <TenantsIcon />, description: 'Catálogo y estado' },
  { label: 'Settings', path: '/settings', icon: <SettingsIcon />, description: 'Entorno y sesión' },
];

export const Layout: React.FC = () => {
  const [mobileOpen, setMobileOpen] = useState(false);
  const { user, roles, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const initials = useMemo(() => {
    const fullName = `${user?.name ?? ''} ${user?.lastName ?? ''}`.trim();
    if (!fullName) {
      return user?.username?.slice(0, 2).toUpperCase() ?? 'AD';
    }

    return fullName
      .split(' ')
      .slice(0, 2)
      .map((part) => part[0])
      .join('')
      .toUpperCase();
  }, [user]);

  const handleNavigation = (path: string): void => {
    navigate(path);
    setMobileOpen(false);
  };

  const drawer = (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Toolbar sx={{ alignItems: 'flex-start', px: 3, py: 2 }}>
        <Stack spacing={1}>
          <Typography variant="h6">{appConfig.appName}</Typography>
          <Typography variant="body2" color="text.secondary">
            Administración centralizada para SuperAdmin.
          </Typography>
        </Stack>
      </Toolbar>
      <Divider />
      <List sx={{ px: 2, py: 2, flexGrow: 1 }}>
        {navItems.map((item) => {
          const selected = item.path === '/'
            ? location.pathname === '/'
            : location.pathname.startsWith(item.path);

          return (
            <ListItem key={item.path} disablePadding sx={{ mb: 1 }}>
              <ListItemButton
                selected={selected}
                onClick={() => handleNavigation(item.path)}
                sx={{ borderRadius: 2, alignItems: 'flex-start', py: 1.5 }}
              >
                <ListItemIcon sx={{ minWidth: 40 }}>{item.icon}</ListItemIcon>
                <ListItemText
                  primary={item.label}
                  secondary={item.description}
                  primaryTypographyProps={{ fontWeight: 600 }}
                />
              </ListItemButton>
            </ListItem>
          );
        })}
      </List>
    </Box>
  );

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh', bgcolor: 'background.default' }}>
      <CssBaseline />
      <AppBar
        position="fixed"
        color="inherit"
        elevation={0}
        sx={{
          borderBottom: (theme) => `1px solid ${theme.palette.divider}`,
          width: { sm: `calc(100% - ${drawerWidth}px)` },
          ml: { sm: `${drawerWidth}px` },
          bgcolor: 'background.paper',
        }}
      >
        <Toolbar sx={{ gap: 2 }}>
          <IconButton
            color="inherit"
            edge="start"
            onClick={() => setMobileOpen((value) => !value)}
            sx={{ display: { sm: 'none' } }}
          >
            <MenuIcon />
          </IconButton>
          <Box sx={{ flexGrow: 1 }}>
            <Typography variant="h6">Portal administrativo</Typography>
            <Typography variant="body2" color="text.secondary">
              Catálogo de tenants y configuración de plataforma.
            </Typography>
          </Box>
          <Stack direction="row" spacing={1.5} alignItems="center">
            <Chip
              label={roles.length > 0 ? roles.join(', ') : 'Sin roles'}
              color="secondary"
              variant="outlined"
            />
            <Stack direction="row" spacing={1.5} alignItems="center">
              <Avatar sx={{ bgcolor: 'primary.main' }}>{initials}</Avatar>
              <Box sx={{ display: { xs: 'none', md: 'block' } }}>
                <Typography variant="body2" fontWeight={600}>
                  {user ? `${user.name} ${user.lastName}`.trim() || user.username : 'Administrador'}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  {user?.email || user?.username}
                </Typography>
              </Box>
            </Stack>
            <Button color="inherit" variant="outlined" onClick={() => { void logout(); }}>
              Logout
            </Button>
          </Stack>
        </Toolbar>
      </AppBar>
      <Box component="nav" sx={{ width: { sm: drawerWidth }, flexShrink: { sm: 0 } }}>
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={() => setMobileOpen(false)}
          ModalProps={{ keepMounted: true }}
          sx={{
            display: { xs: 'block', sm: 'none' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
        >
          {drawer}
        </Drawer>
        <Drawer
          variant="permanent"
          open
          sx={{
            display: { xs: 'none', sm: 'block' },
            '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
          }}
        >
          {drawer}
        </Drawer>
      </Box>
      <Box component="main" sx={{ flexGrow: 1, width: { sm: `calc(100% - ${drawerWidth}px)` } }}>
        <Toolbar />
        <Box sx={{ p: { xs: 2, md: 4 } }}>
          <Outlet />
        </Box>
      </Box>
    </Box>
  );
};
