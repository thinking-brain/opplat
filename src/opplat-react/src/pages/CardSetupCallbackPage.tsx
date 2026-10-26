import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { paymentMethodsApi } from '../api/paymentMethods.api';

/**
 * Landing page for Redsys's Ds_Merchant_UrlOK/UrlKO redirect after the 3DS challenge. The
 * webhook (not this redirect) is authoritative for whether the card setup succeeded, so this
 * page just polls the authenticated payment-methods list for a short while; if there is no
 * session (e.g. right after self-registration, before first login) it falls back to a
 * generic "check back after logging in" message.
 */
export const CardSetupCallbackPage = () => {
  const [status, setStatus] = useState<'checking' | 'found' | 'unknown'>('checking');

  useEffect(() => {
    let cancelled = false;
    let attempts = 0;

    const poll = async () => {
      try {
        const methods = await paymentMethodsApi.list();
        if (cancelled) return;
        if (methods.length > 0) {
          setStatus('found');
          return;
        }
      } catch {
        if (cancelled) return;
        setStatus('unknown');
        return;
      }

      attempts += 1;
      if (attempts >= 5) {
        setStatus('unknown');
        return;
      }
      setTimeout(() => { void poll(); }, 2000);
    };

    void poll();
    return () => { cancelled = true; };
  }, []);

  return (
    <div className="max-w-lg mx-auto mt-16 px-4 text-center">
      {status === 'checking' && (
        <>
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mx-auto mb-4" />
          <p className="text-gray-600">Confirmando tu método de pago con Redsys...</p>
        </>
      )}
      {status === 'found' && (
        <>
          <div className="bg-green-50 border border-green-300 text-green-700 px-4 py-3 rounded-md text-sm mb-4">
            Método de pago configurado correctamente.
          </div>
          <Link className="btn-primary" to="/billing/payment-methods">Ir a Métodos de pago</Link>
        </>
      )}
      {status === 'unknown' && (
        <>
          <div className="bg-blue-50 border border-blue-300 text-blue-800 px-4 py-3 rounded-md text-sm mb-4">
            Estamos procesando tu método de pago. Inicia sesión para comprobar el estado de tu suscripción.
          </div>
          <Link className="btn-primary" to="/login">Ir a Iniciar sesión</Link>
        </>
      )}
    </div>
  );
};
