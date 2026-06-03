import React, { useState, useEffect } from 'react';
import { Plus, Edit, Trash2 } from 'lucide-react';
import { buildAuthAssetUrl } from '../api/tenantPath';
import { usersApi } from '../api/users.api';
import { User, RegisterUser } from '../types';
import { normalizeRoles } from '../auth/roles';
import { LoadingSpinner } from '../components/LoadingSpinner';


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
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });

  useEffect(() => {
    if (snackbar.open) {
      const t = setTimeout(() => setSnackbar((s) => ({ ...s, open: false })), 6000);
      return () => clearTimeout(t);
    }
  }, [snackbar.open]);

  useEffect(() => {
    void loadUsers();
  }, []);

  const loadUsers = async () => {
    try {
      setLoading(true);
      const data = await usersApi.list();
      setUsers(data.map((user) => ({ ...user, roles: normalizeRoles(user.roles) })));
    } catch {
      setSnackbar({ open: true, message: 'Error loading users', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (user?: User) => {
    if (user) {
      setEditingUser(user);
      setFormData({ name: user.name, lastName: user.lastName, username: user.username, email: user.email, password: '' });
    } else {
      setEditingUser(null);
      setFormData({ name: '', lastName: '', username: '', email: '', password: '' });
    }
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setEditingUser(null);
  };

  const handleSave = async () => {
    try {
      if (editingUser) {
        await usersApi.edit(editingUser.userId, formData.name, formData.lastName);
        setSnackbar({ open: true, message: 'Usuario actualizado exitosamente', severity: 'success' });
      } else {
        await usersApi.create(formData);
        setSnackbar({ open: true, message: 'Usuario creado exitosamente', severity: 'success' });
      }
      handleCloseDialog();
      void loadUsers();
    } catch {
      setSnackbar({ open: true, message: 'Error al guardar usuario', severity: 'error' });
    }
  };

  const handleToggleActive = async (userId: string) => {
    try {
      await usersApi.toggleActive(userId);
      setSnackbar({ open: true, message: 'Estado del usuario actualizado', severity: 'success' });
      void loadUsers();
    } catch {
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
      void loadUsers();
    } catch {
      setSnackbar({ open: true, message: 'Error al eliminar usuario', severity: 'error' });
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">Usuarios</h1>
        <button className="btn-primary flex items-center gap-1.5" onClick={() => handleOpenDialog()}>
          <Plus size={18} />
          Agregar Usuario
        </button>
      </div>

      <div className="overflow-auto rounded-md border border-gray-200">
        <table className="w-full text-sm">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-4 py-2 text-left font-medium text-gray-600">Foto</th>
              <th className="px-4 py-2 text-left font-medium text-gray-600">Usuario</th>
              <th className="px-4 py-2 text-left font-medium text-gray-600">Nombre</th>
              <th className="px-4 py-2 text-left font-medium text-gray-600">Email</th>
              <th className="px-4 py-2 text-left font-medium text-gray-600">Roles</th>
              <th className="px-4 py-2 text-left font-medium text-gray-600">Activo</th>
              <th className="px-4 py-2 text-right font-medium text-gray-600">Acciones</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {users.map((user) => (
              <tr key={user.userId} className="hover:bg-gray-50">
                <td className="px-4 py-2">
                  <div className="w-9 h-9 rounded-full overflow-hidden bg-blue-100 flex items-center justify-center text-blue-700 font-semibold text-sm">
                    {user.profilePicture ? (
                      <img
                        src={buildAuthAssetUrl(`/api/uploads/${user.profilePicture}`)}
                        alt={user.name}
                        className="w-full h-full object-cover"
                      />
                    ) : (
                      user.name.charAt(0).toUpperCase()
                    )}
                  </div>
                </td>
                <td className="px-4 py-2">{user.username}</td>
                <td className="px-4 py-2">{user.name} {user.lastName}</td>
                <td className="px-4 py-2">{user.email}</td>
                <td className="px-4 py-2">
                  {user.roles && user.roles.length > 0 ? (
                    <div className="flex flex-wrap gap-1">
                      {user.roles.map((role) => (
                        <span key={role} className="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                          {role}
                        </span>
                      ))}
                    </div>
                  ) : (
                    <span className="text-gray-400 text-xs">Sin roles</span>
                  )}
                </td>
                <td className="px-4 py-2">
                  <input
                    type="checkbox"
                    checked={user.active}
                    onChange={() => void handleToggleActive(user.userId)}
                    title={user.active ? 'Desactivar usuario' : 'Activar usuario'}
                    className="h-4 w-4 rounded text-blue-600 cursor-pointer"
                  />
                </td>
                <td className="px-4 py-2 text-right">
                  <div className="flex items-center justify-end gap-1">
                    <button
                      title="Editar"
                      className="p-1 rounded hover:bg-gray-100 text-blue-600"
                      onClick={() => handleOpenDialog(user)}
                      aria-label="Editar"
                    >
                      <Edit size={16} />
                    </button>
                    <button
                      title="Eliminar"
                      className="p-1 rounded hover:bg-gray-100 text-red-600"
                      onClick={() => handleOpenConfirmDialog(user.userId)}
                      aria-label="Eliminar"
                    >
                      <Trash2 size={16} />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Edit / Create Dialog */}
      {dialogOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
          <div className="bg-white rounded-lg shadow-xl w-full max-w-sm">
            <div className="px-5 py-4 border-b border-gray-200">
              <h2 className="text-lg font-semibold">{editingUser ? 'Editar Usuario' : 'Agregar Usuario'}</h2>
            </div>
            <div className="px-5 py-4 flex flex-col gap-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Nombre *</label>
                <input className="input-field" value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} required />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Apellido *</label>
                <input className="input-field" value={formData.lastName} onChange={(e) => setFormData({ ...formData, lastName: e.target.value })} required />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Usuario *</label>
                <input className="input-field" value={formData.username} onChange={(e) => setFormData({ ...formData, username: e.target.value })} disabled={!!editingUser} required />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Email *</label>
                <input className="input-field" type="email" value={formData.email} onChange={(e) => setFormData({ ...formData, email: e.target.value })} disabled={!!editingUser} required />
              </div>
              <div className="rounded-md bg-blue-50 border border-blue-200 px-3 py-2 text-xs text-blue-700">
                TenantAdmin gestiona usuarios y permisos del tenant desde esta pantalla. SuperAdmin solo existe en el portal administrativo.
              </div>
              {!editingUser && (
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Contraseña *</label>
                  <input className="input-field" type="password" value={formData.password} onChange={(e) => setFormData({ ...formData, password: e.target.value })} required />
                </div>
              )}
            </div>
            <div className="px-5 py-3 border-t border-gray-200 flex justify-end gap-2">
              <button className="btn-secondary" onClick={handleCloseDialog}>Cancelar</button>
              <button className="btn-primary" onClick={() => void handleSave()}>Guardar</button>
            </div>
          </div>
        </div>
      )}

      {/* Confirm Delete Dialog */}
      {confirmDialogOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
          <div className="bg-white rounded-lg shadow-xl w-full max-w-xs">
            <div className="px-5 py-4 border-b border-gray-200">
              <h2 className="text-lg font-semibold">Confirmar Eliminación</h2>
            </div>
            <div className="px-5 py-4">
              <p className="text-sm text-gray-700">¿Está seguro de que desea eliminar este usuario?</p>
            </div>
            <div className="px-5 py-3 border-t border-gray-200 flex justify-end gap-2">
              <button className="btn-secondary" onClick={handleCloseConfirmDialog}>Cancelar</button>
              <button className="btn-danger" onClick={() => void handleConfirmDelete()}>Eliminar</button>
            </div>
          </div>
        </div>
      )}

      {/* Snackbar */}
      {snackbar.open && (
        <div className={`fixed bottom-4 left-1/2 -translate-x-1/2 z-50 px-4 py-3 rounded-md shadow-lg text-sm font-medium ${snackbar.severity === 'success' ? 'bg-green-700 text-white' : 'bg-red-700 text-white'}`}>
          {snackbar.message}
        </div>
      )}
    </div>
  );
};


