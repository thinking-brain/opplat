import React, { useState } from 'react';
import { posApi, posErrorMessage } from '../../api/pos.api';
import { PosShift, ShiftReport } from '../../types/pos';
import { formatMoney } from './money';

interface Props {
  registerId: string;
  shift: PosShift | null;
  onClose: () => void;
  onChanged: (shift: PosShift | null) => void;
}

export const ShiftDialog: React.FC<Props> = ({ registerId, shift, onClose, onChanged }) => {
  const [amount, setAmount] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [report, setReport] = useState<ShiftReport | null>(null);
  const [busy, setBusy] = useState(false);

  const value = parseFloat(amount.replace(',', '.'));

  const submit = async () => {
    if (!Number.isFinite(value) || value < 0) {
      setError('Introduzca un importe válido');
      return;
    }
    setBusy(true);
    setError(null);
    try {
      if (shift) {
        setReport(await posApi.closeShift(shift.id, value));
      } else {
        onChanged(await posApi.openShift(registerId, value));
      }
    } catch (e) {
      setError(posErrorMessage(e, 'No se pudo completar la operación'));
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-sm">
        <div className="px-5 py-4 border-b border-gray-200">
          <h2 className="text-lg font-semibold">{report ? 'Cierre de turno (Z)' : shift ? 'Cerrar turno' : 'Abrir turno'}</h2>
        </div>
        <div className="px-5 py-4 flex flex-col gap-3">
          {error && <p role="alert" className="text-sm text-red-600">{error}</p>}
          {report ? (
            <div className="text-sm flex flex-col gap-1">
              <p>Ventas: <strong>{report.salesCount}</strong> · {formatMoney(report.totalSales)}</p>
              {report.byPaymentMethod.map((line) => (
                <p key={line.key}>{line.key === 'Cash' ? 'Efectivo' : line.key === 'Card' ? 'Tarjeta' : line.key}: {formatMoney(line.amount)}</p>
              ))}
              {report.byTaxRate.map((line) => (
                <p key={line.rate} className="text-gray-600">IVA {line.rate}%: base {formatMoney(line.taxableBase)} · cuota {formatMoney(line.taxAmount)}</p>
              ))}
              <p className="mt-2">Efectivo esperado: <strong>{formatMoney(report.expectedCash)}</strong></p>
              <p>Efectivo contado: <strong>{formatMoney(report.shift.countedCash ?? 0)}</strong></p>
              <p>
                Diferencia:{' '}
                <strong className={Math.abs((report.shift.countedCash ?? 0) - report.expectedCash) > 0.005 ? 'text-red-600' : 'text-green-700'}>
                  {formatMoney((report.shift.countedCash ?? 0) - report.expectedCash)}
                </strong>
              </p>
            </div>
          ) : (
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                {shift ? 'Efectivo contado en caja' : 'Fondo de caja inicial'}
              </label>
              <input className="input-field" inputMode="decimal" autoFocus value={amount} onChange={(e) => setAmount(e.target.value)} />
            </div>
          )}
        </div>
        <div className="px-5 py-3 border-t border-gray-200 flex justify-end gap-2">
          {report ? (
            <button className="btn-primary" onClick={() => onChanged(null)}>Cerrar</button>
          ) : (
            <>
              <button className="btn-secondary" onClick={onClose}>Cancelar</button>
              <button className="btn-primary" disabled={busy} onClick={() => void submit()}>
                {shift ? 'Cerrar turno' : 'Abrir turno'}
              </button>
            </>
          )}
        </div>
      </div>
    </div>
  );
};
