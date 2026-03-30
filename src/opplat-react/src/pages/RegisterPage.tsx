import React, { useState } from 'react';
import { Link as RouterLink, useNavigate } from 'react-router-dom';
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  CircularProgress,
  Container,
  Link,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import { PersonAdd as RegisterIcon } from '@mui/icons-material';
import { registerUser } from '../api/auth.api';

interface FormValues {
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
}

const initialValues: FormValues = {
  firstName: '',
  lastName: '',
  username: '',
  email: '',
  password: '',
  confirmPassword: '',
};

export const RegisterPage: React.FC = () => {
  const navigate = useNavigate();
  const [values, setValues] = useState<FormValues>(initialValues);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setValues(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (values.password !== values.confirmPassword) {
      setError('Passwords do not match.');
      return;
    }

    setLoading(true);
    try {
      await registerUser({
        name: values.firstName,
        lastName: values.lastName,
        username: values.username,
        email: values.email,
        password: values.password,
      });
      setSuccess(true);
      setTimeout(() => navigate('/login'), 2000);
    } catch (err: unknown) {
      const axiosError = err as { response?: { data?: { error?: string } } };
      setError(axiosError?.response?.data?.error ?? 'Registration failed. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="sm">
      <Box display="flex" alignItems="center" justifyContent="center" minHeight="100vh">
        <Card sx={{ width: '100%', maxWidth: 480 }}>
          <CardContent sx={{ p: 4 }}>
            <Stack spacing={3} alignItems="center">
              <RegisterIcon color="primary" sx={{ fontSize: 56 }} />
              <Box textAlign="center">
                <Typography variant="h4" component="h1" gutterBottom>
                  Create Account
                </Typography>
                <Typography variant="body1" color="text.secondary">
                  Register to access the Opplat platform.
                </Typography>
              </Box>

              {error && <Alert severity="error" sx={{ width: '100%' }}>{error}</Alert>}
              {success && (
                <Alert severity="success" sx={{ width: '100%' }}>
                  Account created! Redirecting to login...
                </Alert>
              )}

              <Box component="form" onSubmit={handleSubmit} sx={{ width: '100%' }}>
                <Stack spacing={2}>
                  <Stack direction="row" spacing={2}>
                    <TextField
                      label="First Name"
                      name="firstName"
                      value={values.firstName}
                      onChange={handleChange}
                      required
                      fullWidth
                      disabled={loading || success}
                    />
                    <TextField
                      label="Last Name"
                      name="lastName"
                      value={values.lastName}
                      onChange={handleChange}
                      required
                      fullWidth
                      disabled={loading || success}
                    />
                  </Stack>
                  <TextField
                    label="Username"
                    name="username"
                    value={values.username}
                    onChange={handleChange}
                    required
                    fullWidth
                    disabled={loading || success}
                    autoComplete="username"
                  />
                  <TextField
                    label="Email"
                    name="email"
                    type="email"
                    value={values.email}
                    onChange={handleChange}
                    required
                    fullWidth
                    disabled={loading || success}
                    autoComplete="email"
                  />
                  <TextField
                    label="Password"
                    name="password"
                    type="password"
                    value={values.password}
                    onChange={handleChange}
                    required
                    fullWidth
                    disabled={loading || success}
                    autoComplete="new-password"
                    helperText="Minimum 8 characters"
                  />
                  <TextField
                    label="Confirm Password"
                    name="confirmPassword"
                    type="password"
                    value={values.confirmPassword}
                    onChange={handleChange}
                    required
                    fullWidth
                    disabled={loading || success}
                    autoComplete="new-password"
                  />
                  <Button
                    type="submit"
                    fullWidth
                    size="large"
                    variant="contained"
                    disabled={loading || success}
                    startIcon={loading ? <CircularProgress size={18} color="inherit" /> : <RegisterIcon />}
                  >
                    {loading ? 'Creating account...' : 'Create Account'}
                  </Button>
                </Stack>
              </Box>

              <Typography variant="body2" color="text.secondary">
                Already have an account?{' '}
                <Link component={RouterLink} to="/login">
                  Sign in
                </Link>
              </Typography>
            </Stack>
          </CardContent>
        </Card>
      </Box>
    </Container>
  );
};
