import { useEffect, useState } from 'react';
import { CreditCard, Receipt, XCircle } from 'lucide-react';
import { Link } from 'react-router-dom';
import { billingApi } from '../api/billing.api';
import type { SubscriptionPaymentHistoryItem, TenantSubscriptionDetails } from '../types';

export const BillingPage = () => {
  const [subscription, setSubscription] = useState<TenantSubscriptionDetails | null>(null);
  const [history, setHistory] = useState<SubscriptionPaymentHistoryItem[]>([]);
  const [message, setMessage] = useState<string>();
  const [loading, setLoading] = useState(true);
  const [downloadingInvoiceId, setDownloadingInvoiceId] = useState<string>();

  const loadData = async () => {
    try {
      const [subscriptionResponse, historyResponse] = await Promise.all([
        billingApi.getSubscription(),
        billingApi.history(),
      ]);
      setSubscription(subscriptionResponse);
      setHistory(historyResponse);
    } catch {
      setMessage('No se pudo cargar la información de facturación.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void loadData();
  }, []);

  const cancelSubscription = async () => {
    if (!window.confirm('¿Cancelar la suscripción al final del periodo actual?')) return;
    try {
      await billingApi.cancel();
      setMessage('La suscripción se cancelará al final del periodo actual.');
    } catch {
      setMessage('No se pudo cancelar la suscripción.');
    }
  };

  const downloadInvoice = async (invoiceId: string) => {
    setDownloadingInvoiceId(invoiceId);
    try {
      const blob = await billingApi.downloadInvoice(invoiceId);
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = `SubscriptionInvoice_${invoiceId}.pdf`;
      link.click();
      URL.revokeObjectURL(url);
    } catch {
      setMessage('No se pudo descargar la factura.');
    } finally {
      setDownloadingInvoiceId(undefined);
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
        <Link to="/billing/payment-methods" className="flex items-center gap-3 rounded border bg-white p-5 text-left shadow-sm hover:border-blue-500">
          <CreditCard className="text-blue-600" />
          <span><strong className="block">Métodos de pago</strong><span className="text-sm text-gray-600">Añade, cambia o elimina tus tarjetas guardadas.</span></span>
        </Link>
        <button onClick={() => void cancelSubscription()} className="flex items-center gap-3 rounded border bg-white p-5 text-left shadow-sm hover:border-red-500">
          <XCircle className="text-red-600" />
          <span><strong className="block">Cancelar suscripción</strong><span className="text-sm text-gray-600">El acceso continúa hasta el final del periodo pagado.</span></span>
        </button>
      </div>

      <div className="rounded border bg-white shadow-sm p-5">
        <div className="flex items-center justify-between gap-3">
          <div>
            <h2 className="font-semibold">Suscripción activa</h2>
            <p className="text-sm text-gray-600">{subscription ? `${subscription.subscriptionPlanName} · ${subscription.billingInterval}` : 'Cargando...'}</p>
          </div>
          <Link to="/billing/subscription" className="rounded bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700">
            Cambiar plan
          </Link>
        </div>
        {subscription && (
          <div className="mt-4 rounded border bg-gray-50 p-4 text-sm text-gray-700">
            <div className="flex flex-wrap items-center justify-between gap-3">
              <div>
                <p><span className="font-medium">Estado:</span> {subscription.billingStatus}</p>
                <p><span className="font-medium">Próximo cobro:</span> {subscription.nextBillingDate ? new Date(subscription.nextBillingDate).toLocaleDateString() : 'Sin fecha'}</p>
              </div>
              <div>
                <p><span className="font-medium">Precio mensual:</span> {subscription.pricingMonthly.toFixed(2)} {subscription.currency}</p>
                <p><span className="font-medium">Precio anual:</span> {subscription.pricingAnnual.toFixed(2)} {subscription.currency}</p>
              </div>
            </div>
          </div>
        )}
      </div>

      <div className="rounded border bg-white shadow-sm">
        <div className="flex items-center gap-2 border-b px-5 py-4"><Receipt size={20} /><h2 className="font-semibold">Historial de pagos</h2></div>
        {loading ? <p className="p-5 text-gray-600">Cargando...</p> : history.length === 0 ? <p className="p-5 text-gray-600">Todavía no hay pagos registrados.</p> : (
          <div className="overflow-x-auto"><table className="min-w-full text-sm"><thead><tr className="border-b text-left text-gray-600"><th className="px-5 py-3">Periodo</th><th className="px-5 py-3">Importe</th><th className="px-5 py-3">Estado</th><th className="px-5 py-3" /></tr></thead><tbody>
            {history.map((payment) => <tr key={payment.invoiceId} className="border-b last:border-0"><td className="px-5 py-3">{new Date(payment.periodStart).toLocaleDateString()} - {new Date(payment.periodEnd).toLocaleDateString()}</td><td className="px-5 py-3">{payment.amount.toFixed(2)} {payment.currency.toUpperCase()}</td><td className="px-5 py-3">{payment.status}</td><td className="px-5 py-3 text-right"><button onClick={() => void downloadInvoice(payment.invoiceId)} disabled={downloadingInvoiceId === payment.invoiceId} className="text-blue-600 hover:underline disabled:text-gray-400">{downloadingInvoiceId === payment.invoiceId ? 'Descargando...' : 'Descargar PDF'}</button></td></tr>)}
          </tbody></table></div>
        )}
      </div>
    </section>
  );
};