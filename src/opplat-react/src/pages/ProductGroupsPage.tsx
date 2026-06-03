import React, { useState, useEffect } from 'react';
import { Plus, Pencil, Trash2 } from 'lucide-react';
import { inventoryApi } from '../api/inventory.api';
import type { ProductGroup, ProductClassification } from '../types';
import { LoadingSpinner } from '../components/LoadingSpinner';

type ProductGroupFormData = Omit<ProductGroup, 'id' | 'classification'>;

const emptyForm: ProductGroupFormData = {
  description: '',
  classificationId: 0,
};

export const ProductGroupsPage: React.FC = () => {
  const [groups, setGroups] = useState<ProductGroup[]>([]);
  const [classifications, setClassifications] = useState<ProductClassification[]>([]);
  const [loading, setLoading] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [confirmDialogOpen, setConfirmDialogOpen] = useState(false);
  const [editingGroup, setEditingGroup] = useState<ProductGroup | null>(null);
  const [deletingGroup, setDeletingGroup] = useState<ProductGroup | null>(null);
  const [formData, setFormData] = useState<ProductGroupFormData>(emptyForm);
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: '',
    severity: 'success' as 'success' | 'error',
  });

  useEffect(() => {
    void loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [groupsRes, classificationsRes] = await Promise.all([
        inventoryApi.listProductGroups(),
        inventoryApi.listClassifications(),
      ]);
      setGroups(groupsRes.data);
      setClassifications(classificationsRes.data);
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const loadGroups = async () => {
    try {
      const res = await inventoryApi.listProductGroups();
      setGroups(res.data);
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    }
  };

  const handleOpenDialog = (group?: ProductGroup) => {
    if (group) {
      setEditingGroup(group);
      setFormData({ description: group.description, classificationId: group.classificationId });
    } else {
      setEditingGroup(null);
      setFormData(emptyForm);
    }
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setEditingGroup(null);
  };

  const handleSave = async () => {
    try {
      if (editingGroup) {
        await inventoryApi.updateProductGroup({ ...formData, id: editingGroup.id });
      } else {
        await inventoryApi.createProductGroup(formData);
      }
      setSnackbar({ open: true, message: 'Grupo guardado correctamente', severity: 'success' });
      handleCloseDialog();
      void loadGroups();
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    }
  };

  const handleOpenConfirmDialog = (group: ProductGroup) => {
    setDeletingGroup(group);
    setConfirmDialogOpen(true);
  };

  const handleCloseConfirmDialog = () => {
    setConfirmDialogOpen(false);
    setDeletingGroup(null);
  };

  const handleConfirmDelete = async () => {
    if (!deletingGroup) return;
    try {
      await inventoryApi.deleteProductGroup(deletingGroup.id);
      setSnackbar({ open: true, message: 'Grupo eliminado correctamente', severity: 'success' });
      handleCloseConfirmDialog();
      void loadGroups();
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    }
  };

  useEffect(() => {
    if (!snackbar.open) return;
    const t = setTimeout(() => setSnackbar((s) => ({ ...s, open: false })), 6000);
    return () => clearTimeout(t);
  }, [snackbar.open]);

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">Grupos de Productos</h1>
        <button className="btn-primary flex items-center gap-2" onClick={() => handleOpenDialog()}>
          <Plus size={16} /> Nuevo Grupo
        </button>
      </div>

      <div className="card overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Descripción</th>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Clasificación</th>
              <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Acciones</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200 bg-white">
            {groups.length === 0 ? (
              <tr>
                <td colSpan={3} className="px-4 py-8 text-center text-sm text-gray-500">No se encontraron grupos.</td>
              </tr>
            ) : (
              groups.map((g) => (
                <tr key={g.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3 text-sm">{g.description}</td>
                  <td className="px-4 py-3 text-sm">{g.classification?.description ?? '—'}</td>
                  <td className="px-4 py-3 text-right">
                    <button title="Editar" className="p-1 text-blue-600 hover:bg-blue-50 rounded mr-1" onClick={() => handleOpenDialog(g)}>
                      <Pencil size={16} />
                    </button>
                    <button title="Eliminar" className="p-1 text-red-600 hover:bg-red-50 rounded" onClick={() => handleOpenConfirmDialog(g)}>
                      <Trash2 size={16} />
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>

      {/* Create / Edit Dialog */}
      {dialogOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
          <div className="bg-white rounded-lg shadow-xl w-full max-w-md">
            <div className="px-6 py-4 border-b">
              <h2 className="text-lg font-semibold">{editingGroup ? 'Editar Grupo' : 'Nuevo Grupo'}</h2>
            </div>
            <div className="px-6 py-4 flex flex-col gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Descripción <span className="text-red-500">*</span></label>
                <input
                  className="input-field"
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Clasificación <span className="text-red-500">*</span></label>
                <select
                  className="input-field"
                  value={formData.classificationId || ''}
                  onChange={(e) => setFormData({ ...formData, classificationId: Number(e.target.value) })}
                  required
                >
                  <option value="">Seleccionar clasificación</option>
                  {classifications.map((c) => (
                    <option key={c.id} value={c.id}>{c.description}</option>
                  ))}
                </select>
              </div>
            </div>
            <div className="flex justify-end gap-2 px-6 py-4 border-t">
              <button className="btn-ghost" onClick={handleCloseDialog}>Cancelar</button>
              <button className="btn-primary" onClick={() => void handleSave()}>Guardar</button>
            </div>
          </div>
        </div>
      )}

      {/* Delete Confirm Dialog */}
      {confirmDialogOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
          <div className="bg-white rounded-lg shadow-xl w-full max-w-sm">
            <div className="px-6 py-4 border-b">
              <h2 className="text-lg font-semibold">Confirmar Eliminación</h2>
            </div>
            <div className="px-6 py-4 text-sm">
              {`¿Eliminar grupo '${deletingGroup?.description}'?`}
            </div>
            <div className="flex justify-end gap-2 px-6 py-4 border-t">
              <button className="btn-ghost" onClick={handleCloseConfirmDialog}>Cancelar</button>
              <button className="btn-danger" onClick={() => void handleConfirmDelete()}>Eliminar</button>
            </div>
          </div>
        </div>
      )}

      {snackbar.open && (
        <div className={`fixed bottom-4 left-1/2 -translate-x-1/2 z-50 px-4 py-3 rounded-lg shadow-lg text-white text-sm flex items-center gap-2 ${snackbar.severity === 'success' ? 'bg-green-600' : 'bg-red-600'}`}>
          {snackbar.message}
          <button onClick={() => setSnackbar((s) => ({ ...s, open: false }))} className="ml-1 hover:opacity-75">✕</button>
        </div>
      )}
    </div>
  );
};
