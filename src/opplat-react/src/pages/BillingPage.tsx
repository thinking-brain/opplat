import { useEffect, useState } from 'react';
import { CreditCard, ExternalLink, Receipt, XCircle } from 'lucide-react';
import { billingApi } from '../api/billing.api';
import type { SubscriptionPaymentHistoryItem } from '../types';

export const BillingPage = () => {
  const [history, setHistory] = useState<SubscriptionPaymentHistoryItem[]>([]);
  const [message, setMessage] = useState<string>();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    billingApi.history()
      .then(setHistory)
      .catch(() => setMessage('No se pudo cargar el historial de pagos.'))
      .finally(() => setLoading(false));
  }, []);

  const openPortal = async () => {
    try {
      const result = await billingApi.createPortalSession();
      if (result.url) window.location.assign(result.url);
      else setMessage('El portal de facturación no está configurado.');
    } catch {
      setMessage('No se pudo abrir el portal de facturación.');
    }
  };

  const cancelSubscription = async () => {
    if (!window.confirm('¿Cancelar la suscripción al final del periodo actual?')) return;
    try {
      await billingApi.cancel();
      setMessage('La suscripción se cancelará al final del periodo actual.');
    } catch {
      setMessage('No se pudo cancelar la suscripción.');
    }
  };

  return (
    <section className="max-w-5xl mx-auto space-y-6">
      <div>
        <h1 className="text-2xl font-semibold text-gray-900">Facturación</h1>
        <p className="text-gray-600 mt-1">Gestiona tu método de pago y consulta tus pagos.</p>
      </div>

      {message && <div className="rounded border border-blue-200 bg-blue-50 px-4 py-3 text-blue-800">{message}</div>}

      <div className="grid gap-4 sm:grid-cols-2">
        <button onClick={() => void openPortal()} className="flex items-center gap-3 rounded border bg-white p-5 text-left shadow-sm hover:border-blue-500">
          <CreditCard className="text-blue-600" />
          <span><strong className="block">Gestionar pagos</strong><span className="text-sm text-gray-600">Cambiar tarjeta y consultar facturas en el portal seguro.</span></span>
          <ExternalLink className="ml-auto" size={18} />
        </button>
        <button onClick={() => void cancelSubscription()} className="flex items-center gap-3 rounded border bg-white p-5 text-left shadow-sm hover:border-red-500">
          <XCircle className="text-red-600" />
          <span><strong className="block">Cancelar suscripción</strong><span className="text-sm text-gray-600">El acceso continúa hasta el final del periodo pagado.</span></span>
        </button>
      </div>

      <div className="rounded border bg-white shadow-sm">
        <div className="flex items-center gap-2 border-b px-5 py-4"><Receipt size={20} /><h2 className="font-semibold">Historial de pagos</h2></div>
        {loading ? <p className="p-5 text-gray-600">Cargando...</p> : history.length === 0 ? <p className="p-5 text-gray-600">Todavía no hay pagos registrados.</p> : (
          <div className="overflow-x-auto"><table className="min-w-full text-sm"><thead><tr className="border-b text-left text-gray-600"><th className="px-5 py-3">Periodo</th><th className="px-5 py-3">Importe</th><th className="px-5 py-3">Estado</th><th className="px-5 py-3" /></tr></thead><tbody>
            {history.map((payment) => <tr key={payment.invoiceId} className="border-b last:border-0"><td className="px-5 py-3">{new Date(payment.periodStart).toLocaleDateString()} - {new Date(payment.periodEnd).toLocaleDateString()}</td><td className="px-5 py-3">{payment.amount.toFixed(2)} {payment.currency.toUpperCase()}</td><td className="px-5 py-3">{payment.status}</td><td className="px-5 py-3 text-right">{payment.hostedInvoiceUrl && <a href={payment.hostedInvoiceUrl} target="_blank" rel="noreferrer" className="text-blue-600 hover:underline">Ver factura</a>}</td></tr>)}
          </tbody></table></div>
        )}
      </div>
    </section>
  );
};