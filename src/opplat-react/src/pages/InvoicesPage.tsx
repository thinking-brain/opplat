import React, { useState, useEffect } from 'react';
import { FileText, Plus, CheckCircle, XCircle, Clock, Send } from 'lucide-react';
import { invoicesApi } from '../api/invoices.api';
import { Invoice, InvoiceStatus } from '../types';
import { LoadingSpinner } from '../components/LoadingSpinner';

const statusBadge = (status: InvoiceStatus) => {
  const map: Record<InvoiceStatus, { label: string; className: string }> = {
    Draft: { label: 'Borrador', className: 'bg-gray-100 text-gray-700' },
    Issued: { label: 'Emitida', className: 'bg-green-100 text-green-700' },
    Sent: { label: 'Enviada', className: 'bg-blue-100 text-blue-700' },
    Cancelled: { label: 'Cancelada', className: 'bg-red-100 text-red-700' },
  };
  const { label, className } = map[status] ?? { label: status, className: 'bg-gray-100 text-gray-700' };
  return <span className={`text-xs font-medium px-2 py-0.5 rounded-full ${className}`}>{label}</span>;
};

const fiscalBadge = (status?: string) => {
  if (!status || status === 'NotApplicable') return null;
  const map: Record<string, { icon: React.ReactNode; className: string }> = {
    Pending: { icon: <Clock size={12} />, className: 'text-yellow-600' },
    Submitted: { icon: <Send size={12} />, className: 'text-blue-600' },
    Accepted: { icon: <CheckCircle size={12} />, className: 'text-green-600' },
    Rejected: { icon: <XCircle size={12} />, className: 'text-red-600' },
  };
  const entry = map[status];
  if (!entry) return null;
  return <span className={`flex items-center gap-0.5 ${entry.className}`} title={`AEAT: ${status}`}>{entry.icon}</span>;
};

