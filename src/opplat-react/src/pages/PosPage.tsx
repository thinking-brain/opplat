import React, { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { Link } from 'react-router-dom';
import { Minus, Plus, Printer, Search, Trash2 } from 'lucide-react';
import { posApi, posErrorMessage } from '../api/pos.api';
import { productsApi } from '../api/products.api';
import { LoadingSpinner } from '../components/LoadingSpinner';
import { PaymentDialog } from '../components/pos/PaymentDialog';
import { ShiftDialog } from '../components/pos/ShiftDialog';
import { formatMoney } from '../components/pos/money';
import { ProductForSale } from '../types';
import { PaymentTerminal, PosRegister, PosSale, PosShift } from '../types/pos';

interface CartLine {
  product: ProductForSale;
  quantity: number;
}

const REGISTER_KEY = 'opplat.pos.register';

export const PosPage: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [registers, setRegisters] = useState<PosRegister[]>([]);
  const [registerId, setRegisterId] = useState<string>(() => localStorage.getItem(REGISTER_KEY) ?? '');
  const [shift, setShift] = useState<PosShift | null>(null);
  const [products, setProducts] = useState<ProductForSale[]>([]);
  const [terminals, setTerminals] = useState<PaymentTerminal[]>([]);
  const [cart, setCart] = useState<CartLine[]>([]);
  const [search, setSearch] = useState('');
  const [sale, setSale] = useState<PosSale | null>(null);
  const [receipt, setReceipt] = useState<PosSale | null>(null);
  const [shiftDialog, setShiftDialog] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const searchRef = useRef<HTMLInputElement>(null);

  const total = useMemo(() => cart.reduce((sum, line) => sum + line.quantity * line.product.price, 0), [cart]);

  const loadShift = useCallback(async (id: string) => {
    setShift(id ? await posApi.currentShift(id) : null);
  }, []);

  useEffect(() => {
    void (async () => {
      try {
        const [registerList, productList, terminalList] = await Promise.all([
          posApi.listRegisters(),
          productsApi.list(),
          posApi.listTerminals(),
        ]);
        const active = registerList.filter((r) => r.isActive);
        setRegisters(active);
        setProducts(productList.filter((p) => p.active !== false));
        setTerminals(terminalList.filter((t) => t.isActive));
        const selected = active.find((r) => r.id === registerId)?.id ?? active[0]?.id ?? '';
        setRegisterId(selected);
        await loadShift(selected);
      } catch (e) {
        setError(posErrorMessage(e, 'No se pudo cargar el TPV'));
      } finally {
        setLoading(false);
      }
    })();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const selectRegister = async (id: string) => {
    setRegisterId(id);
    localStorage.setItem(REGISTER_KEY, id);
    setCart([]);
    await loadShift(id);
  };

  const addProduct = (product: ProductForSale) => {
    setCart((current) => {
      const existing = current.find((line) => line.product.id === product.id);
      return existing
        ? current.map((line) => (line === existing ? { ...line, quantity: line.quantity + 1 } : line))
        : [...current, { product, quantity: 1 }];
    });
  };

  const changeQuantity = (productId: string | undefined, delta: number) =>
    setCart((current) =>
      current
        .map((line) => (line.product.id === productId ? { ...line, quantity: line.quantity + delta } : line))
        .filter((line) => line.quantity > 0),
    );

  // A barcode scanner types the code and presses Enter, so an exact match adds the product at once.
  const onSearchKey = (event: React.KeyboardEvent<HTMLInputElement>) => {
    if (event.key !== 'Enter') return;
    const term = search.trim().toLowerCase();
    const exact = products.find((p) => p.barcode?.toLowerCase() === term || p.code?.toLowerCase() === term);
    if (exact) {
      addProduct(exact);
      setSearch('');
    }
  };

  const visibleProducts = useMemo(() => {
    const term = search.trim().toLowerCase();
    return term ? products.filter((p) => p.name.toLowerCase().includes(term) || p.code?.toLowerCase().includes(term)) : products;
  }, [products, search]);

  const charge = async () => {
    if (!shift || cart.length === 0) return;
    setError(null);
    try {
      setSale(
        await posApi.createSale(
          registerId,
          cart.map((line) => ({ productId: line.product.id!, quantity: line.quantity })),
        ),
      );
    } catch (e) {
      setError(posErrorMessage(e, 'No se pudo iniciar la venta'));
    }
  };

  const newSale = () => {
    setReceipt(null);
    setSale(null);
    setCart([]);
    setSearch('');
    searchRef.current?.focus();
  };

  const printTicket = async (invoiceId: string) => {
    const blob = await posApi.ticketPdf(invoiceId);
    const url = URL.createObjectURL(blob);
    const win = window.open(url, '_blank');
    win?.addEventListener('load', () => win.print());
  };

  if (loading) return <LoadingSpinner />;

  if (registers.length === 0) {
    return (
      <div className="card text-center">
        <h1 className="text-xl font-semibold mb-2">TPV</h1>
        <p className="text-gray-600 mb-3">Aún no hay cajas configuradas.</p>
        <Link className="btn-primary inline-block" to="/pos/settings">Configurar cajas y datáfonos</Link>
      </div>
    );
  }

  return (
    <div className="flex flex-col gap-3 h-full">
      <div className="flex flex-wrap items-center gap-3">
        <h1 className="text-2xl font-bold">TPV</h1>
        <select className="input-field w-auto" value={registerId} onChange={(e) => void selectRegister(e.target.value)}>
          {registers.map((r) => (
            <option key={r.id} value={r.id}>{r.name}</option>
          ))}
        </select>
        <span className={`text-sm ${shift ? 'text-green-700' : 'text-amber-600'}`}>
          {shift ? `Turno abierto por ${shift.openedBy}` : 'Turno cerrado'}
        </span>
        <button className="btn-secondary ml-auto" onClick={() => setShiftDialog(true)}>
          {shift ? 'Cerrar turno' : 'Abrir turno'}
        </button>
        <Link className="text-sm text-blue-600" to="/pos/settings">Ajustes</Link>
      </div>

      {error && <p role="alert" className="text-sm text-red-600 bg-red-50 rounded p-2">{error}</p>}

      {receipt ? (
        <div className="card max-w-md mx-auto text-center">
          <p className="text-green-700 font-semibold">Venta completada</p>
          <p className="text-4xl font-bold my-2">{formatMoney(receipt.total)}</p>
          <p className="text-sm text-gray-600">Factura {receipt.invoiceNumber}</p>
          {receipt.payments.find((p) => p.method === 'Cash' && p.changeGiven) && (
            <p className="text-sm mt-1">
              Cambio: <strong>{formatMoney(receipt.payments.reduce((sum, p) => sum + (p.changeGiven ?? 0), 0))}</strong>
            </p>
          )}
          <div className="flex gap-2 justify-center mt-4">
            {receipt.invoiceId && (
              <button className="btn-secondary flex items-center gap-1.5" onClick={() => void printTicket(receipt.invoiceId!)}>
                <Printer size={18} /> Ticket
              </button>
            )}
            <button className="btn-primary" onClick={newSale}>Nueva venta</button>
          </div>
        </div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-3 flex-1 min-h-0">
          <div className="lg:col-span-2 flex flex-col gap-2 min-h-0">
            <div className="relative">
              <Search size={16} className="absolute left-3 top-3 text-gray-400" />
              <input
                ref={searchRef}
                autoFocus
                className="input-field pl-9"
                placeholder="Buscar o escanear código…"
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                onKeyDown={onSearchKey}
              />
            </div>
            <div className="grid grid-cols-2 md:grid-cols-3 xl:grid-cols-4 gap-2 overflow-auto">
              {visibleProducts.map((product) => (
                <button
                  key={product.id}
                  disabled={!shift}
                  onClick={() => addProduct(product)}
                  className="rounded-lg border border-gray-200 bg-white p-3 text-left hover:border-blue-400 active:bg-blue-50 disabled:opacity-50 min-h-[88px]"
                >
                  <p className="font-medium leading-tight">{product.name}</p>
                  <p className="text-sm text-gray-600 mt-1">{formatMoney(product.price)}</p>
                </button>
              ))}
            </div>
          </div>

          <div className="card flex flex-col min-h-0">
            <h2 className="font-semibold mb-2">Ticket</h2>
            <ul className="flex-1 overflow-auto divide-y divide-gray-100">
              {cart.length === 0 && <li className="text-sm text-gray-500 py-4 text-center">Añada productos</li>}
              {cart.map((line) => (
                <li key={line.product.id} className="py-2 flex items-center gap-2">
                  <div className="flex-1">
                    <p className="text-sm font-medium">{line.product.name}</p>
                    <p className="text-xs text-gray-500">{formatMoney(line.product.price)}</p>
                  </div>
                  <button aria-label="Quitar uno" className="p-2 rounded border border-gray-200" onClick={() => changeQuantity(line.product.id, -1)}><Minus size={14} /></button>
                  <span className="w-6 text-center">{line.quantity}</span>
                  <button aria-label="Añadir uno" className="p-2 rounded border border-gray-200" onClick={() => changeQuantity(line.product.id, 1)}><Plus size={14} /></button>
                  <button aria-label="Eliminar línea" className="p-2 text-red-600" onClick={() => changeQuantity(line.product.id, -line.quantity)}><Trash2 size={14} /></button>
                </li>
              ))}
            </ul>
            <div className="border-t border-gray-200 pt-3 mt-2">
              <div className="flex justify-between text-xl font-bold mb-2">
                <span>Total</span>
                <span>{formatMoney(total)}</span>
              </div>
              <button className="btn-primary w-full py-3 text-lg" disabled={!shift || cart.length === 0} onClick={() => void charge()}>
                Cobrar
              </button>
            </div>
          </div>
        </div>
      )}

      {sale && (
        <PaymentDialog
          sale={sale}
          terminals={terminals.filter((t) => !t.registerId || t.registerId === registerId)}
          onSaleChange={setSale}
          onCompleted={(completed) => {
            setSale(null);
            setReceipt(completed);
          }}
          onAbandon={() => setSale(null)}
        />
      )}

      {shiftDialog && (
        <ShiftDialog
          registerId={registerId}
          shift={shift}
          onClose={() => setShiftDialog(false)}
          onChanged={(next) => {
            setShift(next);
            setShiftDialog(false);
          }}
        />
      )}
    </div>
  );
};
