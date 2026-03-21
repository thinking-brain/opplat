import React, { useEffect, useMemo, useState } from 'react';
import type { SelectChangeEvent } from '@mui/material/Select';
import {
  Alert,
  Box,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControl,
  IconButton,
  InputLabel,
  MenuItem,
  Select,
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
  Edit as EditIcon,
  Refresh as RefreshIcon,
} from '@mui/icons-material';
import { adminApi } from '../api/admin.api';
import { TENANT_ADMIN_ROLE, TENANT_USER_ROLE, normalizeRoles } from '../auth/roles';
import { getStoredTenantIdentifier, persistTenantIdentifier } from '../auth/claims';
import { PageHeader } from '../components/PageHeader';
import type { AdminCreateUserRequest, AdminTenant, AdminUpdateUserRequest, User } from '../types';

interface UserDialogState {
  mode: 'create' | 'edit';
  user: User | null;
}

interface UserFormState {
  name: string;
  lastName: string;
  username: string;
  email: string;
  roles: string;
  active: boolean;
}

const emptyForm: UserFormState = {
  name: '',
  lastName: '',
  username: '',
  email: '',
  roles: TENANT_USER_ROLE,
  active: true,
};

const getErrorMessage = (error: unknown): string =>
  error instanceof Error ? error.message : 'No se pudieron cargar los usuarios.';

const parseRoles = (value: string): string[] =>
  value
    .split(',')
    .map((role) => role.trim())
    .filter(Boolean);

