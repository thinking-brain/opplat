import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { posApi, posErrorMessage } from '../api/pos.api';
import { LoadingSpinner } from '../components/LoadingSpinner';
import { PaymentTerminal, PosRegister } from '../types/pos';

export const PosSettingsPage: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [registers, setRegisters] = useState<PosRegister[]>([]);
  const [terminals, setTerminals] = useState<PaymentTerminal[]>([]);
  const [providers, setProviders] = useState<string[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [register, setRegister] = useState({ code: '', name: '', invoiceSeries: '' });
  const [terminal, setTerminal] = useState({ registerId: '', provider: '', providerTerminalId: '', displayName: '' });

  const load = async () => {
    const [r, t, p] = await Promise.all([posApi.listRegisters(), posApi.listTerminals(), posApi.listProviders()]);
    setRegisters(r);
    setTerminals(t);
    setProviders(p);
    setTerminal((current) => ({ ...current, provider: current.provider || p[0] || '' }));
  };

  useEffect(() => {
    load()
      .catch((e) => setError(posErrorMessage(e, 'No se pudo cargar la configuración')))
      .finally(() => setLoading(false));
  }, []);

  const run = async (action: () => Promise<unknown>, failure: string) => {
    setError(null);
    try {
      await action();
      await load();
    } catch (e) {
      setError(posErrorMessage(e, failure));
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="flex flex-col gap-6 max-w-3xl">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Ajustes del TPV</h1>
        <Link to="/pos" className="text-sm text-blue-600">Volver al TPV</Link>
      </div>
      {error && <p role="alert" className="text-sm text-red-600 bg-red-50 rounded p-2">{error}</p>}

      <section className="card">
        <h2 className="font-semibold mb-1">Cajas</h2>
        <p className="text-xs text-gray-500 mb-3">
          Cada caja tiene su propia serie de facturación; la serie forma parte del número fiscal de los tickets.
        </p>
        <ul className="divide-y divide-gray-100 mb-3">
          {registers.map((r) => (
            <li key={r.id} className="py-2 text-sm flex justify-between">
              <span>{r.name} ({r.code})</span>
              <span className="text-gray-500">Serie {r.invoiceSeries}</span>
            </li>
          ))}
        </ul>
        <div className="grid grid-cols-1 md:grid-cols-4 gap-2">
          <input className="input-field" placeholder="Código" value={register.code} onChange={(e) => setRegister({ ...register, code: e.target.value })} />
          <input className="input-field" placeholder="Nombre" value={register.name} onChange={(e) => setRegister({ ...register, name: e.target.value })} />
          <input className="input-field" placeholder="Serie (p. ej. T1)" value={register.invoiceSeries} onChange={(e) => setRegister({ ...register, invoiceSeries: e.target.value })} />
          <button
            className="btn-primary"
            disabled={!register.code || !register.name || !register.invoiceSeries}
            onClick={() => void run(async () => {
              await posApi.createRegister(register);
              setRegister({ code: '', name: '', invoiceSeries: '' });
            }, 'No se pudo crear la caja')}
          >
            Añadir caja
          </button>
        </div>
      </section>

      <section className="card">
        <h2 className="font-semibold mb-3">Datáfonos</h2>
        <ul className="divide-y divide-gray-100 mb-3">
          {terminals.map((t) => (
            <li key={t.id} className="py-2 text-sm flex justify-between items-center">
              <span>{t.displayName} · {t.provider}/{t.providerTerminalId}{t.isActive ? '' : ' (inactivo)'}</span>
              {t.isActive && (
                <button className="text-red-600 text-xs" onClick={() => void run(() => posApi.deactivateTerminal(t.id), 'No se pudo desactivar')}>
                  Desactivar
                </button>
              )}
            </li>
          ))}
        </ul>
        <div className="grid grid-cols-1 md:grid-cols-5 gap-2">
          <select className="input-field" value={terminal.provider} onChange={(e) => setTerminal({ ...terminal, provider: e.target.value })}>
            {providers.map((p) => (
              <option key={p} value={p}>{p}</option>
            ))}
          </select>
          <input className="input-field" placeholder="ID del terminal" value={terminal.providerTerminalId} onChange={(e) => setTerminal({ ...terminal, providerTerminalId: e.target.value })} />
          <input className="input-field" placeholder="Nombre" value={terminal.displayName} onChange={(e) => setTerminal({ ...terminal, displayName: e.target.value })} />
          <select className="input-field" value={terminal.registerId} onChange={(e) => setTerminal({ ...terminal, registerId: e.target.value })}>
            <option value="">Todas las cajas</option>
            {registers.map((r) => (
              <option key={r.id} value={r.id}>{r.name}</option>
            ))}
          </select>
          <button
            className="btn-primary"
            disabled={!terminal.provider || !terminal.providerTerminalId || !terminal.displayName}
            onClick={() => void run(async () => {
              await posApi.createTerminal({ ...terminal, registerId: terminal.registerId || null });
              setTerminal({ ...terminal, providerTerminalId: '', displayName: '' });
            }, 'No se pudo registrar el datáfono')}
          >
            Añadir
          </button>
        </div>
      </section>
    </div>
  );
};
