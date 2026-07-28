import React, { useState, useEffect } from 'react';
import { Plus, Minus, Trash2, ShoppingCart, FileText } from 'lucide-react';
import { productsApi } from '../api/products.api';
import { salesApi } from '../api/sales.api';
import { invoicesApi } from '../api/invoices.api';
import { ProductForSale, SaleItem } from '../types';
import { LoadingSpinner } from '../components/LoadingSpinner';

export const SellPage: React.FC = () => {
  const [products, setProducts] = useState<ProductForSale[]>([]);
  const [cart, setCart] = useState<SaleItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [generatingInvoice, setGeneratingInvoice] = useState(false);
  const [lastSaleId, setLastSaleId] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [saleDetails, setSaleDetails] = useState({
    dependiente: '',
    posicion: '',
    comanda: '',
    observaciones: '',
  });
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });

  const dependienteOptions = ['Juan Pérez', 'María García', 'Carlos López', 'Ana Martínez'];
  const posicionOptions = ['Mesa 1', 'Mesa 2', 'Mesa 3', 'Mesa 4', 'Mesa 5', 'Barra', 'Para Llevar'];

  useEffect(() => {
    loadProducts();
  }, []);

  const loadProducts = async () => {
    try {
      setLoading(true);
      const data = await productsApi.list();
      setProducts(data);
    } catch (error) {
      setSnackbar({ open: true, message: 'Error loading products', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const addToCart = (product: ProductForSale) => {
    const existingItem = cart.find((item) => item.productId === product.id);

    if (existingItem) {
      setCart(
        cart.map((item) =>
          item.productId === product.id
            ? {
                ...item,
                quantity: item.quantity + 1,
                subtotal: (item.quantity + 1) * item.price,
              }
            : item
        )
      );
    } else {
      setCart([
        ...cart,
        {
          productId: product.id!,
          productName: product.name,
          quantity: 1,
          price: product.price,
          subtotal: product.price,
        },
      ]);
    }
  };

  const updateQuantity = (productId: string, change: number) => {
    setCart(
      cart
        .map((item) => {
          if (item.productId === productId) {
            const newQuantity = item.quantity + change;
            if (newQuantity <= 0) return null;
            return {
              ...item,
              quantity: newQuantity,
              subtotal: newQuantity * item.price,
            };
          }
          return item;
        })
        .filter(Boolean) as SaleItem[]
    );
  };

  const removeFromCart = (productId: string) => {
    setCart(cart.filter((item) => item.productId !== productId));
  };

  const calculateTotal = () => {
    return cart.reduce((sum, item) => sum + item.subtotal, 0);
  };

  const handleCheckout = async () => {
    if (cart.length === 0) {
      setSnackbar({ open: true, message: 'El carrito está vacío', severity: 'error' });
      return;
    }

    try {
      setSubmitting(true);
      const created = await salesApi.create({
        date: new Date().toISOString(),
        total: calculateTotal(),
        items: cart,
      });
      setLastSaleId(created?.id ?? null);
      setSnackbar({ open: true, message: '¡Venta registrada exitosamente!', severity: 'success' });
      setCart([]);
      setSaleDetails({ dependiente: '', posicion: '', comanda: '', observaciones: '' });
    } catch (error) {
      setSnackbar({ open: true, message: 'Error al procesar la venta', severity: 'error' });
    } finally {
      setSubmitting(false);
    }
  };

  const handleGenerateInvoice = async () => {
    if (!lastSaleId) return;
    setGeneratingInvoice(true);
    try {
      const result = await invoicesApi.create({
        series: 'A',
        number: 0,
        fullNumber: '',
        issueDate: new Date().toISOString(),
        invoiceType: 'Simplified',
        status: 'Draft',
        saleId: lastSaleId,
        currency: 'EUR',
        subtotal: 0,
        totalAmount: 0,
        customerSnapshot: { name: '', isFinalConsumer: true },
        lines: [],
      });
      if (result.status) {
        setSnackbar({ open: true, message: 'Factura creada. Complétala en la sección Facturas.', severity: 'success' });
        setLastSaleId(null);
      } else {
        setSnackbar({ open: true, message: result.message, severity: 'error' });
      }
    } catch {
      setSnackbar({ open: true, message: 'Error al crear la factura', severity: 'error' });
    } finally {
      setGeneratingInvoice(false);
    }
  };

  const filteredProducts = products.filter((product) =>
    product.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  useEffect(() => {
    if (!snackbar.open) return;
    const t = setTimeout(() => setSnackbar((s) => ({ ...s, open: false })), 6000);
    return () => clearTimeout(t);
  }, [snackbar.open]);

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <h1 className="text-2xl font-bold mb-6">Punto de Venta</h1>

      {/* Sale Details */}
      <div className="card p-4 mb-4">
        <h2 className="text-base font-semibold mb-3">Detalles de Venta</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Dependiente</label>
            <select
              className="input-field"
              value={saleDetails.dependiente}
              onChange={(e) => setSaleDetails({ ...saleDetails, dependiente: e.target.value })}
            >
              <option value="">Seleccionar dependiente...</option>
              {dependienteOptions.map((opt) => <option key={opt} value={opt}>{opt}</option>)}
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Posición</label>
            <select
              className="input-field"
              value={saleDetails.posicion}
              onChange={(e) => setSaleDetails({ ...saleDetails, posicion: e.target.value })}
            >
              <option value="">Seleccionar posición...</option>
              {posicionOptions.map((opt) => <option key={opt} value={opt}>{opt}</option>)}
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Comanda</label>
            <input
              className="input-field"
              value={saleDetails.comanda}
              onChange={(e) => setSaleDetails({ ...saleDetails, comanda: e.target.value })}
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Observaciones</label>
            <textarea
              className="input-field resize-none"
              rows={2}
              value={saleDetails.observaciones}
              onChange={(e) => setSaleDetails({ ...saleDetails, observaciones: e.target.value })}
            />
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-7 gap-4">
        {/* Product Grid */}
        <div className="md:col-span-4">
          <div className="card p-4">
            <input
              className="input-field mb-4"
              placeholder="Buscar productos..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
            <div className="grid grid-cols-2 sm:grid-cols-3 gap-2">
              {filteredProducts.map((product) => (
                <button
                  key={product.id}
                  className="card p-3 text-left hover:shadow-md transition-shadow cursor-pointer"
                  onClick={() => addToCart(product)}
                >
                  <p className="font-semibold text-sm truncate">{product.name}</p>
                  <p className="text-blue-600 text-sm font-medium">${product.price.toFixed(2)}</p>
                  <p className="text-xs text-gray-400">Stock: {product.stock || 0}</p>
                </button>
              ))}
            </div>
          </div>
        </div>

        {/* Cart */}
        <div className="md:col-span-3">
          <div className="card p-4">
            <div className="flex items-center gap-2 mb-3">
              <ShoppingCart size={18} className="text-gray-600" />
              <h2 className="font-semibold">Carrito</h2>
            </div>

            {cart.length === 0 ? (
              <p className="text-sm text-gray-400">El carrito está vacío</p>
            ) : (
              <>
                <div className="overflow-x-auto">
                  <table className="min-w-full text-sm">
                    <thead>
                      <tr className="border-b">
                        <th className="text-left py-1 font-medium text-gray-500">Producto</th>
                        <th className="text-center py-1 font-medium text-gray-500">Cant.</th>
                        <th className="text-right py-1 font-medium text-gray-500">Total</th>
                        <th></th>
                      </tr>
                    </thead>
                    <tbody>
                      {cart.map((item) => (
                        <tr key={item.productId} className="border-b last:border-0">
                          <td className="py-1.5 pr-2 truncate max-w-[100px]">{item.productName}</td>
                          <td className="py-1.5">
                            <div className="flex items-center justify-center gap-1">
                              <button
                                className="p-0.5 rounded hover:bg-gray-100"
                                onClick={() => updateQuantity(item.productId, -1)}
                              >
                                <Minus size={12} />
                              </button>
                              <span className="w-5 text-center">{item.quantity}</span>
                              <button
                                className="p-0.5 rounded hover:bg-gray-100"
                                onClick={() => updateQuantity(item.productId, 1)}
                              >
                                <Plus size={12} />
                              </button>
                            </div>
                          </td>
                          <td className="py-1.5 text-right">${item.subtotal.toFixed(2)}</td>
                          <td className="py-1.5 pl-1">
                            <button
                              className="p-0.5 text-red-500 hover:bg-red-50 rounded"
                              onClick={() => removeFromCart(item.productId)}
                            >
                              <Trash2 size={13} />
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
                <div className="mt-4 pt-3 border-t">
                  <p className="text-right font-bold text-lg mb-3">Total: ${calculateTotal().toFixed(2)}</p>
                  <button
                    className="btn-primary w-full py-3 text-base"
                    onClick={handleCheckout}
                    disabled={submitting}
                  >
                    {submitting ? 'Procesando...' : 'Registrar Venta'}
                  </button>
                  {lastSaleId && (
                    <button
                      className="mt-2 w-full py-2 text-sm border border-blue-500 text-blue-600 rounded hover:bg-blue-50 flex items-center justify-center gap-1.5"
                      onClick={handleGenerateInvoice}
                      disabled={generatingInvoice}
                    >
                      <FileText size={14} />
                      {generatingInvoice ? 'Generando...' : 'Generar Factura'}
                    </button>
                  )}
                </div>
              </>
            )}
          </div>
        </div>
      </div>

      {snackbar.open && (
        <div className={`fixed bottom-4 left-1/2 -translate-x-1/2 z-50 px-4 py-3 rounded-lg shadow-lg text-white text-sm flex items-center gap-2 ${snackbar.severity === 'success' ? 'bg-green-600' : 'bg-red-600'}`}>
          {snackbar.message}
          <button onClick={() => setSnackbar((s) => ({ ...s, open: false }))} className="ml-1 hover:opacity-75">✕</button>
        </div>
      )}
    </div>
  );
};
