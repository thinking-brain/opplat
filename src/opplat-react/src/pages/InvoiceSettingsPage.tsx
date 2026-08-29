import React, { useState, useEffect } from 'react';
import { invoicesApi } from '../api/invoices.api';
import { TenantFiscalSettings, InvoicingMode } from '../types';
import { LoadingSpinner } from '../components/LoadingSpinner';

const INVOICING_MODES: { value: InvoicingMode; label: string; description: string }[] = [
  { value: 'None', label: 'Sin fiscalización', description: 'Para negocios fuera de España o durante la fase de prueba.' },
  { value: 'Verifactu', label: 'VERI*FACTU (envío automático)', description: 'Envío en tiempo real a la AEAT según RD 1007/2023. Requiere certificado en una fase posterior.' },
  { value: 'NonVerifactuSigned', label: 'SIF sin VERI*FACTU (firma local)', description: 'Hash-chain local + firma XAdES. No disponible en esta versión MVP.' },
];

const empty: TenantFiscalSettings = {
  legalName: '',
  taxId: '',
  fiscalAddress: '',
  country: 'ES',
  businessSector: '',
  defaultSeries: 'A',
  invoicingMode: 'None',
  simplifiedInvoiceThreshold: 400,
  softwareLicenseId: '',
};

export const InvoiceSettingsPage: React.FC = () => {
  const [form, setForm] = useState<TenantFiscalSettings>(empty);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });

  useEffect(() => {
    (async () => {
      try {
        setLoading(true);
        const data = await invoicesApi.getSettings();
        setForm({ ...empty, ...data });
      } catch {
        setSnackbar({ open: true, message: 'Error al cargar la configuración fiscal', severity: 'error' });
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  useEffect(() => {
    if (!snackbar.open) return;
    const t = setTimeout(() => setSnackbar(s => ({ ...s, open: false })), 5000);
    return () => clearTimeout(t);
  }, [snackbar.open]);

  const handleChange = (field: keyof TenantFiscalSettings, value: string | number) => {
    setForm(f => ({ ...f, [field]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    try {
      const result = await invoicesApi.saveSettings(form);
      setSnackbar({ open: true, message: result.message, severity: result.status ? 'success' : 'error' });
    } catch {
      setSnackbar({ open: true, message: 'Error al guardar la configuración', severity: 'error' });
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <div className="max-w-2xl">
      <h1 className="text-2xl font-bold mb-6">Configuración Fiscal del Negocio</h1>

      {snackbar.open && (
        <div className={`mb-4 p-3 rounded text-sm ${snackbar.severity === 'success' ? 'bg-green-50 text-green-800 border border-green-200' : 'bg-red-50 text-red-800 border border-red-200'}`}>
          {snackbar.message}
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-5">
        <div className="card p-4 space-y-4">
          <h2 className="font-semibold text-gray-700">Datos del emisor</h2>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Razón social *</label>
            <input
              className="input-field"
              required
              value={form.legalName}
              onChange={e => handleChange('legalName', e.target.value)}
            />
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">NIF / CIF *</label>
              <input
                className="input-field font-mono"
                required
                placeholder="B12345678"
                value={form.taxId}
                onChange={e => handleChange('taxId', e.target.value)}
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">País</label>
              <input
                className="input-field"
                placeholder="ES"
                value={form.country ?? ''}
                onChange={e => handleChange('country', e.target.value)}
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Domicilio fiscal *</label>
            <input
              className="input-field"
              required
              value={form.fiscalAddress}
              onChange={e => handleChange('fiscalAddress', e.target.value)}
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Sector de actividad</label>
            <input
              className="input-field"
              placeholder="p. ej. Hostelería, Peluquería..."
              value={form.businessSector ?? ''}
              onChange={e => handleChange('businessSector', e.target.value)}
            />
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Email de contacto</label>
              <input
                type="email"
                className="input-field"
                placeholder="info@empresa.com"
                value={form.email ?? ''}
                onChange={e => handleChange('email', e.target.value)}
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Teléfono</label>
              <input
                className="input-field"
                placeholder="+34 900 000 000"
                value={form.phone ?? ''}
                onChange={e => handleChange('phone', e.target.value)}
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Web</label>
            <input
              className="input-field"
              placeholder="https://empresa.com"
              value={form.website ?? ''}
              onChange={e => handleChange('website', e.target.value)}
            />
          </div>
        </div>

        <div className="card p-4 space-y-4">
          <h2 className="font-semibold text-gray-700">Facturación</h2>

          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Serie por defecto *</label>
              <input
                className="input-field font-mono"
                required
                placeholder="A"
                maxLength={10}
                value={form.defaultSeries}
                onChange={e => handleChange('defaultSeries', e.target.value.toUpperCase())}
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Umbral factura simplificada (€)</label>
              <input
                type="number"
                className="input-field"
                min={0}
                step={0.01}
                value={form.simplifiedInvoiceThreshold}
                onChange={e => handleChange('simplifiedInvoiceThreshold', parseFloat(e.target.value) || 0)}
              />
              <p className="text-xs text-gray-400 mt-0.5">Por defecto 400 € según Verifactu</p>
            </div>
          </div>
        </div>

        <div className="card p-4 space-y-3">
          <h2 className="font-semibold text-gray-700">Modo de fiscalización (Verifactu)</h2>
          {INVOICING_MODES.map(mode => (
            <label key={mode.value} className={`flex items-start gap-3 p-3 rounded-lg border cursor-pointer transition-colors ${form.invoicingMode === mode.value ? 'border-blue-400 bg-blue-50' : 'border-gray-200 hover:border-gray-300'}`}>
              <input
                type="radio"
                name="invoicingMode"
                value={mode.value}
                checked={form.invoicingMode === mode.value}
                onChange={() => handleChange('invoicingMode', mode.value)}
                className="mt-0.5"
              />
              <div>
                <p className="text-sm font-medium">{mode.label}</p>
                <p className="text-xs text-gray-500">{mode.description}</p>
              </div>
            </label>
          ))}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Nº de instalación (SoftwareLicenseId)</label>
            <input
              className="input-field font-mono"
              placeholder="Proporcionado por la AEAT"
              value={form.softwareLicenseId ?? ''}
              onChange={e => handleChange('softwareLicenseId', e.target.value)}
            />
          </div>
        </div>

        <button
          type="submit"
          className="btn-primary w-full py-2.5"
          disabled={saving}
        >
          {saving ? 'Guardando...' : 'Guardar configuración'}
        </button>
      </form>
    </div>
  );
};
