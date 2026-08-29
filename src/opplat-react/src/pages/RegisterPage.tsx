import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getSubscriptionPlans, registerTenant } from '../api/auth.api';
import type { SubscriptionPlan } from '../types';

const STEPS = ['Choose a Plan', 'Business Info', 'Your Account', 'Payment Method (optional)'];

const slugify = (value: string): string =>
  value
    .toLowerCase()
    .replace(/\s+/g, '-')
    .replace(/[^a-z0-9-]/g, '')
    .slice(0, 30);

const IDENTIFIER_REGEX = /^[a-z0-9-]{3,30}$/;

export const RegisterPage = () => {
  const navigate = useNavigate();

  const [activeStep, setActiveStep] = useState(0);

  // Step 0 — Plan
  const [plans, setPlans] = useState<SubscriptionPlan[]>([]);
  const [plansLoading, setPlansLoading] = useState(true);
  const [plansError, setPlansError] = useState<string | null>(null);
  const [selectedPlanId, setSelectedPlanId] = useState<string>('');

  // Step 1 — Business
  const [businessName, setBusinessName] = useState('');
  const [tenantIdentifier, setTenantIdentifier] = useState('');
  const [identifierManuallyEdited, setIdentifierManuallyEdited] = useState(false);

  // Step 2 — Account
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const [cardNumber, setCardNumber] = useState('');
  const [cardExpMonth, setCardExpMonth] = useState(1);
  const [cardExpYear, setCardExpYear] = useState(new Date().getFullYear());
  const [cardCvc, setCardCvc] = useState('');

  // Submit state
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitSuccess, setSubmitSuccess] = useState(false);

  useEffect(() => {
    getSubscriptionPlans()
      .then((data) => setPlans(data))
      .catch(() => setPlansError('Failed to load subscription plans. Please try again.'))
      .finally(() => setPlansLoading(false));
  }, []);

  useEffect(() => {
    if (!identifierManuallyEdited) {
      setTenantIdentifier(slugify(businessName));
    }
  }, [businessName, identifierManuallyEdited]);

  const isStep0Valid = selectedPlanId !== '';
  const isStep1Valid =
    businessName.trim().length > 0 && IDENTIFIER_REGEX.test(tenantIdentifier);
  const isStep2Valid =
    firstName.trim().length > 0 &&
    lastName.trim().length > 0 &&
    email.trim().length > 0 &&
    password.length >= 8 &&
    password === confirmPassword;
  const hasCardInput = cardNumber.length > 0 || cardCvc.length > 0;
  const isStep3Valid = !hasCardInput || (cardNumber.length > 0 && cardCvc.length >= 3 && cardCvc.length <= 4 && cardExpMonth >= 1 && cardExpMonth <= 12 && cardExpYear >= new Date().getFullYear());

  const canProceed = [isStep0Valid, isStep1Valid, isStep2Valid, isStep3Valid][activeStep];

  const handleNext = () => {
    if (activeStep < STEPS.length - 1) {
      setActiveStep((s) => s + 1);
    }
  };

  const handleBack = () => setActiveStep((s) => s - 1);

  const handleSubmit = async () => {
    setSubmitting(true);
    setSubmitError(null);
    try {
      const username = `${tenantIdentifier}-${firstName.toLowerCase()}`;
      const result = await registerTenant({
        firstName,
        lastName,
        username,
        email,
        password,
        businessName,
        tenantIdentifier,
        subscriptionPlanId: selectedPlanId,
        ...(hasCardInput ? { cardNumber, cardExpMonth, cardExpYear, cardCvc } : {}),
      });
      if (result.succeeded) {
        setSubmitSuccess(true);
      } else {
        setSubmitError(result.message ?? 'Registration failed. Please try again.');
      }
    } catch {
      setSubmitError('An unexpected error occurred. Please try again.');
    } finally {
      setSubmitting(false);
    }
  };

  if (submitSuccess) {
    return (
      <div className="max-w-lg mx-auto mt-16 px-4">
        <div className="bg-green-50 border border-green-300 text-green-700 px-4 py-3 rounded-md text-sm mb-4">
          Registration successful! Your account is being set up.
        </div>
        <button className="btn-primary" onClick={() => navigate('/login')}>
          Go to Login
        </button>
      </div>
    );
  }

  return (
    <div className="max-w-2xl mx-auto mt-10 px-4">
      <h1 className="text-2xl font-semibold mb-6">Create your account</h1>

      {/* Step indicator */}
      <div className="flex items-center gap-2 mb-8">
        {STEPS.map((label, index) => (
          <React.Fragment key={label}>
            <div className="flex items-center gap-1.5">
              <div className={`w-7 h-7 rounded-full flex items-center justify-center text-sm font-medium ${index <= activeStep ? 'bg-blue-600 text-white' : 'bg-gray-200 text-gray-600'}`}>
                {index + 1}
              </div>
              <span className={`text-sm hidden sm:block ${index === activeStep ? 'font-semibold text-gray-900' : 'text-gray-500'}`}>{label}</span>
            </div>
            {index < STEPS.length - 1 && <div className={`flex-1 h-0.5 ${index < activeStep ? 'bg-blue-600' : 'bg-gray-200'}`} />}
          </React.Fragment>
        ))}
      </div>

      {/* Step 0: Plan Selection */}
      {activeStep === 0 && (
        <div>
          {plansLoading && (
            <div className="flex justify-center py-8">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
            </div>
          )}
          {plansError && (
            <div className="bg-red-50 border border-red-300 text-red-700 px-4 py-3 rounded-md text-sm">
              {plansError}
            </div>
          )}
          {!plansLoading && !plansError && (
            <div className="flex flex-wrap gap-4">
              {plans.map((plan) => (
                <div key={plan.id} className="flex-1 min-w-[200px] max-w-[280px]">
                  <button
                    type="button"
                    onClick={() => setSelectedPlanId(plan.id)}
                    className={`w-full text-left card p-5 transition-all ${selectedPlanId === plan.id ? 'border-blue-600 border-2' : 'hover:shadow-md'}`}
                  >
                    <p className="text-lg font-semibold">{plan.name}</p>
                    {plan.description && (
                      <p className="text-sm text-gray-500 mb-2">{plan.description}</p>
                    )}
                    <p className="text-base font-bold">${plan.pricingMonthly}/month</p>
                    <p className="text-sm text-gray-500">Up to {plan.maxActiveUsers} users</p>
                  </button>
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {/* Step 1: Business Info */}
      {activeStep === 1 && (
        <div className="flex flex-col gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Business Name *</label>
            <input
              className="input-field"
              value={businessName}
              onChange={(e) => setBusinessName(e.target.value)}
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Tenant Identifier *</label>
            <input
              className={`input-field ${tenantIdentifier.length > 0 && !IDENTIFIER_REGEX.test(tenantIdentifier) ? 'border-red-500' : ''}`}
              value={tenantIdentifier}
              onChange={(e) => {
                setTenantIdentifier(e.target.value);
                setIdentifierManuallyEdited(true);
              }}
              required
            />
            <p className="text-xs text-gray-500 mt-1">Lowercase letters, numbers, and hyphens only (3–30 characters)</p>
          </div>
        </div>
      )}

      {/* Step 2: Account Info */}
      {activeStep === 2 && (
        <div className="flex flex-col gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">First Name *</label>
            <input className="input-field" value={firstName} onChange={(e) => setFirstName(e.target.value)} required />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Last Name *</label>
            <input className="input-field" value={lastName} onChange={(e) => setLastName(e.target.value)} required />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Email *</label>
            <input className="input-field" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Password *</label>
            <input className="input-field" type="password" value={password} onChange={(e) => setPassword(e.target.value)} required minLength={8} />
            <p className="text-xs text-gray-500 mt-1">Minimum 8 characters</p>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Confirm Password *</label>
            <input
              className={`input-field ${confirmPassword.length > 0 && password !== confirmPassword ? 'border-red-500' : ''}`}
              type="password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              required
            />
            {confirmPassword.length > 0 && password !== confirmPassword && (
              <p className="text-xs text-red-600 mt-1">Passwords do not match</p>
            )}
          </div>
          {submitError && (
            <div className="bg-red-50 border border-red-300 text-red-700 px-4 py-3 rounded-md text-sm">
              {submitError}
            </div>
          )}
        </div>
      )}

      {activeStep === 3 && (
        <div className="flex flex-col gap-4">
          <p className="text-sm text-gray-600">Puedes añadir una tarjeta ahora o hacerlo más tarde desde Facturación. Este entorno usa un simulador de pagos.</p>
          <div><label className="block text-sm font-medium text-gray-700 mb-1">Número de tarjeta</label><input className="input-field" inputMode="numeric" autoComplete="cc-number" value={cardNumber} onChange={(e) => setCardNumber(e.target.value)} /></div>
          <div className="grid grid-cols-2 gap-4"><div><label className="block text-sm font-medium text-gray-700 mb-1">Mes de caducidad</label><input className="input-field" type="number" min="1" max="12" value={cardExpMonth} onChange={(e) => setCardExpMonth(Number(e.target.value))} /></div><div><label className="block text-sm font-medium text-gray-700 mb-1">Año de caducidad</label><input className="input-field" type="number" min={new Date().getFullYear()} value={cardExpYear} onChange={(e) => setCardExpYear(Number(e.target.value))} /></div></div>
          <div><label className="block text-sm font-medium text-gray-700 mb-1">CVC</label><input className="input-field" inputMode="numeric" autoComplete="cc-csc" maxLength={4} value={cardCvc} onChange={(e) => setCardCvc(e.target.value)} /></div>
          {hasCardInput && !isStep3Valid && <p className="text-sm text-red-600">Completa los datos de la tarjeta o deja todos los campos vacíos para continuar sin tarjeta.</p>}
          {submitError && <div className="bg-red-50 border border-red-300 text-red-700 px-4 py-3 rounded-md text-sm">{submitError}</div>}
        </div>
      )}

      <hr className="my-6" />

      <div className="flex justify-between">
        <button className="btn-ghost" onClick={handleBack} disabled={activeStep === 0}>
          Back
        </button>
        {activeStep < STEPS.length - 1 ? (
          <button
            className="btn-primary"
            onClick={handleNext}
            disabled={!canProceed || plansLoading}
          >
            Next
          </button>
        ) : (
          <button
            className="btn-primary flex items-center gap-2"
            onClick={() => { void handleSubmit(); }}
            disabled={!canProceed || submitting}
          >
            {submitting && <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white" />}
            {submitting ? 'Creating account...' : 'Create Account'}
          </button>
        )}
      </div>
    </div>
  );
};