export const UsersPage: React.FC = () => {
  const [tenants, setTenants] = useState<AdminTenant[]>([]);
  const [users, setUsers] = useState<User[]>([]);
  const [selectedTenant, setSelectedTenant] = useState<string>(getStoredTenantIdentifier() ?? '');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [dialogState, setDialogState] = useState<UserDialogState | null>(null);
  const [formState, setFormState] = useState<UserFormState>(emptyForm);
  const [saving, setSaving] = useState(false);

  const selectedTenantName = useMemo(
    () => tenants.find((tenant) => tenant.identifier === selectedTenant)?.name,
    [selectedTenant, tenants],
  );

  const loadData = async (tenantIdentifier = selectedTenant): Promise<void> => {
    setLoading(true);
    setError(null);

    try {
      const tenantResponse = await adminApi.listTenants();
      setTenants(tenantResponse);

      const normalizedTenant = tenantIdentifier && tenantResponse.some((tenant) => tenant.identifier === tenantIdentifier)
        ? tenantIdentifier
        : '';

      if (tenantIdentifier !== normalizedTenant) {
        setSelectedTenant(normalizedTenant);
      }

      persistTenantIdentifier(normalizedTenant || null);

      const userResponse = normalizedTenant
        ? await adminApi.listTenantUsers(normalizedTenant)
        : await adminApi.listUsers();
      setUsers(userResponse);
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadData(selectedTenant);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleTenantChange = (event: SelectChangeEvent): void => {
    const value = event.target.value;
    setSelectedTenant(value);
    persistTenantIdentifier(value || null);
    void loadData(value);
  };

  const openCreateDialog = (): void => {
    setDialogState({ mode: 'create', user: null });
    setFormState(emptyForm);
  };

  const openEditDialog = (user: User): void => {
    setDialogState({ mode: 'edit', user });
    setFormState({
      name: user.name,
      lastName: user.lastName,
      username: user.username,
      email: user.email,
      roles: normalizeRoles(user.roles).join(', '),
      active: user.active,
    });
  };

  const closeDialog = (): void => {
    if (saving) {
      return;
    }

    setDialogState(null);
    setFormState(emptyForm);
  };

  const handleSave = async (): Promise<void> => {
    if (!selectedTenant) {
      setError('Selecciona un tenant para crear o editar usuarios.');
      return;
    }

    setSaving(true);
    setError(null);

    try {
      const roles = parseRoles(formState.roles);

      if (dialogState?.mode === 'edit' && dialogState.user) {
        const updateRequest: AdminUpdateUserRequest = {
          name: formState.name.trim(),
          lastName: formState.lastName.trim(),
          username: formState.username.trim(),
          email: formState.email.trim(),
          active: formState.active,
        };

        await adminApi.updateTenantUser(selectedTenant, dialogState.user.userId, updateRequest);
        await adminApi.setTenantUserRoles(selectedTenant, dialogState.user.userId, { roles });
        await adminApi.setTenantUserStatus(selectedTenant, dialogState.user.userId, { active: formState.active });
      } else {
        const createRequest: AdminCreateUserRequest = {
          name: formState.name.trim(),
          lastName: formState.lastName.trim(),
          username: formState.username.trim(),
          email: formState.email.trim(),
          roles,
        };

        await adminApi.createTenantUser(selectedTenant, createRequest);
      }

      closeDialog();
      await loadData(selectedTenant);
    } catch (saveError) {
      setError(getErrorMessage(saveError));
    } finally {
      setSaving(false);
    }
  };

  const handleToggleActive = async (user: User): Promise<void> => {
    const tenantIdentifier = selectedTenant || user.tenantIdentifier;
    if (!tenantIdentifier) {
      setError('No se pudo resolver el tenant para actualizar el estado.');
      return;
    }

    setSaving(true);
    setError(null);

    try {
      await adminApi.setTenantUserStatus(tenantIdentifier, user.userId, { active: !user.active });
      await loadData(selectedTenant);
    } catch (toggleError) {
      setError(getErrorMessage(toggleError));
    } finally {
      setSaving(false);
    }
  };

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Usuarios"
        subtitle={
          selectedTenant
            ? `Gestionando usuarios de ${selectedTenantName ?? selectedTenant}.`
            : 'Vista global de usuarios. Selecciona un tenant para altas y ediciones.'
        }
        actions={
          <Stack direction="row" spacing={1}>
            <Tooltip title="Refrescar">
              <span>
                <IconButton color="primary" onClick={() => { void loadData(selectedTenant); }} disabled={loading}>
                  <RefreshIcon />
                </IconButton>
              </span>
            </Tooltip>
            <FormControl size="small" sx={{ minWidth: 220 }}>
              <InputLabel id="tenant-filter-label">Tenant</InputLabel>
              <Select
                labelId="tenant-filter-label"
                label="Tenant"
                value={selectedTenant}
                onChange={handleTenantChange}
              >
                <MenuItem value="">Todos</MenuItem>
                {tenants.map((tenant) => (
                  <MenuItem key={tenant.id} value={tenant.identifier}>
                    {tenant.name} ({tenant.identifier})
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
            <Button variant="contained" startIcon={<AddIcon />} onClick={openCreateDialog} disabled={!selectedTenant}>
              Nuevo usuario
            </Button>
          </Stack>
        }
      />

      {!selectedTenant ? (
        <Alert severity="info">
          Selecciona un tenant para crear o editar usuarios. La vista global es solo informativa.
        </Alert>
      ) : null}
      {error ? <Alert severity="error">{error}</Alert> : null}

      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Usuario</TableCell>
            <TableCell>Email</TableCell>
            <TableCell>Roles</TableCell>
            <TableCell>Tenant</TableCell>
            <TableCell>Activo</TableCell>
            <TableCell align="right">Acciones</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {users.map((user) => (
            <TableRow key={`${user.tenantIdentifier ?? 'global'}-${user.userId}`} hover>
              <TableCell>
                <Stack spacing={0.5}>
                  <Typography fontWeight={600}>{user.username}</Typography>
                  <Typography variant="body2" color="text.secondary">
                    {user.name} {user.lastName}
                  </Typography>
                </Stack>
              </TableCell>
              <TableCell>{user.email}</TableCell>
              <TableCell>
                <Stack direction="row" spacing={0.5} flexWrap="wrap" useFlexGap>
                  {user.roles.length > 0 ? (
                    user.roles.map((role) => <Chip key={role} label={role} size="small" variant="outlined" />)
                  ) : (
                    <Chip label="Sin roles" size="small" variant="outlined" />
                  )}
                </Stack>
              </TableCell>
              <TableCell>{user.tenantName ?? user.tenantIdentifier ?? 'N/A'}</TableCell>
              <TableCell>
                <Switch
                  checked={user.active}
                  onChange={() => { void handleToggleActive(user); }}
                  disabled={saving || !user.tenantIdentifier}
                />
              </TableCell>
              <TableCell align="right">
                <Tooltip title={selectedTenant ? 'Editar usuario' : 'Selecciona un tenant para editar'}>
                  <span>
                    <IconButton size="small" onClick={() => openEditDialog(user)} disabled={!selectedTenant}>
                      <EditIcon fontSize="small" />
                    </IconButton>
                  </span>
                </Tooltip>
              </TableCell>
            </TableRow>
          ))}
          {users.length === 0 ? (
            <TableRow>
              <TableCell colSpan={6}>
                <Box py={3} textAlign="center">
                  <Typography variant="body2" color="text.secondary">
                    No hay usuarios para mostrar.
                  </Typography>
                </Box>
              </TableCell>
            </TableRow>
          ) : null}
        </TableBody>
      </Table>

      <Dialog open={Boolean(dialogState)} onClose={closeDialog} maxWidth="sm" fullWidth>
        <DialogTitle>{dialogState?.mode === 'edit' ? 'Editar usuario' : 'Nuevo usuario'}</DialogTitle>
        <DialogContent>
          <Stack spacing={2} mt={1}>
            <TextField
              label="Nombre"
              value={formState.name}
              onChange={(event) => setFormState((current) => ({ ...current, name: event.target.value }))}
              fullWidth
              required
            />
            <TextField
              label="Apellidos"
              value={formState.lastName}
              onChange={(event) => setFormState((current) => ({ ...current, lastName: event.target.value }))}
              fullWidth
              required
            />
            <TextField
              label="Username"
              value={formState.username}
              onChange={(event) => setFormState((current) => ({ ...current, username: event.target.value }))}
              fullWidth
              required
              disabled={dialogState?.mode === 'edit'}
            />
            <TextField
              label="Email"
              type="email"
              value={formState.email}
              onChange={(event) => setFormState((current) => ({ ...current, email: event.target.value }))}
              fullWidth
              required
            />
              <TextField
                label="Roles"
                value={formState.roles}
                onChange={(event) => setFormState((current) => ({ ...current, roles: event.target.value }))}
                helperText={`Separados por coma. Roles válidos: ${TENANT_ADMIN_ROLE}, ${TENANT_USER_ROLE}`}
                fullWidth
              />
            <Stack direction="row" justifyContent="space-between" alignItems="center">
              <Box>
                <Typography fontWeight={600}>Usuario activo</Typography>
                <Typography variant="body2" color="text.secondary">
                  Controla el acceso del usuario dentro del tenant seleccionado.
                </Typography>
              </Box>
              <Switch
                checked={formState.active}
                onChange={(event) => setFormState((current) => ({ ...current, active: event.target.checked }))}
              />
            </Stack>
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={closeDialog}>Cancelar</Button>
          <Button variant="contained" onClick={() => { void handleSave(); }} disabled={saving || !selectedTenant}>
            {saving ? 'Guardando...' : dialogState?.mode === 'edit' ? 'Guardar cambios' : 'Crear usuario'}
          </Button>
        </DialogActions>
      </Dialog>
    </Stack>
  );
};
