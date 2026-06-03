import React, { useState, useEffect } from 'react';
import { Plus } from 'lucide-react';
import { inventoryApi } from '../api/inventory.api';
import type { CreateMovementData } from '../api/inventory.api';
import type { InventoryProduct, ProductMovement, Warehouse } from '../types';

const movementBadgeClass = (type: string): string => {
  switch (type.toLowerCase()) {
    case 'in':
    case 'entrada':
      return 'bg-green-100 text-green-800';
    case 'out':
    case 'salida':
      return 'bg-red-100 text-red-800';
    case 'adjustment':
    case 'ajuste':
      return 'bg-yellow-100 text-yellow-800';
    default:
      return 'bg-gray-100 text-gray-700';
  }
};

const MOVEMENT_TYPES = [
  { value: 'in', label: 'Entrada' },
  { value: 'out', label: 'Salida' },
  { value: 'adjustment', label: 'Ajuste' },
];

export const InventoryPage: React.FC = () => {
  const [tab, setTab] = useState(0);
  const [products, setProducts] = useState<InventoryProduct[]>([]);
  const [movements, setMovements] = useState<ProductMovement[]>([]);
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [movementDialogOpen, setMovementDialogOpen] = useState(false);
  const [movementForm, setMovementForm] = useState<CreateMovementData>({
    productId: 0,
    storageId: 0,
    quantity: 1,
    type: '',
    observations: '',
  });
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: '',
    severity: 'success' as 'success' | 'error',
  });

  useEffect(() => {
    if (snackbar.open) {
      const t = setTimeout(() => setSnackbar((s) => ({ ...s, open: false })), 6000);
      return () => clearTimeout(t);
    }
  }, [snackbar.open]);

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);
        const [prodRes, movRes, whRes] = await Promise.all([
          inventoryApi.getProducts(),
          inventoryApi.getMovements(),
          inventoryApi.listWarehouses(),
        ]);
        setProducts(prodRes.data);
        setMovements(movRes.data);
        setWarehouses(whRes.data);
      } catch {
        setError('Failed to load inventory data.');
      } finally {
        setLoading(false);
      }
    };
    void loadData();
  }, []);

  const loadMovements = async () => {
    try {
      const res = await inventoryApi.getMovements();
      setMovements(res.data);
    } catch {
      setSnackbar({ open: true, message: 'Error al cargar movimientos', severity: 'error' });
    }
  };

  const handleOpenMovementDialog = () => {
    setMovementForm({ productId: 0, storageId: 0, quantity: 1, type: '', observations: '' });
    setMovementDialogOpen(true);
  };

  const handleSaveMovement = async () => {
    try {
      await inventoryApi.createMovement(movementForm);
      setSnackbar({ open: true, message: 'Movimiento registrado correctamente', severity: 'success' });
      setMovementDialogOpen(false);
      void loadMovements();
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    }
  };

  if (loading) {
    return (
      <div className="flex justify-center pt-16">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
      </div>
    );
  }

  return (
    <div>
      <h1 className="text-2xl font-bold mb-4">Inventory</h1>

      {error && (
        <div className="bg-red-50 border border-red-300 text-red-700 px-4 py-3 rounded-md text-sm mb-4">
          {error}
        </div>
      )}

      {/* Tabs */}
      <div className="flex border-b border-gray-200 mb-4">
        {['Productos', 'Movimientos'].map((label, i) => (
          <button
            key={label}
            className={`px-4 py-2 text-sm font-medium -mb-px border-b-2 transition-colors ${tab === i ? 'border-blue-600 text-blue-600' : 'border-transparent text-gray-500 hover:text-gray-700'}`}
            onClick={() => setTab(i)}
          >
            {label}
          </button>
        ))}
      </div>

      {/* Products Tab */}
      {tab === 0 && (
        <div className="overflow-auto rounded-md border border-gray-200">
          <table className="w-full text-sm">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-4 py-2 text-left font-medium text-gray-600">Name</th>
                <th className="px-4 py-2 text-left font-medium text-gray-600">Group</th>
                <th className="px-4 py-2 text-left font-medium text-gray-600">Unit</th>
                <th className="px-4 py-2 text-right font-medium text-gray-600">Total Stock</th>
                <th className="px-4 py-2 text-left font-medium text-gray-600">Status</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {products.map((p) => (
                <tr key={p.id} className="hover:bg-gray-50">
                  <td className="px-4 py-2">{p.nombre}</td>
                  <td className="px-4 py-2">{p.grupo.descripcion}</td>
                  <td className="px-4 py-2">{p.unidadDeMedida.siglas}</td>
                  <td className="px-4 py-2 text-right">{p.existenciaTotal ?? 0}</td>
                  <td className="px-4 py-2">
                    <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${p.active ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-600'}`}>
                      {p.active ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                </tr>
              ))}
              {products.length === 0 && (
                <tr>
                  <td colSpan={5} className="px-4 py-8 text-center text-gray-400">No products found.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}

      {/* Movements Tab */}
      {tab === 1 && (
        <div>
          <div className="flex justify-end mb-3">
            <button className="btn-primary flex items-center gap-1.5" onClick={handleOpenMovementDialog}>
              <Plus size={18} />
              Nuevo Movimiento
            </button>
          </div>
          <div className="overflow-auto rounded-md border border-gray-200">
            <table className="w-full text-sm">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Fecha</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Producto ID</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Almacén ID</th>
                  <th className="px-4 py-2 text-right font-medium text-gray-600">Cantidad</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Tipo</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Observaciones</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {movements.map((m) => (
                  <tr key={m.id} className="hover:bg-gray-50">
                    <td className="px-4 py-2">{new Date(m.date).toLocaleDateString()}</td>
                    <td className="px-4 py-2">{m.productId}</td>
                    <td className="px-4 py-2">{m.storageId}</td>
                    <td className="px-4 py-2 text-right">{m.quantity}</td>
                    <td className="px-4 py-2">
                      <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${movementBadgeClass(m.type)}`}>
                        {m.type}
                      </span>
                    </td>
                    <td className="px-4 py-2">{m.observations || '—'}</td>
                  </tr>
                ))}
                {movements.length === 0 && (
                  <tr>
                    <td colSpan={6} className="px-4 py-8 text-center text-gray-400">No movements found.</td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {/* New Movement Dialog */}
      {movementDialogOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
          <div className="bg-white rounded-lg shadow-xl w-full max-w-sm">
            <div className="px-5 py-4 border-b border-gray-200">
              <h2 className="text-lg font-semibold">Nuevo Movimiento</h2>
            </div>
            <div className="px-5 py-4 flex flex-col gap-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Almacén *</label>
                <select
                  className="input-field"
                  value={movementForm.storageId || ''}
                  onChange={(e) => setMovementForm({ ...movementForm, storageId: Number(e.target.value) })}
                  required
                >
                  <option value="">— seleccionar —</option>
                  {warehouses.map((w) => (
                    <option key={w.id} value={w.id}>{w.description}</option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Producto *</label>
                <select
                  className="input-field"
                  value={movementForm.productId || ''}
                  onChange={(e) => setMovementForm({ ...movementForm, productId: Number(e.target.value) })}
                  required
                >
                  <option value="">— seleccionar —</option>
                  {products.map((p) => (
                    <option key={p.id} value={p.id}>{p.nombre}</option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Tipo *</label>
                <select
                  className="input-field"
                  value={movementForm.type}
                  onChange={(e) => setMovementForm({ ...movementForm, type: e.target.value })}
                  required
                >
                  <option value="">— seleccionar —</option>
                  {MOVEMENT_TYPES.map((t) => (
                    <option key={t.value} value={t.value}>{t.label}</option>
                  ))}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Cantidad *</label>
                <input
                  className="input-field"
                  type="number"
                  min={1}
                  value={movementForm.quantity}
                  onChange={(e) => setMovementForm({ ...movementForm, quantity: Math.max(1, parseInt(e.target.value) || 1) })}
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Observaciones</label>
                <textarea
                  className="input-field"
                  rows={3}
                  value={movementForm.observations}
                  onChange={(e) => setMovementForm({ ...movementForm, observations: e.target.value })}
                />
              </div>
            </div>
            <div className="px-5 py-3 border-t border-gray-200 flex justify-end gap-2">
              <button className="btn-secondary" onClick={() => setMovementDialogOpen(false)}>Cancelar</button>
              <button className="btn-primary" onClick={() => void handleSaveMovement()}>Guardar</button>
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