export const InvoicesPage: React.FC = () => {
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [loading, setLoading] = useState(true);
  const [actionId, setActionId] = useState<string | null>(null);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });
  const [selected, setSelected] = useState<Invoice | null>(null);

  useEffect(() => { load(); }, []);
  useEffect(() => {
    if (!snackbar.open) return;
    const t = setTimeout(() => setSnackbar(s => ({ ...s, open: false })), 5000);
    return () => clearTimeout(t);
  }, [snackbar.open]);

  const load = async () => {
    try {
      setLoading(true);
      setInvoices(await invoicesApi.list());
    } catch {
      setSnackbar({ open: true, message: 'Error al cargar facturas', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const handleIssue = async (id: string) => {
    setActionId(id);
    try {
      const result = await invoicesApi.issue(id);
      setSnackbar({ open: true, message: result.message, severity: result.status ? 'success' : 'error' });
      await load();
      if (selected?.id === id) setSelected(await invoicesApi.get(id));
    } catch {
      setSnackbar({ open: true, message: 'Error al emitir factura', severity: 'error' });
    } finally {
      setActionId(null);
    }
  };

  const handleCancel = async (id: string) => {
    if (!window.confirm('¿Cancelar esta factura? No se puede deshacer.')) return;
    setActionId(id);
    try {
      const result = await invoicesApi.cancel(id);
      setSnackbar({ open: true, message: result.message, severity: result.status ? 'success' : 'error' });
      await load();
      if (selected?.id === id) setSelected(null);
    } catch {
      setSnackbar({ open: true, message: 'Error al cancelar factura', severity: 'error' });
    } finally {
      setActionId(null);
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-2xl font-bold">Facturas</h1>
        <a href="/invoicing/settings" className="btn-secondary text-sm flex items-center gap-1">
          <Plus size={15} /> Ajustes fiscales
        </a>
      </div>

      {snackbar.open && (
        <div className={`mb-4 p-3 rounded text-sm ${snackbar.severity === 'success' ? 'bg-green-50 text-green-800 border border-green-200' : 'bg-red-50 text-red-800 border border-red-200'}`}>
          {snackbar.message}
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
        {/* List */}
        <div className="lg:col-span-2">
          <div className="card overflow-hidden">
            {invoices.length === 0 ? (
              <div className="p-8 text-center text-gray-400 flex flex-col items-center gap-2">
                <FileText size={36} className="opacity-40" />
                <p className="text-sm">No hay facturas. Emite una desde el Punto de Venta.</p>
              </div>
            ) : (
              <table className="min-w-full text-sm divide-y divide-gray-100">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="text-left px-4 py-2 text-xs text-gray-500 font-medium">Número</th>
                    <th className="text-left px-4 py-2 text-xs text-gray-500 font-medium">Fecha</th>
                    <th className="text-left px-4 py-2 text-xs text-gray-500 font-medium">Cliente</th>
                    <th className="text-right px-4 py-2 text-xs text-gray-500 font-medium">Total</th>
                    <th className="text-center px-4 py-2 text-xs text-gray-500 font-medium">Estado</th>
                    <th className="text-center px-4 py-2 text-xs text-gray-500 font-medium">AEAT</th>
                    <th className="px-4 py-2"></th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-50">
                  {invoices.map((inv) => (
                    <tr
                      key={inv.id}
                      className={`hover:bg-gray-50 cursor-pointer transition-colors ${selected?.id === inv.id ? 'bg-blue-50' : ''}`}
                      onClick={() => setSelected(inv)}
                    >
                      <td className="px-4 py-2.5 font-mono text-xs">{inv.fullNumber || `${inv.series}-???`}</td>
                      <td className="px-4 py-2.5 text-gray-600">{new Date(inv.issueDate).toLocaleDateString('es-ES')}</td>
                      <td className="px-4 py-2.5 text-gray-600 truncate max-w-[140px]">
                        {inv.customerSnapshot.name || <span className="text-gray-400 italic">Consumidor final</span>}
                      </td>
                      <td className="px-4 py-2.5 text-right font-medium">{inv.totalAmount.toFixed(2)} {inv.currency}</td>
                      <td className="px-4 py-2.5 text-center">{statusBadge(inv.status)}</td>
                      <td className="px-4 py-2.5 text-center">{fiscalBadge(inv.fiscalRecord?.submissionStatus)}</td>
                      <td className="px-4 py-2.5 text-right">
                        <div className="flex items-center justify-end gap-1">
                          {inv.status === 'Draft' && (
                            <button
                              className="btn-primary text-xs py-1 px-2"
                              disabled={actionId === inv.id}
                              onClick={(e) => { e.stopPropagation(); handleIssue(inv.id!); }}
                            >
                              Emitir
                            </button>
                          )}
                          {inv.status === 'Issued' && (
                            <button
                              className="text-xs py-1 px-2 text-red-600 hover:bg-red-50 rounded"
                              disabled={actionId === inv.id}
                              onClick={(e) => { e.stopPropagation(); handleCancel(inv.id!); }}
                            >
                              Cancelar
                            </button>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        </div>

        {/* Detail panel */}
        <div>
          {selected ? (
            <div className="card p-4 space-y-4">
              <div className="flex items-center justify-between">
                <h2 className="font-semibold">{selected.fullNumber || 'Borrador'}</h2>
                {statusBadge(selected.status)}
              </div>
              <div className="text-sm space-y-1 text-gray-700">
                <p><span className="text-gray-400">Fecha:</span> {new Date(selected.issueDate).toLocaleDateString('es-ES')}</p>
                <p><span className="text-gray-400">Tipo:</span> {selected.invoiceType}</p>
                <p><span className="text-gray-400">Moneda:</span> {selected.currency}</p>
                {selected.customerSnapshot.name && (
                  <p><span className="text-gray-400">Cliente:</span> {selected.customerSnapshot.name}</p>
                )}
                {selected.customerSnapshot.taxId && (
                  <p><span className="text-gray-400">NIF:</span> {selected.customerSnapshot.taxId}</p>
                )}
                {selected.notes && <p><span className="text-gray-400">Notas:</span> {selected.notes}</p>}
              </div>

              {selected.lines.length > 0 && (
                <div>
                  <p className="text-xs font-medium text-gray-500 mb-1">Líneas</p>
                  <table className="min-w-full text-xs">
                    <tbody className="divide-y divide-gray-100">
                      {selected.lines.map((l, i) => (
                        <tr key={i}>
                          <td className="py-1 pr-2 flex-1">{l.description}</td>
                          <td className="py-1 text-right text-gray-500">{l.quantity}×{l.unitPrice.toFixed(2)}</td>
                          <td className="py-1 text-right font-medium pl-2">{l.lineTotal.toFixed(2)}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}

              <div className="border-t pt-3 text-sm">
                <div className="flex justify-between">
                  <span className="text-gray-500">Subtotal</span>
                  <span>{selected.subtotal.toFixed(2)} {selected.currency}</span>
                </div>
                <div className="flex justify-between font-bold mt-1">
                  <span>Total</span>
                  <span>{selected.totalAmount.toFixed(2)} {selected.currency}</span>
                </div>
              </div>

              {selected.fiscalRecord && (
                <div className="text-xs bg-gray-50 rounded p-2 space-y-0.5">
                  <p className="font-medium text-gray-600 mb-1">Registro fiscal</p>
                  <p><span className="text-gray-400">Estado AEAT:</span> {selected.fiscalRecord.submissionStatus}</p>
                  {selected.fiscalRecord.aeatCsv && (
                    <p><span className="text-gray-400">CSV:</span> <span className="font-mono">{selected.fiscalRecord.aeatCsv}</span></p>
                  )}
                  {selected.fiscalRecord.lastErrorMessage && (
                    <p className="text-red-500">{selected.fiscalRecord.lastErrorMessage}</p>
                  )}
                  {selected.fiscalRecord.qrCodePayload && (
                    <p className="truncate"><span className="text-gray-400">QR:</span> {selected.fiscalRecord.qrCodePayload}</p>
                  )}
                </div>
              )}
            </div>
          ) : (
            <div className="card p-8 text-center text-gray-400 text-sm">
              Selecciona una factura para ver el detalle
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
