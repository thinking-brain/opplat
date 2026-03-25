import React from 'react';
import {
  Alert,
  Card,
  CardContent,
  Grid,
  List,
  ListItem,
  ListItemText,
  Stack,
  Typography,
} from '@mui/material';
import { PageHeader } from '../components/PageHeader';
import { appConfig } from '../runtimeConfig';

const runtimeEntries: Array<[string, string]> = [
  ['App name', appConfig.appName],
  ['Admin API base', appConfig.adminApiUrl || 'Same-origin (/admin/*)'],
  ['Auth ownership', 'Handled outside the admin SPA'],
  ['Session bootstrap', 'Removed from the client shell'],
];

export const SettingsPage: React.FC = () => {
  return (
    <Stack spacing={3}>
      <PageHeader
        title="Settings"
        subtitle="Referencia del entorno y comportamiento del shell administrativo sin autenticación integrada."
      />
      <Alert severity="info">
        La app admin ya no inicia sesión, restaura sesión ni ejecuta logout. Usa este shell para trabajar
        contra el admin API mientras resuelves la autenticación por tu cuenta.
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
                <Typography variant="h6">Admin API scope</Typography>
                <Typography color="text.secondary">
                  El admin API es responsable solo del catálogo multitenant. La gestión de usuarios pertenece
                  al tenant y se resuelve dentro de su propia aplicación.
                </Typography>
              </Stack>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Stack>
  );
};
