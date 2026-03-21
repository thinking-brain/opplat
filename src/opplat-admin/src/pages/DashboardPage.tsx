import React, { useEffect, useMemo, useState } from 'react';
import {
  Alert,
  Card,
  CardContent,
  Chip,
  Grid,
  List,
  ListItem,
  ListItemText,
  Stack,
  Typography,
} from '@mui/material';
import { PageHeader } from '../components/PageHeader';
import { StatCard } from '../components/StatCard';
import { adminApi } from '../api/admin.api';
import type { AdminTenant, User } from '../types';

const getErrorMessage = (error: unknown): string =>
  error instanceof Error ? error.message : 'No se pudo cargar el panel de administración.';

export const DashboardPage: React.FC = () => {
  const [tenants, setTenants] = useState<AdminTenant[]>([]);
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let mounted = true;

    const loadDashboard = async () => {
      setLoading(true);
      setError(null);

      try {
        const [tenantResponse, userResponse] = await Promise.all([
          adminApi.listTenants(),
          adminApi.listUsers(),
        ]);

        if (!mounted) {
          return;
        }

        setTenants(tenantResponse);
        setUsers(userResponse);
      } catch (loadError) {
        if (mounted) {
          setError(getErrorMessage(loadError));
        }
      } finally {
        if (mounted) {
          setLoading(false);
        }
      }
    };

    void loadDashboard();

    return () => {
      mounted = false;
    };
  }, []);

  const activeTenants = useMemo(() => tenants.filter((tenant) => tenant.isActive).length, [tenants]);
  const activeUsers = useMemo(() => users.filter((user) => user.active).length, [users]);
  const topTenants = useMemo(() => {
    const counts = users.reduce<Record<string, number>>((accumulator, user) => {
      const key = user.tenantIdentifier ?? 'sin-tenant';
      accumulator[key] = (accumulator[key] ?? 0) + 1;
      return accumulator;
    }, {});

    return Object.entries(counts).sort(([, left], [, right]) => right - left).slice(0, 5);
  }, [users]);

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Dashboard"
        subtitle="Resumen de tenants, usuarios y estado operativo del portal administrativo."
      />
      {error ? <Alert severity="error">{error}</Alert> : null}
      <Grid container spacing={2}>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard label="Tenants activos" value={activeTenants} helper="Disponibles para operación" />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard label="Tenants totales" value={tenants.length} helper="Catalogados en la plataforma" />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard label="Usuarios activos" value={activeUsers} helper="Con acceso habilitado" />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard label="Usuarios totales" value={users.length} helper="Visibles desde /admin/users" />
        </Grid>
      </Grid>

      <Grid container spacing={2}>
        <Grid item xs={12} lg={7}>
          <Card variant="outlined" sx={{ height: '100%' }}>
            <CardContent>
              <Stack spacing={2}>
                <Typography variant="h6">Tenants destacados</Typography>
                {loading ? (
                  <Typography color="text.secondary">Cargando tenants...</Typography>
                ) : tenants.length === 0 ? (
                  <Typography color="text.secondary">No hay tenants registrados.</Typography>
                ) : (
                  <List disablePadding>
                    {tenants.slice(0, 5).map((tenant) => (
                      <ListItem key={tenant.identifier} divider>
                        <ListItemText
                          primary={tenant.name}
                          secondary={`${tenant.identifier} · ${tenant.connectionString}`}
                        />
                        <Chip
                          label={tenant.isActive ? 'Activo' : 'Inactivo'}
                          color={tenant.isActive ? 'success' : 'default'}
                          variant={tenant.isActive ? 'filled' : 'outlined'}
                        />
                      </ListItem>
                    ))}
                  </List>
                )}
              </Stack>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} lg={5}>
          <Card variant="outlined" sx={{ height: '100%' }}>
            <CardContent>
              <Stack spacing={2}>
                <Typography variant="h6">Distribución de usuarios</Typography>
                {loading ? (
                  <Typography color="text.secondary">Cargando usuarios...</Typography>
                ) : topTenants.length === 0 ? (
                  <Typography color="text.secondary">No hay usuarios disponibles.</Typography>
                ) : (
                  <List disablePadding>
                    {topTenants.map(([tenantIdentifier, count]) => (
                      <ListItem key={tenantIdentifier} divider>
                        <ListItemText
                          primary={tenantIdentifier}
                          secondary={`${count} usuario${count === 1 ? '' : 's'}`}
                        />
                      </ListItem>
                    ))}
                  </List>
                )}
              </Stack>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Stack>
  );
};
