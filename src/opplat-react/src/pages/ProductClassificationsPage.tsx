import React, { useState, useEffect } from 'react';
import { Plus, Pencil, Trash2 } from 'lucide-react';
import { inventoryApi } from '../api/inventory.api';
import type { ProductClassification } from '../types';
import { LoadingSpinner } from '../components/LoadingSpinner';

type ClassificationFormData = Omit<ProductClassification, 'id'>;

const emptyForm: ClassificationFormData = {
  description: '',
};

export const ProductClassificationsPage: React.FC = () => {
  const [classifications, setClassifications] = useState<ProductClassification[]>([]);
  const [loading, setLoading] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [confirmDialogOpen, setConfirmDialogOpen] = useState(false);
  const [editingClassification, setEditingClassification] = useState<ProductClassification | null>(null);
  const [deletingClassification, setDeletingClassification] = useState<ProductClassification | null>(null);
  const [formData, setFormData] = useState<ClassificationFormData>(emptyForm);
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: '',
    severity: 'success' as 'success' | 'error',
  });

  useEffect(() => {
    void loadClassifications();
  }, []);

  const loadClassifications = async () => {
    try {
      setLoading(true);
      const res = await inventoryApi.listClassifications();
      setClassifications(res.data);
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (classification?: ProductClassification) => {
    if (classification) {
      setEditingClassification(classification);
      setFormData({ description: classification.description });
    } else {
      setEditingClassification(null);
      setFormData(emptyForm);
    }
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setEditingClassification(null);
  };

  const handleSave = async () => {
    try {
      if (editingClassification) {
        await inventoryApi.updateClassification({ ...formData, id: editingClassification.id });
      } else {
        await inventoryApi.createClassification(formData);
      }
      setSnackbar({ open: true, message: 'Clasificación guardada correctamente', severity: 'success' });
      handleCloseDialog();
      void loadClassifications();
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    }
  };

  const handleOpenConfirmDialog = (classification: ProductClassification) => {
    setDeletingClassification(classification);
    setConfirmDialogOpen(true);
  };

  const handleCloseConfirmDialog = () => {
    setConfirmDialogOpen(false);
    setDeletingClassification(null);
  };

  const handleConfirmDelete = async () => {
    if (!deletingClassification) return;
    try {
      await inventoryApi.deleteClassification(deletingClassification.id);
      setSnackbar({ open: true, message: 'Clasificación eliminada correctamente', severity: 'success' });
      handleCloseConfirmDialog();
      void loadClassifications();
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    }
  };

  if (loading) return <LoadingSpinner />;

  useEffect(() => {
    if (!snackbar.open) return;
    const t = setTimeout(() => setSnackbar((s) => ({ ...s, open: false })), 6000);
    return () => clearTimeout(t);
  }, [snackbar.open]);

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">Clasificaciones de Productos</h1>
        <button className="btn-primary flex items-center gap-2" onClick={() => handleOpenDialog()}>
          <Plus size={16} /> Nueva Clasificación
        </button>
      </div>

      <div className="card overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Descripción</th>
              <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Acciones</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200 bg-white">
            {classifications.length === 0 ? (
              <tr>
                <td colSpan={2} className="px-4 py-8 text-center text-sm text-gray-500">No se encontraron clasificaciones.</td>
              </tr>
            ) : (
              classifications.map((c) => (
                <tr key={c.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3 text-sm">{c.description}</td>
                  <td className="px-4 py-3 text-right">
                    <button title="Editar" className="p-1 text-blue-600 hover:bg-blue-50 rounded mr-1" onClick={() => handleOpenDialog(c)}>
                      <Pencil size={16} />
                    </button>
                    <button title="Eliminar" className="p-1 text-red-600 hover:bg-red-50 rounded" onClick={() => handleOpenConfirmDialog(c)}>
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
              <h2 className="text-lg font-semibold">{editingClassification ? 'Editar Clasificación' : 'Nueva Clasificación'}</h2>
            </div>
            <div className="px-6 py-4">
              <label className="block text-sm font-medium text-gray-700 mb-1">Descripción <span className="text-red-500">*</span></label>
              <input
                className="input-field"
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                required
              />
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
              {`¿Eliminar clasificación '${deletingClassification?.description}'?`}
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
