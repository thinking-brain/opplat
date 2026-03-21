import React, { useEffect, useMemo, useState } from 'react';
import {
  Alert,
  Box,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  Stack,
  Switch,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material';
import {
  Add as AddIcon,
  Delete as DeleteIcon,
  Edit as EditIcon,
  Refresh as RefreshIcon,
} from '@mui/icons-material';
import { adminApi } from '../api/admin.api';
import { PageHeader } from '../components/PageHeader';
import type { AdminTenant, UpsertTenantRequest } from '../types';

interface TenantFormState {
  id: string;
  identifier: string;
  name: string;
  connectionString: string;
  isActive: boolean;
}

const emptyForm: TenantFormState = {
  id: '',
  identifier: '',
  name: '',
  connectionString: '',
  isActive: true,
};

const getErrorMessage = (error: unknown): string =>
  error instanceof Error ? error.message : 'No se pudieron cargar los tenants.';

export const TenantsPage: React.FC = () => {
  const [tenants, setTenants] = useState<AdminTenant[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [confirmTenant, setConfirmTenant] = useState<AdminTenant | null>(null);
  const [editingTenant, setEditingTenant] = useState<AdminTenant | null>(null);
  const [formState, setFormState] = useState<TenantFormState>(emptyForm);
  const [saving, setSaving] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);

  const isFormValid = useMemo(
    () =>
      formState.identifier.trim().length > 0 &&
      formState.name.trim().length > 0 &&
      formState.connectionString.trim().length > 0,
    [formState.connectionString, formState.identifier, formState.name],
  );

  const loadTenants = async (): Promise<void> => {
    setLoading(true);
    setError(null);
    try {
      const data = await adminApi.listTenants();
      setTenants(data);
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadTenants();
  }, []);

  const openCreateDialog = (): void => {
    setEditingTenant(null);
    setFormState(emptyForm);
    setFormError(null);
    setDialogOpen(true);
  };

  const openEditDialog = (tenant: AdminTenant): void => {
    setEditingTenant(tenant);
    setFormState({
      id: tenant.id,
      identifier: tenant.identifier,
      name: tenant.name,
      connectionString: tenant.connectionString,
      isActive: tenant.isActive,
    });
    setFormError(null);
    setDialogOpen(true);
  };

  const handleFormChange = (field: keyof TenantFormState) => (
    event: React.ChangeEvent<HTMLInputElement>,
  ): void => {
    const value = field === 'isActive' ? event.target.checked : event.target.value;
    setFormState((current) => ({ ...current, [field]: value }));
  };

  const handleSave = async (): Promise<void> => {
    if (!isFormValid) {
      setFormError('Completa los campos requeridos.');
      return;
    }

    setSaving(true);
    setFormError(null);

    try {
      const payload: UpsertTenantRequest = {
        id: formState.id.trim() || undefined,
        identifier: formState.identifier.trim(),
        name: formState.name.trim(),
        connectionString: formState.connectionString.trim(),
        isActive: formState.isActive,
      };

      if (editingTenant) {
        await adminApi.updateTenant(editingTenant.identifier, payload);
      } else {
        await adminApi.createTenant(payload);
      }

      setDialogOpen(false);
      await loadTenants();
    } catch (saveError) {
      setFormError(getErrorMessage(saveError));
    } finally {
      setSaving(false);
    }
  };

  const handleDeactivate = async (): Promise<void> => {
    if (!confirmTenant) {
      return;
    }

    setSaving(true);
    setFormError(null);
    try {
      await adminApi.deactivateTenant(confirmTenant.identifier);
      setConfirmTenant(null);
      await loadTenants();
    } catch (deactivateError) {
      setFormError(getErrorMessage(deactivateError));
    } finally {
      setSaving(false);
    }
  };

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Tenants"
        subtitle="Gestiona el catálogo multitenant y sus conexiones."
        actions={
          <Stack direction="row" spacing={1}>
            <Tooltip title="Refrescar">
              <span>
                <IconButton color="primary" onClick={() => { void loadTenants(); }} disabled={loading}>
                  <RefreshIcon />
                </IconButton>
              </span>
            </Tooltip>
            <Button variant="contained" startIcon={<AddIcon />} onClick={openCreateDialog}>
              Nuevo tenant
            </Button>
          </Stack>
        }
      />

      {error ? <Alert severity="error">{error}</Alert> : null}

      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Nombre</TableCell>
            <TableCell>Identifier</TableCell>
            <TableCell>Connection string</TableCell>
            <TableCell>Estado</TableCell>
            <TableCell align="right">Acciones</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {tenants.map((tenant) => (
            <TableRow key={tenant.identifier} hover>
              <TableCell>
                <Stack spacing={0.5}>
                  <Typography fontWeight={600}>{tenant.name}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    ID interno: {tenant.id}
                  </Typography>
                </Stack>
              </TableCell>
              <TableCell>{tenant.identifier}</TableCell>
              <TableCell sx={{ maxWidth: 420 }}>
                <Typography variant="body2" sx={{ wordBreak: 'break-all' }}>
                  {tenant.connectionString}
                </Typography>
              </TableCell>
              <TableCell>
                <Chip
                  label={tenant.isActive ? 'Activo' : 'Inactivo'}
                  color={tenant.isActive ? 'success' : 'default'}
                  variant={tenant.isActive ? 'filled' : 'outlined'}
                />
              </TableCell>
              <TableCell align="right">
                <Stack direction="row" spacing={1} justifyContent="flex-end">
                  <Tooltip title="Editar">
                    <IconButton size="small" onClick={() => openEditDialog(tenant)}>
                      <EditIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Desactivar">
                    <span>
                      <IconButton
                        size="small"
                        color="error"
                        disabled={!tenant.isActive}
                        onClick={() => setConfirmTenant(tenant)}
                      >
                        <DeleteIcon fontSize="small" />
                      </IconButton>
                    </span>
                  </Tooltip>
                </Stack>
              </TableCell>
            </TableRow>
          ))}
          {tenants.length === 0 ? (
            <TableRow>
              <TableCell colSpan={5}>
                <Box py={3} textAlign="center">
                  <Typography variant="body2" color="text.secondary">
                    No hay tenants registrados.
                  </Typography>
                </Box>
              </TableCell>
            </TableRow>
          ) : null}
        </TableBody>
      </Table>

      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{editingTenant ? 'Editar tenant' : 'Nuevo tenant'}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} mt={1}>
            {formError ? <Alert severity="error">{formError}</Alert> : null}
            <TextField
              label="ID"
              value={formState.id}
              onChange={handleFormChange('id')}
              helperText="Opcional al crear."
              fullWidth
            />
            <TextField
              label="Identifier"
              value={formState.identifier}
              onChange={handleFormChange('identifier')}
              fullWidth
              required
            />
            <TextField
              label="Nombre"
              value={formState.name}
              onChange={handleFormChange('name')}
              fullWidth
              required
            />
            <TextField
              label="Connection string"
              value={formState.connectionString}
              onChange={handleFormChange('connectionString')}
              fullWidth
              multiline
              minRows={2}
              required
            />
            <Stack direction="row" spacing={1} alignItems="center">
              <Switch checked={formState.isActive} onChange={handleFormChange('isActive')} />
              <Typography variant="body2">Tenant activo</Typography>
            </Stack>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancelar</Button>
          <Button variant="contained" onClick={() => { void handleSave(); }} disabled={!isFormValid || saving}>
            {saving ? 'Guardando...' : 'Guardar'}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={Boolean(confirmTenant)} onClose={() => setConfirmTenant(null)}>
        <DialogTitle>Desactivar tenant</DialogTitle>
        <DialogContent>
          <Stack spacing={1} mt={1}>
            <Typography>
              ¿Seguro que deseas desactivar el tenant {confirmTenant?.identifier}?
            </Typography>
            {formError ? <Alert severity="error">{formError}</Alert> : null}
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setConfirmTenant(null)}>Cancelar</Button>
          <Button color="error" variant="contained" onClick={() => { void handleDeactivate(); }} disabled={saving}>
            {saving ? 'Desactivando...' : 'Desactivar'}
          </Button>
        </DialogActions>
      </Dialog>
    </Stack>
  );
};
