import React from 'react';
import {
  Alert,
  Button,
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
import { useAuth } from '../auth/AuthContext';
import { getStoredTenantIdentifier, persistTenantIdentifier } from '../auth/claims';
import { PageHeader } from '../components/PageHeader';
import { appConfig } from '../runtimeConfig';

const runtimeEntries: Array<[string, string]> = [
  ['App name', appConfig.appName],
  ['API URL', appConfig.adminApiUrl],
  ['OIDC authority', appConfig.authAuthority],
  ['OIDC client', appConfig.authClientId],
  ['OIDC audience', appConfig.authAudience],
  ['OIDC scope', appConfig.authScope],
  ['Redirect path', appConfig.authRedirectPath],
  ['Silent renew path', appConfig.authSilentRedirectPath],
];

export const SettingsPage: React.FC = () => {
  const { user, roles } = useAuth();
  const selectedTenant = getStoredTenantIdentifier();

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Settings"
        subtitle="Referencia del entorno, proveedor OIDC y modelo de acceso del portal administrativo."
      />
      <Alert severity="info">
        El portal admin está reservado para SuperAdmin. La gestión de usuarios TenantAdmin y TenantUser
        se hace desde la sección de usuarios de la app cliente.
      </Alert>
      <Grid container spacing={2}>
        <Grid item xs={12} lg={6}>
          <Card variant="outlined" sx={{ height: '100%' }}>
            <CardContent>
              <Stack spacing={2}>
                <Typography variant="h6">Runtime configuration</Typography>
                <List disablePadding>
                  {runtimeEntries.map(([label, value]) => (
                    <ListItem key={label} divider>
                      <ListItemText primary={label} secondary={value} />
                    </ListItem>
                  ))}
                </List>
              </Stack>
            </CardContent>
          </Card>
        </Grid>
        <Grid item xs={12} lg={6}>
          <Card variant="outlined" sx={{ height: '100%' }}>
            <CardContent>
              <Stack spacing={2}>
                <Typography variant="h6">Current session</Typography>
                <Typography><strong>Usuario:</strong> {user?.username ?? 'N/A'}</Typography>
                <Typography>
                  <strong>Nombre:</strong> {[user?.name, user?.lastName].filter(Boolean).join(' ') || 'N/A'}
                </Typography>
                <Typography><strong>Email:</strong> {user?.email || 'N/A'}</Typography>
                <Typography><strong>Tenant seleccionado:</strong> {selectedTenant || 'Ninguno'}</Typography>
                <Stack direction="row" spacing={1} flexWrap="wrap" useFlexGap>
                  {roles.length > 0 ? (
                    roles.map((role) => <Chip key={role} label={role} color="primary" variant="outlined" />)
                  ) : (
                    <Chip label="Sin roles" variant="outlined" />
                  )}
                </Stack>
                <Button variant="outlined" onClick={() => persistTenantIdentifier(null)}>
                  Limpiar tenant seleccionado
                </Button>
              </Stack>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Stack>
  );
};
