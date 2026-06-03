import React, { useState, useEffect, useRef } from 'react';
import { Plus, Edit, Trash2, Image, History, ToggleRight, ToggleLeft } from 'lucide-react';
import { productsApi } from '../api/products.api';
import { buildSalesAssetUrl } from '../api/tenantPath';
import { ProductForSale } from '../types';
import { LoadingSpinner } from '../components/LoadingSpinner';

export const ProductsPage: React.FC = () => {
  const [products, setProducts] = useState<ProductForSale[]>([]);
  const [filteredProducts, setFilteredProducts] = useState<ProductForSale[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [confirmDialogOpen, setConfirmDialogOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState<ProductForSale | null>(null);
  const [deletingProductId, setDeletingProductId] = useState<string | null>(null);
  const [formData, setFormData] = useState<ProductForSale>({
    name: '',
    price: 0,
    stock: 0,
    description: '',
    active: true,
  });
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [uploadingProductId, setUploadingProductId] = useState<string | null>(null);

  useEffect(() => {
    if (snackbar.open) {
      const t = setTimeout(() => setSnackbar((s) => ({ ...s, open: false })), 6000);
      return () => clearTimeout(t);
    }
  }, [snackbar.open]);

  useEffect(() => {
    loadProducts();
  }, []);

  useEffect(() => {
    const filtered = products.filter((product) =>
      product.name.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredProducts(filtered);
  }, [searchTerm, products]);

  const loadProducts = async () => {
    try {
      setLoading(true);
      const data = await productsApi.list();
      setProducts(data);
      setFilteredProducts(data);
    } catch {
      setSnackbar({ open: true, message: 'Error al cargar productos', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (product?: ProductForSale) => {
    if (product) {
      setEditingProduct(product);
      setFormData(product);
    } else {
      setEditingProduct(null);
      setFormData({ name: '', price: 0, stock: 0, description: '', active: true });
    }
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setEditingProduct(null);
  };

  const handleSave = async () => {
    try {
      if (editingProduct) {
        await productsApi.update({ ...formData, id: editingProduct.id });
        setSnackbar({ open: true, message: 'Producto actualizado exitosamente', severity: 'success' });
      } else {
        await productsApi.create(formData);
        setSnackbar({ open: true, message: 'Producto creado exitosamente', severity: 'success' });
      }
      handleCloseDialog();
      void loadProducts();
    } catch {
      setSnackbar({ open: true, message: 'Error al guardar producto', severity: 'error' });
    }
  };

  const handleOpenConfirmDialog = (productId: string) => {
    setDeletingProductId(productId);
    setConfirmDialogOpen(true);
  };

  const handleCloseConfirmDialog = () => {
    setConfirmDialogOpen(false);
    setDeletingProductId(null);
  };

  const handleConfirmDelete = async () => {
    if (!deletingProductId) return;
    try {
      await productsApi.delete(deletingProductId);
      setSnackbar({ open: true, message: 'Producto eliminado exitosamente', severity: 'success' });
      handleCloseConfirmDialog();
      void loadProducts();
    } catch {
      setSnackbar({ open: true, message: 'Error al eliminar producto', severity: 'error' });
    }
  };

  const handleToggleActive = async (product: ProductForSale) => {
    try {
      await productsApi.toggleActive(product.id!, !product.active);
      setSnackbar({
        open: true,
        message: product.active ? 'Producto deshabilitado' : 'Producto habilitado',
        severity: 'success',
      });
      void loadProducts();
    } catch {
      setSnackbar({ open: true, message: 'Error al cambiar estado del producto', severity: 'error' });
    }
  };

  const handleImageUploadClick = (productId: string) => {
    setUploadingProductId(productId);
    fileInputRef.current?.click();
  };

  const handleImageUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file || !uploadingProductId) return;
    try {
      await productsApi.uploadImage(uploadingProductId, file);
      setSnackbar({ open: true, message: 'Imagen subida exitosamente', severity: 'success' });
      void loadProducts();
    } catch {
      setSnackbar({ open: true, message: 'Error al subir imagen', severity: 'error' });
    } finally {
      setUploadingProductId(null);
      if (fileInputRef.current) fileInputRef.current.value = '';
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">Productos</h1>
        <button className="btn-primary flex items-center gap-1.5" onClick={() => handleOpenDialog()}>
          <Plus size={18} />
          Agregar Producto
        </button>
      </div>

      <div className="mb-3">
        <input
          className="input-field"
          placeholder="Buscar productos..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
      </div>

      <div className="overflow-auto rounded-md border border-gray-200">
        <table className="w-full text-sm">
          <thead className="bg-gray-50">
            <tr>
              <th className="px-4 py-2 text-left font-medium text-gray-600">Imagen</th>
              <th className="px-4 py-2 text-left font-medium text-gray-600">Descripción</th>
              <th className="px-4 py-2 text-left font-medium text-gray-600">Precio</th>
              <th className="px-4 py-2 text-left font-medium text-gray-600">Stock</th>
              <th className="px-4 py-2 text-right font-medium text-gray-600">Acciones</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {filteredProducts.map((product) => (
              <tr key={product.id} className="hover:bg-gray-50">
                <td className="px-4 py-2">
                  <div className="w-12 h-12 rounded-lg overflow-hidden bg-gray-100 flex items-center justify-center">
                    {product.imageUrl ? (
                      <img
                        src={buildSalesAssetUrl(`/api/uploads/${product.imageUrl}`)}
                        alt={product.name}
                        className="w-full h-full object-cover"
                      />
                    ) : (
                      <Image size={20} className="text-gray-400" />
                    )}
                  </div>
                </td>
                <td className="px-4 py-2">
                  <p className="font-medium">{product.name}</p>
                  <p className="text-xs text-gray-500">{product.description || '-'}</p>
                </td>
                <td className="px-4 py-2">${product.price.toFixed(2)}</td>
                <td className="px-4 py-2">{product.stock || 0}</td>
                <td className="px-4 py-2 text-right">
                  <div className="flex items-center justify-end gap-1">
                    <button
                      title={product.active ? 'Editar' : 'Producto deshabilitado'}
                      className="p-1 rounded hover:bg-gray-100 text-blue-600 disabled:opacity-40"
                      onClick={() => handleOpenDialog(product)}
                      disabled={!product.active}
                      aria-label="Editar"
                    >
                      <Edit size={16} />
                    </button>
                    <button
                      title={product.active ? 'Deshabilitar' : 'Habilitar'}
                      className={`p-1 rounded hover:bg-gray-100 ${product.active ? 'text-amber-500' : 'text-green-600'}`}
                      onClick={() => void handleToggleActive(product)}
                      aria-label={product.active ? 'Deshabilitar' : 'Habilitar'}
                    >
                      {product.active ? <ToggleRight size={16} /> : <ToggleLeft size={16} />}
                    </button>
                    <button
                      title="Subir imagen"
                      className="p-1 rounded hover:bg-gray-100 text-sky-600"
                      onClick={() => handleImageUploadClick(product.id!)}
                      aria-label="Subir imagen"
                    >
                      <Image size={16} />
                    </button>
                    <button
                      title="Historial"
                      className="p-1 rounded hover:bg-gray-100 text-gray-500"
                      aria-label="Historial"
                    >
                      <History size={16} />
                    </button>
                    <button
                      title="Eliminar"
                      className="p-1 rounded hover:bg-gray-100 text-red-600"
                      onClick={() => handleOpenConfirmDialog(product.id!)}
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

      <input
        type="file"
        ref={fileInputRef}
        className="hidden"
        accept="image/*"
        onChange={handleImageUpload}
      />

      {/* Edit / Create Dialog */}
      {dialogOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
          <div className="bg-white rounded-lg shadow-xl w-full max-w-sm">
            <div className="px-5 py-4 border-b border-gray-200">
              <h2 className="text-lg font-semibold">{editingProduct ? 'Editar Producto' : 'Agregar Producto'}</h2>
            </div>
            <div className="px-5 py-4 flex flex-col gap-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Nombre *</label>
                <input className="input-field" value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} required />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Precio *</label>
                <input className="input-field" type="number" value={formData.price} onChange={(e) => setFormData({ ...formData, price: parseFloat(e.target.value) })} required />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Stock</label>
                <input className="input-field" type="number" value={formData.stock || 0} onChange={(e) => setFormData({ ...formData, stock: parseInt(e.target.value) })} />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Descripción</label>
                <textarea className="input-field" rows={3} value={formData.description || ''} onChange={(e) => setFormData({ ...formData, description: e.target.value })} />
              </div>
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
              <p className="text-sm text-gray-700">¿Está seguro de que desea eliminar este producto?</p>
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

