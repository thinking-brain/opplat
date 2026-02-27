import React, { useState, useEffect, useRef } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  Alert,
  CircularProgress,
  Divider,
  Stack,
} from '@mui/material';
import {
  CloudUpload as UploadIcon,
  Delete as DeleteIcon,
} from '@mui/icons-material';
import { licenseApi } from '../api/license.api';
import type { LicenseInfo } from '../types';

export const LicensePage: React.FC = () => {
  const [license, setLicense] = useState<LicenseInfo | null>(null);
  const [loading, setLoading] = useState(true);
  const [uploading, setUploading] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [alert, setAlert] = useState<{ type: 'success' | 'error'; message: string } | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    void loadLicense();
  }, []);

  const loadLicense = async () => {
    try {
      setLoading(true);
      const response = await licenseApi.get();
      setLicense(response.data);
    } catch {
      setLicense(null);
    } finally {
      setLoading(false);
    }
  };

  const handleUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    try {
      setUploading(true);
      await licenseApi.upload(file);
      setAlert({ type: 'success', message: 'License uploaded successfully.' });
      await loadLicense();
    } catch {
      setAlert({ type: 'error', message: 'Failed to upload license.' });
    } finally {
      setUploading(false);
      if (fileInputRef.current) fileInputRef.current.value = '';
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('Are you sure you want to delete the current license?')) return;
    try {
      setDeleting(true);
      await licenseApi.delete();
      setAlert({ type: 'success', message: 'License deleted.' });
      setLicense(null);
    } catch {
      setAlert({ type: 'error', message: 'Failed to delete license.' });
    } finally {
      setDeleting(false);
    }
  };

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        License / Settings
      </Typography>

      {alert && (
        <Alert severity={alert.type} onClose={() => setAlert(null)} sx={{ mb: 2 }}>
          {alert.message}
        </Alert>
      )}

      <Card>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Current License
          </Typography>
          <Divider sx={{ mb: 2 }} />
          {loading ? (
            <Box display="flex" justifyContent="center" py={3}>
              <CircularProgress />
            </Box>
          ) : license ? (
            <Box>
              <Typography variant="body1">
                <strong>Subscriber:</strong> {license.subscriptor}
              </Typography>
              <Typography variant="body1" sx={{ mt: 1 }}>
                <strong>Expiration:</strong>{' '}
                {new Date(license.fechaVencimiento).toLocaleDateString()}
              </Typography>
            </Box>
          ) : (
            <Alert severity="warning">No active license found.</Alert>
          )}
        </CardContent>
      </Card>

      <Card sx={{ mt: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Manage License
          </Typography>
          <Divider sx={{ mb: 2 }} />
          <Stack direction="row" spacing={2} flexWrap="wrap">
            <input
              ref={fileInputRef}
              type="file"
              accept=".lic,.xml,.json"
              style={{ display: 'none' }}
              onChange={(e) => { void handleUpload(e); }}
            />
            <Button
              variant="contained"
              startIcon={
                uploading ? <CircularProgress size={18} color="inherit" /> : <UploadIcon />
              }
              onClick={() => fileInputRef.current?.click()}
              disabled={uploading}
            >
              {uploading ? 'Uploading…' : 'Upload License'}
            </Button>
            {license && (
              <Button
                variant="outlined"
                color="error"
                startIcon={
                  deleting ? <CircularProgress size={18} color="inherit" /> : <DeleteIcon />
                }
                onClick={() => { void handleDelete(); }}
                disabled={deleting}
              >
                {deleting ? 'Deleting…' : 'Delete License'}
              </Button>
            )}
          </Stack>
        </CardContent>
      </Card>
    </Box>
  );
};
