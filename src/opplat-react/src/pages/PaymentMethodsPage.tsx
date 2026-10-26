import { useEffect, useState } from 'react';
import { CreditCard, Star, Trash2 } from 'lucide-react';
import { paymentMethodsApi } from '../api/paymentMethods.api';
import { submitRedsysCardSetupRedirect } from '../utils/redsysRedirect';
import type { TenantPaymentMethodDto } from '../types';

export const PaymentMethodsPage = () => {
  const [methods, setMethods] = useState<TenantPaymentMethodDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [redirecting, setRedirecting] = useState(false);
  const [message, setMessage] = useState<string>();

  const load = () => paymentMethodsApi.list().then(setMethods).catch(() => setMessage('No se pudieron cargar los métodos de pago.')).finally(() => setLoading(false));
  useEffect(() => { void load(); }, []);

  const addCard = async () => {
    setRedirecting(true);
    setMessage(undefined);
    try {
      const redirect = await paymentMethodsApi.initiateCardSetup();
      submitRedsysCardSetupRedirect(redirect);
    } catch {
      setMessage('No se pudo iniciar la configuración del método de pago.');
      setRedirecting(false);
    }
  };

  const remove = async (id: string) => {
    try { await paymentMethodsApi.remove(id); await load(); setMessage('Método de pago eliminado.'); }
    catch { setMessage('No se pudo eliminar el método de pago.'); }
  };

  const setDefault = async (id: string) => {
    try { await paymentMethodsApi.setDefault(id); await load(); setMessage('Método de pago predeterminado actualizado.'); }
    catch { setMessage('No se pudo actualizar el método predeterminado.'); }
  };

  return (
    <section className="max-w-3xl mx-auto space-y-6">
      <div><h1 className="text-2xl font-semibold text-gray-900">Métodos de pago</h1><p className="text-gray-600 mt-1">Gestiona las tarjetas de tu suscripción.</p></div>
      {message && <div className="rounded border border-blue-200 bg-blue-50 px-4 py-3 text-blue-800">{message}</div>}
      <div className="rounded border bg-white shadow-sm">
        <div className="flex items-center gap-2 border-b px-5 py-4"><CreditCard size={20} /><h2 className="font-semibold">Tarjetas guardadas</h2></div>
        {loading ? <p className="p-5 text-gray-600">Cargando...</p> : methods.length === 0 ? <p className="p-5 text-gray-600">No hay tarjetas guardadas.</p> : <div className="divide-y">{methods.map((method) => <div className="flex items-center gap-3 px-5 py-4" key={method.id}><CreditCard className="text-blue-600" /><div className="flex-1"><p className="font-medium">{method.brand ?? 'Tarjeta'} terminada en {method.last4}</p><p className="text-sm text-gray-500">Caduca {String(method.expMonth).padStart(2, '0')}/{method.expYear}</p></div>{method.isDefault ? <span className="flex items-center gap-1 text-sm text-green-700"><Star size={15} />Predeterminada</span> : <button className="btn-ghost text-sm" onClick={() => void setDefault(method.id)}>Usar como predeterminada</button>}<button className="text-red-600" title="Eliminar tarjeta" onClick={() => void remove(method.id)}><Trash2 size={18} /></button></div>)}</div>}
      </div>
      <div className="rounded border bg-white p-5 shadow-sm space-y-4">
        <div><h2 className="font-semibold">Añadir tarjeta</h2><p className="text-sm text-gray-500">Serás redirigido a la pasarela segura de Redsys para introducir los datos de tu tarjeta.</p></div>
        <button className="btn-primary" onClick={() => void addCard()} disabled={redirecting}>{redirecting ? 'Redirigiendo...' : 'Añadir método de pago'}</button>
      </div>
    </section>
  );
};

