import type { SubscriptionPlan } from '../types';

interface PlanSelectionCardsProps {
  plans: SubscriptionPlan[];
  plansLoading: boolean;
  plansError?: string | null;
  selectedPlanId: string;
  onSelectPlan: (planId: string) => void;
  priceMode: 'monthly' | 'annual';
  onChangePriceMode?: (mode: 'monthly' | 'annual') => void;
  showPriceToggle?: boolean;
}

const formatPrice = (value: number, currency: string) =>
  new Intl.NumberFormat('es-ES', {
    style: 'currency',
    currency: currency || 'EUR',
    maximumFractionDigits: 0,
  }).format(value);

export const PlanSelectionCards = ({
  plans,
  plansLoading,
  plansError,
  selectedPlanId,
  onSelectPlan,
  priceMode,
  onChangePriceMode,
  showPriceToggle = false,
}: PlanSelectionCardsProps) => {
  if (plansLoading) {
    return (
      <div className="flex justify-center py-8">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
      </div>
    );
  }

  if (plansError) {
    return (
      <div className="bg-red-50 border border-red-300 text-red-700 px-4 py-3 rounded-md text-sm">
        {plansError}
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {showPriceToggle && onChangePriceMode && (
        <div className="inline-flex rounded-full border border-gray-200 bg-white p-1 shadow-sm">
          <button
            type="button"
            onClick={() => onChangePriceMode('monthly')}
            className={`rounded-full px-4 py-2 text-sm font-medium ${priceMode === 'monthly' ? 'bg-blue-600 text-white' : 'text-gray-600 hover:bg-gray-50'}`}
          >
            Mensual
          </button>
          <button
            type="button"
            onClick={() => onChangePriceMode('annual')}
            className={`rounded-full px-4 py-2 text-sm font-medium ${priceMode === 'annual' ? 'bg-blue-600 text-white' : 'text-gray-600 hover:bg-gray-50'}`}
          >
            Anual
          </button>
        </div>
      )}

      <div className="flex flex-wrap gap-4">
        {plans.map((plan) => {
          const price = priceMode === 'annual' ? plan.pricingAnnual : plan.pricingMonthly;
          const label = priceMode === 'annual' ? 'por año' : 'por mes';
          return (
            <div key={plan.id} className="flex-1 min-w-[220px] max-w-[280px]">
              <button
                type="button"
                onClick={() => onSelectPlan(plan.id)}
                className={`w-full text-left card p-5 transition-all ${selectedPlanId === plan.id ? 'border-blue-600 border-2' : 'hover:shadow-md'}`}
              >
                <p className="text-lg font-semibold">{plan.name}</p>
                {plan.description && (
                  <p className="text-sm text-gray-500 mb-2">{plan.description}</p>
                )}
                <p className="text-base font-bold">{formatPrice(price, plan.currency)} / {label}</p>
                <p className="text-sm text-gray-500">Hasta {plan.maxActiveUsers} usuarios</p>
              </button>
            </div>
          );
        })}
      </div>
    </div>
  );
};
