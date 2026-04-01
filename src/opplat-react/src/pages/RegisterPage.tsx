import { useState, useEffect } from 'react';
import {
  Box,
  Button,
  Card,
  CardContent,
  CircularProgress,
  Divider,
  Stepper,
  Step,
  StepLabel,
  TextField,
  Typography,
  Alert,
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import { getSubscriptionPlans, registerTenant } from '../api/auth.api';
import type { SubscriptionPlan } from '../types';

const STEPS = ['Choose a Plan', 'Business Info', 'Your Account'];

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

  const canProceed = [isStep0Valid, isStep1Valid, isStep2Valid][activeStep];

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
      <Box sx={{ maxWidth: 480, mx: 'auto', mt: 8, p: 3 }}>
        <Alert severity="success" sx={{ mb: 2 }}>
          Registration successful! Your account is being set up.
        </Alert>
        <Button variant="contained" onClick={() => navigate('/login')}>
          Go to Login
        </Button>
      </Box>
    );
  }

  return (
    <Box sx={{ maxWidth: 700, mx: 'auto', mt: 6, p: 3 }}>
      <Typography variant="h5" fontWeight={600} mb={3}>
        Create your account
      </Typography>

      <Stepper activeStep={activeStep} sx={{ mb: 4 }}>
        {STEPS.map((label) => (
          <Step key={label}>
            <StepLabel>{label}</StepLabel>
          </Step>
        ))}
      </Stepper>

      {/* Step 0: Plan Selection */}
      {activeStep === 0 && (
        <Box>
          {plansLoading && (
            <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}>
              <CircularProgress />
            </Box>
          )}
          {plansError && <Alert severity="error">{plansError}</Alert>}
          {!plansLoading && !plansError && (
            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2 }}>
              {plans.map((plan) => (
                <Box key={plan.id} sx={{ flexBasis: { xs: '100%', sm: 'calc(50% - 8px)' } }}>
                  <Card
                    onClick={() => setSelectedPlanId(plan.id)}
                    sx={{
                      cursor: 'pointer',
                      border: selectedPlanId === plan.id ? '2px solid' : '2px solid transparent',
                      borderColor: selectedPlanId === plan.id ? 'primary.main' : 'transparent',
                      transition: 'border-color 0.2s',
                    }}
                  >
                    <CardContent>
                      <Typography variant="h6">{plan.name}</Typography>
                      {plan.description && (
                        <Typography variant="body2" color="text.secondary" mb={1}>
                          {plan.description}
                        </Typography>
                      )}
                      <Typography variant="body1" fontWeight={600}>
                        ${plan.pricingMonthly}/month
                      </Typography>
                      <Typography variant="body2" color="text.secondary">
                        Up to {plan.maxActiveUsers} users
                      </Typography>
                    </CardContent>
                  </Card>
                </Box>
              ))}
            </Box>
          )}
        </Box>
      )}

      {/* Step 1: Business Info */}
      {activeStep === 1 && (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          <TextField
            label="Business Name"
            value={businessName}
            onChange={(e) => setBusinessName(e.target.value)}
            required
            fullWidth
          />
          <TextField
            label="Tenant Identifier"
            value={tenantIdentifier}
            onChange={(e) => {
              setTenantIdentifier(e.target.value);
              setIdentifierManuallyEdited(true);
            }}
            required
            fullWidth
            helperText="Lowercase letters, numbers, and hyphens only (3–30 characters)"
            error={tenantIdentifier.length > 0 && !IDENTIFIER_REGEX.test(tenantIdentifier)}
          />
        </Box>
      )}

      {/* Step 2: Account Info */}
      {activeStep === 2 && (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
          <TextField
            label="First Name"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
            required
            fullWidth
          />
          <TextField
            label="Last Name"
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
            required
            fullWidth
          />
          <TextField
            label="Email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            fullWidth
          />
          <TextField
            label="Password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            fullWidth
            helperText="Minimum 8 characters"
            inputProps={{ minLength: 8 }}
          />
          <TextField
            label="Confirm Password"
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            required
            fullWidth
            error={confirmPassword.length > 0 && password !== confirmPassword}
            helperText={
              confirmPassword.length > 0 && password !== confirmPassword
                ? 'Passwords do not match'
                : ''
            }
          />
          {submitError && <Alert severity="error">{submitError}</Alert>}
        </Box>
      )}

      <Divider sx={{ my: 3 }} />

      <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
        <Button onClick={handleBack} disabled={activeStep === 0}>
          Back
        </Button>
        {activeStep < STEPS.length - 1 ? (
          <Button
            variant="contained"
            onClick={handleNext}
            disabled={!canProceed || plansLoading}
          >
            Next
          </Button>
        ) : (
          <Button
            variant="contained"
            onClick={() => { void handleSubmit(); }}
            disabled={!canProceed || submitting}
            startIcon={submitting ? <CircularProgress size={16} color="inherit" /> : null}
          >
            {submitting ? 'Creating account...' : 'Create Account'}
          </Button>
        )}
      </Box>
    </Box>
  );
};
