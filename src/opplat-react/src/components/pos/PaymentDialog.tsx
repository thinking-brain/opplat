import React, { useCallback, useEffect, useRef, useState } from 'react';
import { CreditCard, Banknote, X } from 'lucide-react';
import { posApi, posErrorMessage } from '../../api/pos.api';
import { PaymentTerminal, PosCustomer, PosPayment, PosSale } from '../../types/pos';
import { formatMoney } from './money';

interface Props {
  sale: PosSale;
  terminals: PaymentTerminal[];
  onSaleChange: (sale: PosSale) => void;
  onCompleted: (sale: PosSale) => void;
  onAbandon: () => void;
}

const POLL_MS = 1500;

export const PaymentDialog: React.FC<Props> = ({ sale, terminals, onSaleChange, onCompleted, onAbandon }) => {
  const [tendered, setTendered] = useState('');
  const [terminalId, setTerminalId] = useState(terminals[0]?.id ?? '');
  const [pending, setPending] = useState<PosPayment | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);
  const [needsCustomer, setNeedsCustomer] = useState(false);
  const [customer, setCustomer] = useState<PosCustomer>({ name: '', taxId: '', address: '', country: 'ES' });
  // One key per card attempt so a double click or retry can never charge twice.
  const attemptKey = useRef<string>(crypto.randomUUID());
  const completing = useRef(false);

  const refreshSale = useCallback(async () => {
    const fresh = await posApi.getSale(sale.id);
    onSaleChange(fresh);
    return fresh;
  }, [sale.id, onSaleChange]);

  const finish = useCallback(
    async (customerData?: PosCustomer) => {
      if (completing.current) return;
      completing.current = true;
      try {
        onCompleted(await posApi.completeSale(sale.id, customerData));
      } catch (e) {
        completing.current = false;
        const message = posErrorMessage(e, 'No se pudo finalizar la venta');
        // Totals over the simplified-invoice limit need the customer's data.
        if (message.includes('tax data') || message.includes('full invoice')) setNeedsCustomer(true);
        setError(message);
      }
    },
    [sale.id, onCompleted],
  );

  // Auto-finish as soon as the sale is fully paid (unless customer data is still needed).
  useEffect(() => {
    if (sale.due <= 0 && sale.status === 'Draft' && !pending && !needsCustomer) void finish();
  }, [sale.due, sale.status, pending, needsCustomer, finish]);

  // Poll the terminal while a card payment is in progress.
  useEffect(() => {
    if (!pending) return;
    const timer = setInterval(async () => {
      try {
        const payment = await posApi.getPayment(pending.id);
        if (payment.status !== 'Pending') {
          setPending(null);
          attemptKey.current = crypto.randomUUID();
          if (payment.status !== 'Captured') {
            setError(payment.attempt?.failureReason ?? 'Pago no completado');
          }
          await refreshSale();
        } else {
          setPending(payment);
        }
      } catch {
        setError('No se pudo consultar el datáfono');
      }
    }, POLL_MS);
    return () => clearInterval(timer);
  }, [pending, refreshSale]);

  const payCash = async (amount: number) => {
    setBusy(true);
    setError(null);
    try {
      await posApi.addPayment(sale.id, { method: 'Cash', amount });
      setTendered('');
      await refreshSale();
    } catch (e) {
      setError(posErrorMessage(e, 'No se pudo registrar el pago'));
    } finally {
      setBusy(false);
    }
  };

  const payCard = async () => {
    if (!terminalId) {
      setError('Seleccione un datáfono');
      return;
    }
    setBusy(true);
    setError(null);
    try {
      const payment = await posApi.addPayment(sale.id, { method: 'Card', terminalId }, attemptKey.current);
      if (payment.status === 'Pending') {
        setPending(payment);
      } else {
        attemptKey.current = crypto.randomUUID();
        await refreshSale();
      }
    } catch (e) {
      setError(posErrorMessage(e, 'No se pudo iniciar el cobro con tarjeta'));
    } finally {
      setBusy(false);
    }
  };

  const cancelCard = async () => {
    if (!pending) return;
    try {
      await posApi.cancelPayment(pending.id);
      setPending(null);
      attemptKey.current = crypto.randomUUID();
      await refreshSale();
    } catch (e) {
      setError(posErrorMessage(e, 'No se pudo cancelar el cobro'));
    }
  };

  const abandon = async () => {
    if (pending) return;
    if (sale.paid > 0) {
      setError('Hay pagos registrados; complete la venta o reembolse antes de cancelar.');
      return;
    }
    try {
      await posApi.voidSale(sale.id);
    } finally {
      onAbandon();
    }
  };

  const tenderedValue = parseFloat(tendered.replace(',', '.'));
  const quickAmounts = [5, 10, 20, 50].filter((amount) => amount >= sale.due);

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-md">
        <div className="px-5 py-4 border-b border-gray-200 flex items-center justify-between">
          <h2 className="text-lg font-semibold">Cobrar</h2>
          <button aria-label="Cancelar venta" className="p-1 rounded hover:bg-gray-100" onClick={() => void abandon()}>
            <X size={18} />
          </button>
        </div>

        <div className="px-5 py-4 flex flex-col gap-4">
          <div className="text-center">
            <p className="text-sm text-gray-500">Pendiente</p>
            <p className="text-4xl font-bold">{formatMoney(sale.due)}</p>
            <p className="text-xs text-gray-500">Total {formatMoney(sale.total)} · Pagado {formatMoney(sale.paid)}</p>
          </div>

          {error && <p role="alert" className="text-sm text-red-600 bg-red-50 rounded p-2">{error}</p>}

          {pending ? (
            <div className="text-center py-4">
              <p className="font-medium">Esperando al datáfono…</p>
              <p className="text-sm text-gray-500 mb-3">El cliente debe acercar o insertar la tarjeta.</p>
              <button className="btn-secondary" onClick={() => void cancelCard()}>Cancelar cobro</button>
            </div>
          ) : sale.due > 0 ? (
            <>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Efectivo entregado</label>
                <div className="flex gap-2">
                  <input
                    className="input-field"
                    inputMode="decimal"
                    placeholder={sale.due.toFixed(2)}
                    value={tendered}
                    onChange={(e) => setTendered(e.target.value)}
                  />
                  <button
                    className="btn-primary flex items-center gap-1.5"
                    disabled={busy}
                    onClick={() => void payCash(Number.isFinite(tenderedValue) ? tenderedValue : sale.due)}
                  >
                    <Banknote size={18} /> Efectivo
                  </button>
                </div>
                <div className="flex gap-2 mt-2">
                  {quickAmounts.map((amount) => (
                    <button key={amount} className="btn-secondary" disabled={busy} onClick={() => void payCash(amount)}>
                      {amount} €
                    </button>
                  ))}
                </div>
                {Number.isFinite(tenderedValue) && tenderedValue > sale.due && (
                  <p className="text-sm mt-1">Cambio: <strong>{formatMoney(tenderedValue - sale.due)}</strong></p>
                )}
              </div>

              <div className="border-t border-gray-200 pt-3">
                <label className="block text-sm font-medium text-gray-700 mb-1">Tarjeta</label>
                <div className="flex gap-2">
                  <select className="input-field" value={terminalId} onChange={(e) => setTerminalId(e.target.value)}>
                    {terminals.length === 0 && <option value="">Sin datáfonos</option>}
                    {terminals.map((terminal) => (
                      <option key={terminal.id} value={terminal.id}>{terminal.displayName}</option>
                    ))}
                  </select>
                  <button
                    className="btn-primary flex items-center gap-1.5"
                    disabled={busy || terminals.length === 0}
                    onClick={() => void payCard()}
                  >
                    <CreditCard size={18} /> Tarjeta
                  </button>
                </div>
              </div>
            </>
          ) : null}

          {needsCustomer && (
            <div className="border-t border-gray-200 pt-3 flex flex-col gap-2">
              <p className="text-sm font-medium">Factura completa: datos del cliente</p>
              <input className="input-field" placeholder="Razón social / nombre *" value={customer.name} onChange={(e) => setCustomer({ ...customer, name: e.target.value })} />
              <input className="input-field" placeholder="NIF / CIF *" value={customer.taxId} onChange={(e) => setCustomer({ ...customer, taxId: e.target.value })} />
              <input className="input-field" placeholder="Dirección" value={customer.address} onChange={(e) => setCustomer({ ...customer, address: e.target.value })} />
              <button
                className="btn-primary"
                disabled={!customer.name.trim() || !customer.taxId?.trim()}
                onClick={() => void finish(customer)}
              >
                Emitir factura y finalizar
              </button>
            </div>
          )}

          {sale.payments.some((p) => p.status === 'Captured') && (
            <ul className="text-sm text-gray-600 border-t border-gray-200 pt-2">
              {sale.payments
                .filter((p) => p.status === 'Captured')
                .map((p) => (
                  <li key={p.id} className="flex justify-between">
                    <span>{p.method === 'Cash' ? 'Efectivo' : p.method === 'Card' ? `Tarjeta ${p.attempt?.cardLast4 ? `•••• ${p.attempt.cardLast4}` : ''}` : 'Otro'}</span>
                    <span>{formatMoney(p.amount)}</span>
                  </li>
                ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
};
