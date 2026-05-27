import React, { useState, useEffect } from 'react';
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
  Chip,
  Alert,
  Snackbar,
  Switch,
  Avatar,
  Tooltip,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
} from '@mui/material';
import { Add as AddIcon, Edit as EditIcon, Delete as DeleteIcon } from '@mui/icons-material';
import { buildAuthAssetUrl } from '../api/tenantPath';
import { usersApi } from '../api/users.api';
import { User, RegisterUser } from '../types';
import { LoadingSpinner } from '../components/LoadingSpinner';
import { TENANT_ADMIN_ROLE, TENANT_USER_ROLE, normalizeRoles } from '../auth/roles';

type TenantRole = typeof TENANT_ADMIN_ROLE | typeof TENANT_USER_ROLE;

const getEditableTenantRole = (roles: string[]): TenantRole =>
  normalizeRoles(roles).includes(TENANT_ADMIN_ROLE) ? TENANT_ADMIN_ROLE : TENANT_USER_ROLE;

export const UsersPage: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [confirmDialogOpen, setConfirmDialogOpen] = useState(false);
  const [editingUser, setEditingUser] = useState<User | null>(null);
  const [deletingUserId, setDeletingUserId] = useState<string | null>(null);
  const [formData, setFormData] = useState<RegisterUser>({
    name: '',
    lastName: '',
    username: '',
    email: '',
    password: '',
  });
  const [selectedRole, setSelectedRole] = useState<TenantRole>(TENANT_USER_ROLE);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });

  useEffect(() => {
    loadUsers();
  }, []);

  const loadUsers = async () => {
    try {
      setLoading(true);
      const data = await usersApi.list();
      setUsers(data.map((user) => ({ ...user, roles: normalizeRoles(user.roles) })));
    } catch (error) {
      setSnackbar({ open: true, message: 'Error loading users', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (user?: User) => {
    if (user) {
      setEditingUser(user);
      setFormData({
        name: user.name,
        lastName: user.lastName,
        username: user.username,
        email: user.email,
        password: '',
      });
      setSelectedRole(getEditableTenantRole(user.roles));
    } else {
      setEditingUser(null);
      setFormData({
        name: '',
        lastName: '',
        username: '',
        email: '',
        password: '',
      });
      setSelectedRole(TENANT_USER_ROLE);
    }
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setEditingUser(null);
    setSelectedRole(TENANT_USER_ROLE);
  };

  const handleSave = async () => {
    try {
      if (editingUser) {
        await usersApi.edit(editingUser.userId, formData.name, formData.lastName);
        setSnackbar({ open: true, message: 'Usuario actualizado exitosamente', severity: 'success' });
      } else {
        const createdUser = await usersApi.create(formData);
        setSnackbar({ open: true, message: 'Usuario creado exitosamente', severity: 'success' });
      }
      handleCloseDialog();
      loadUsers();
    } catch (error) {
      setSnackbar({ open: true, message: 'Error al guardar usuario', severity: 'error' });
    }
  };

  const handleToggleActive = async (userId: string) => {
    try {
      await usersApi.toggleActive(userId);
      setSnackbar({ open: true, message: 'Estado del usuario actualizado', severity: 'success' });
      loadUsers();
    } catch (error) {
      setSnackbar({ open: true, message: 'Error al actualizar estado del usuario', severity: 'error' });
    }
  };

  const handleOpenConfirmDialog = (userId: string) => {
    setDeletingUserId(userId);
    setConfirmDialogOpen(true);
  };

  const handleCloseConfirmDialog = () => {
    setConfirmDialogOpen(false);
    setDeletingUserId(null);
  };

  const handleConfirmDelete = async () => {
    if (!deletingUserId) return;

    try {
      await usersApi.delete(deletingUserId);
      setSnackbar({ open: true, message: 'Usuario eliminado exitosamente', severity: 'success' });
      handleCloseConfirmDialog();
      loadUsers();
    } catch (error) {
      setSnackbar({ open: true, message: 'Error al eliminar usuario', severity: 'error' });
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Usuarios</Typography>
        <Button
          variant="contained"
          color="primary"
          startIcon={<AddIcon />}
          onClick={() => handleOpenDialog()}
        >
          Agregar Usuario
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Foto</TableCell>
              <TableCell>Usuario</TableCell>
              <TableCell>Nombre</TableCell>
              <TableCell>Email</TableCell>
              <TableCell>Roles</TableCell>
              <TableCell>Activo</TableCell>
              <TableCell align="right">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {users.map((user) => (
              <TableRow key={user.userId}>
                <TableCell>
                  <Avatar
                    src={user.profilePicture ? buildAuthAssetUrl(`/api/uploads/${user.profilePicture}`) : undefined}
                    alt={user.name}
                  >
                    {!user.profilePicture && user.name.charAt(0).toUpperCase()}
                  </Avatar>
                </TableCell>
                <TableCell>{user.username}</TableCell>
                <TableCell>
                  {user.name} {user.lastName}
                </TableCell>
                <TableCell>{user.email}</TableCell>
                <TableCell>
                  {user.roles && user.roles.length > 0 ? (
                    user.roles.map((role) => (
                      <Chip key={role} label={role} size="small" sx={{ mr: 0.5 }} />
                    ))
                  ) : (
                    <Typography variant="body2" color="textSecondary">
                      Sin roles
                    </Typography>
                  )}
                </TableCell>
                <TableCell>
                  <Tooltip title={user.active ? 'Desactivar usuario' : 'Activar usuario'}>
                    <Switch
                      checked={user.active}
                      onChange={() => handleToggleActive(user.userId)}
                      color="primary"
                    />
                  </Tooltip>
                </TableCell>
                <TableCell align="right">
                  <Tooltip title="Editar">
                    <IconButton
                      color="primary"
                      onClick={() => handleOpenDialog(user)}
                      size="small"
                    >
                      <EditIcon />
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Eliminar">
                    <IconButton
                      color="error"
                      onClick={() => handleOpenConfirmDialog(user.userId)}
                      size="small"
                    >
                      <DeleteIcon />
                    </IconButton>
                  </Tooltip>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <Dialog open={dialogOpen} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
        <DialogTitle>{editingUser ? 'Editar Usuario' : 'Agregar Usuario'}</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            label="Nombre"
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            margin="normal"
            required
          />
          <TextField
            fullWidth
            label="Apellido"
            value={formData.lastName}
            onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
            margin="normal"
            required
          />
          <TextField
            fullWidth
            label="Usuario"
            value={formData.username}
            onChange={(e) => setFormData({ ...formData, username: e.target.value })}
            margin="normal"
            required
            disabled={!!editingUser}
          />
          <TextField
            fullWidth
            label="Email"
            type="email"
            value={formData.email}
            onChange={(e) => setFormData({ ...formData, email: e.target.value })}
            margin="normal"
            required
            disabled={!!editingUser}
          />
          <Alert severity="info" sx={{ mt: 2 }}>
            TenantAdmin gestiona usuarios y permisos del tenant desde esta pantalla. SuperAdmin solo existe en el portal administrativo.
          </Alert>
          {!editingUser && (
            <TextField
              fullWidth
              label="Contraseña"
              type="password"
              value={formData.password}
              onChange={(e) => setFormData({ ...formData, password: e.target.value })}
              margin="normal"
              required
            />
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancelar</Button>
          <Button onClick={handleSave} variant="contained" color="primary">
            Guardar
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={confirmDialogOpen} onClose={handleCloseConfirmDialog}>
        <DialogTitle>Confirmar Eliminación</DialogTitle>
        <DialogContent>
          <Typography>¿Está seguro de que desea eliminar este usuario?</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseConfirmDialog}>Cancelar</Button>
          <Button onClick={handleConfirmDelete} variant="contained" color="error">
            Eliminar
          </Button>
        </DialogActions>
      </Dialog>

      <Snackbar
        open={snackbar.open}
        autoHideDuration={6000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
      >
        <Alert severity={snackbar.severity} onClose={() => setSnackbar({ ...snackbar, open: false })}>
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
};
