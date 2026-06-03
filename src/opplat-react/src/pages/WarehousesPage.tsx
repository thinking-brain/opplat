import React, { useState, useEffect } from 'react';
import { Plus, Pencil, Trash2 } from 'lucide-react';
import { inventoryApi } from '../api/inventory.api';
import type { Warehouse } from '../types';
import { LoadingSpinner } from '../components/LoadingSpinner';

type WarehouseFormData = Omit<Warehouse, 'id'>;

const emptyForm: WarehouseFormData = {
  code: '',
  description: '',
  isCostCenter: true,
};

export const WarehousesPage: React.FC = () => {
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [filteredWarehouses, setFilteredWarehouses] = useState<Warehouse[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [confirmDialogOpen, setConfirmDialogOpen] = useState(false);
  const [editingWarehouse, setEditingWarehouse] = useState<Warehouse | null>(null);
  const [deletingWarehouse, setDeletingWarehouse] = useState<Warehouse | null>(null);
  const [formData, setFormData] = useState<WarehouseFormData>(emptyForm);
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: '',
    severity: 'success' as 'success' | 'error',
  });

  useEffect(() => {
    void loadWarehouses();
  }, []);

  useEffect(() => {
    const lower = searchTerm.toLowerCase();
    setFilteredWarehouses(
      warehouses.filter(
        (w) =>
          w.description.toLowerCase().includes(lower) ||
          w.code.toLowerCase().includes(lower),
      ),
    );
  }, [searchTerm, warehouses]);

  const loadWarehouses = async () => {
    try {
      setLoading(true);
      const res = await inventoryApi.listWarehouses();
      setWarehouses(res.data);
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (warehouse?: Warehouse) => {
    if (warehouse) {
      setEditingWarehouse(warehouse);
      setFormData({ code: warehouse.code, description: warehouse.description, isCostCenter: warehouse.isCostCenter });
    } else {
      setEditingWarehouse(null);
      setFormData(emptyForm);
    }
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setEditingWarehouse(null);
  };

  const handleSave = async () => {
    try {
      if (editingWarehouse) {
        await inventoryApi.updateWarehouse({ ...formData, id: editingWarehouse.id });
      } else {
        await inventoryApi.createWarehouse(formData);
      }
      setSnackbar({ open: true, message: 'Almacén guardado correctamente', severity: 'success' });
      handleCloseDialog();
      void loadWarehouses();
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    }
  };

  const handleOpenConfirmDialog = (warehouse: Warehouse) => {
    setDeletingWarehouse(warehouse);
    setConfirmDialogOpen(true);
  };

  const handleCloseConfirmDialog = () => {
    setConfirmDialogOpen(false);
    setDeletingWarehouse(null);
  };

  const handleConfirmDelete = async () => {
    if (!deletingWarehouse) return;
    try {
      await inventoryApi.deleteWarehouse(deletingWarehouse.id);
      setSnackbar({ open: true, message: 'Almacén eliminado correctamente', severity: 'success' });
      handleCloseConfirmDialog();
      void loadWarehouses();
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
        <h1 className="text-2xl font-bold">Almacenes</h1>
        <button className="btn-primary flex items-center gap-2" onClick={() => handleOpenDialog()}>
          <Plus size={16} /> Nuevo Almacén
        </button>
      </div>

      <div className="mb-4">
        <input
          className="input-field"
          placeholder="Buscar almacenes..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
      </div>

      <div className="card overflow-hidden">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Código</th>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Descripción</th>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase">Centro de Costo</th>
              <th className="px-4 py-3 text-right text-xs font-medium text-gray-500 uppercase">Acciones</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-200 bg-white">
            {filteredWarehouses.length === 0 ? (
              <tr>
                <td colSpan={4} className="px-4 py-8 text-center text-sm text-gray-500">No se encontraron almacenes.</td>
              </tr>
            ) : (
              filteredWarehouses.map((w) => (
                <tr key={w.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3 text-sm font-mono">{w.code}</td>
                  <td className="px-4 py-3 text-sm">{w.description}</td>
                  <td className="px-4 py-3">
                    <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${w.isCostCenter ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-600'}`}>
                      {w.isCostCenter ? 'Sí' : 'No'}
                    </span>
                  </td>
                  <td className="px-4 py-3 text-right">
                    <button title="Editar" className="p-1 text-blue-600 hover:bg-blue-50 rounded mr-1" onClick={() => handleOpenDialog(w)}>
                      <Pencil size={16} />
                    </button>
                    <button title="Eliminar" className="p-1 text-red-600 hover:bg-red-50 rounded" onClick={() => handleOpenConfirmDialog(w)}>
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
              <h2 className="text-lg font-semibold">{editingWarehouse ? 'Editar Almacén' : 'Nuevo Almacén'}</h2>
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
                <label className="block text-sm font-medium text-gray-700 mb-1">Código</label>
                <input
                  className="input-field"
                  value={formData.code}
                  onChange={(e) => setFormData({ ...formData, code: e.target.value })}
                />
              </div>
              <label className="flex items-center gap-2 cursor-pointer mt-1">
                <input
                  type="checkbox"
                  className="w-4 h-4 text-blue-600 rounded border-gray-300"
                  checked={formData.isCostCenter}
                  onChange={(e) => setFormData({ ...formData, isCostCenter: e.target.checked })}
                />
                <span className="text-sm font-medium text-gray-700">Centro de Costo</span>
              </label>
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
              {`¿Eliminar almacén '${deletingWarehouse?.description}'?`}
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
