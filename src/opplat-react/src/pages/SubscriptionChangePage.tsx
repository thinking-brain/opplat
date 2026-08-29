import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { billingApi } from '../api/billing.api';
import { PlanSelectionCards } from '../components/PlanSelectionCards';
import type { SubscriptionPlan, TenantSubscriptionDetails } from '../types';

export const SubscriptionChangePage = () => {
  const navigate = useNavigate();
  const [subscription, setSubscription] = useState<TenantSubscriptionDetails | null>(null);
  const [plans, setPlans] = useState<SubscriptionPlan[]>([]);
  const [plansLoading, setPlansLoading] = useState(true);
  const [plansError, setPlansError] = useState<string | null>(null);
  const [selectedPlanId, setSelectedPlanId] = useState<string>('');
  const [billingInterval, setBillingInterval] = useState<'Monthly' | 'Annual'>('Monthly');
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState<string>();

  useEffect(() => {
    const loadData = async () => {
      try {
        const [subscriptionResponse, planResponse] = await Promise.all([
          billingApi.getSubscription(),
          billingApi.listPlans(),
        ]);
        setSubscription(subscriptionResponse);
        setPlans(planResponse);
        setSelectedPlanId(subscriptionResponse.subscriptionPlanId);
        setBillingInterval(subscriptionResponse.billingInterval === 'Annual' ? 'Annual' : 'Monthly');
      } catch {
        setPlansError('No se pudo cargar la información de planes.');
      } finally {
        setPlansLoading(false);
      }
    };

    void loadData();
  }, []);

  const handleSubmit = async () => {
    if (!selectedPlanId) return;
    setSaving(true);
    setMessage(undefined);
    try {
      const nextSubscription = await billingApi.changeSubscription({
        subscriptionPlanId: selectedPlanId,
        billingInterval,
      });
      setSubscription(nextSubscription);
      setMessage(`Plan actualizado a ${nextSubscription.subscriptionPlanName}.`);
      setTimeout(() => navigate('/billing'), 900);
    } catch {
      setMessage('No se pudo actualizar la suscripción.');
    } finally {
      setSaving(false);
    }
  };

  return (
    <section className="mx-auto max-w-5xl space-y-6 px-4 py-6">
      <div>
        <h1 className="text-2xl font-semibold text-gray-900">Cambiar plan</h1>
        <p className="mt-1 text-gray-600">Elige un nuevo plan con la misma experiencia visual que en el registro.</p>
      </div>

      {message && <div className="rounded border border-blue-200 bg-blue-50 px-4 py-3 text-blue-800">{message}</div>}

      {subscription && (
        <div className="rounded border bg-white p-4 shadow-sm">
          <p className="text-sm text-gray-600">Plan actual: <span className="font-semibold text-gray-900">{subscription.subscriptionPlanName}</span></p>
          <p className="text-sm text-gray-600">Estado: <span className="font-semibold text-gray-900">{subscription.billingStatus}</span></p>
        </div>
      )}

      <div className="rounded border bg-white p-5 shadow-sm">
        <PlanSelectionCards
          plans={plans}
          plansLoading={plansLoading}
          plansError={plansError}
          selectedPlanId={selectedPlanId}
          onSelectPlan={setSelectedPlanId}
          priceMode={billingInterval === 'Annual' ? 'annual' : 'monthly'}
          onChangePriceMode={(mode) => setBillingInterval(mode === 'annual' ? 'Annual' : 'Monthly')}
          showPriceToggle
        />

        <div className="mt-6 flex justify-end gap-3">
          <button type="button" className="btn-ghost" onClick={() => navigate('/billing')}>
            Volver
          </button>
          <button type="button" className="btn-primary" onClick={() => void handleSubmit()} disabled={saving || !selectedPlanId}>
            {saving ? 'Guardando...' : 'Guardar cambio'}
          </button>
        </div>
      </div>
    </section>
  );
};
