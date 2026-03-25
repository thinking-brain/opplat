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
import type { AdminTenant } from '../types';

const getErrorMessage = (error: unknown): string =>
  error instanceof Error ? error.message : 'No se pudo cargar el panel de administración.';

export const DashboardPage: React.FC = () => {
  const [tenants, setTenants] = useState<AdminTenant[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let mounted = true;

    const loadDashboard = async () => {
      setLoading(true);
      setError(null);

      try {
        const tenantResponse = await adminApi.listTenants();

        if (!mounted) {
          return;
        }

        setTenants(tenantResponse);
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
  const totalUsers = useMemo(() => tenants.reduce((sum, tenant) => sum + tenant.userCount, 0), [tenants]);

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Dashboard"
        subtitle="Resumen de tenants, conteo de usuarios y estado operativo del portal administrativo."
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
          <StatCard label="Usuarios totales" value={totalUsers} helper="Por suscripción en todos los tenants" />
        </Grid>
        <Grid item xs={12} sm={6} md={3}>
          <StatCard label="Base de datos" value={tenants.length} helper="Instancias aprovisionadas" />
        </Grid>
      </Grid>

      <Grid container spacing={2}>
        <Grid item xs={12}>
          <Card variant="outlined" sx={{ height: '100%' }}>
            <CardContent>
              <Stack spacing={2}>
                <Typography variant="h6">Catálogo de tenants</Typography>
                {loading ? (
                  <Typography color="text.secondary">Cargando tenants...</Typography>
                ) : tenants.length === 0 ? (
                  <Typography color="text.secondary">No hay tenants registrados.</Typography>
                ) : (
                  <List disablePadding>
                    {tenants.slice(0, 10).map((tenant) => (
                      <ListItem key={tenant.identifier} divider>
                        <ListItemText
                          primary={tenant.name}
                          secondary={`${tenant.identifier} · ${tenant.databaseName} (schema: ${tenant.databaseSchema}) · ${tenant.userCount} usuario${tenant.userCount === 1 ? '' : 's'}`}
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
      </Grid>
    </Stack>
  );
};
