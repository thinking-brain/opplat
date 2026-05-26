import React, { useState, useEffect } from 'react';
import {
  Alert,
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  Paper,
  Snackbar,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Tooltip,
  Typography,
} from '@mui/material';
import { Add as AddIcon, Delete as DeleteIcon, Edit as EditIcon } from '@mui/icons-material';
import { inventoryApi } from '../api/inventory.api';
import type { ProductClassification } from '../types';
import { LoadingSpinner } from '../components/LoadingSpinner';

type ClassificationFormData = Omit<ProductClassification, 'id'>;

const emptyForm: ClassificationFormData = {
  description: '',
};

export const ProductClassificationsPage: React.FC = () => {
  const [classifications, setClassifications] = useState<ProductClassification[]>([]);
  const [loading, setLoading] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [confirmDialogOpen, setConfirmDialogOpen] = useState(false);
  const [editingClassification, setEditingClassification] = useState<ProductClassification | null>(null);
  const [deletingClassification, setDeletingClassification] = useState<ProductClassification | null>(null);
  const [formData, setFormData] = useState<ClassificationFormData>(emptyForm);
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: '',
    severity: 'success' as 'success' | 'error',
  });

  useEffect(() => {
    void loadClassifications();
  }, []);

  const loadClassifications = async () => {
    try {
      setLoading(true);
      const res = await inventoryApi.listClassifications();
      setClassifications(res.data);
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (classification?: ProductClassification) => {
    if (classification) {
      setEditingClassification(classification);
      setFormData({ description: classification.description });
    } else {
      setEditingClassification(null);
      setFormData(emptyForm);
    }
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setEditingClassification(null);
  };

  const handleSave = async () => {
    try {
      if (editingClassification) {
        await inventoryApi.updateClassification({ ...formData, id: editingClassification.id });
      } else {
        await inventoryApi.createClassification(formData);
      }
      setSnackbar({ open: true, message: 'Clasificación guardada correctamente', severity: 'success' });
      handleCloseDialog();
      void loadClassifications();
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    }
  };

  const handleOpenConfirmDialog = (classification: ProductClassification) => {
    setDeletingClassification(classification);
    setConfirmDialogOpen(true);
  };

  const handleCloseConfirmDialog = () => {
    setConfirmDialogOpen(false);
    setDeletingClassification(null);
  };

  const handleConfirmDelete = async () => {
    if (!deletingClassification) return;
    try {
      await inventoryApi.deleteClassification(deletingClassification.id);
      setSnackbar({ open: true, message: 'Clasificación eliminada correctamente', severity: 'success' });
      handleCloseConfirmDialog();
      void loadClassifications();
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Clasificaciones de Productos</Typography>
        <Button variant="contained" color="primary" startIcon={<AddIcon />} onClick={() => handleOpenDialog()}>
          Nueva Clasificación
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Descripción</TableCell>
              <TableCell align="right">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {classifications.length === 0 ? (
              <TableRow>
                <TableCell colSpan={2} align="center" sx={{ py: 4 }}>
                  <Typography color="text.secondary">No se encontraron clasificaciones.</Typography>
                </TableCell>
              </TableRow>
            ) : (
              classifications.map((c) => (
                <TableRow key={c.id} hover>
                  <TableCell>{c.description}</TableCell>
                  <TableCell align="right">
                    <Tooltip title="Editar">
                      <IconButton color="primary" size="small" onClick={() => handleOpenDialog(c)}>
                        <EditIcon />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Eliminar">
                      <IconButton color="error" size="small" onClick={() => handleOpenConfirmDialog(c)}>
                        <DeleteIcon />
                      </IconButton>
                    </Tooltip>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Create / Edit Dialog */}
      <Dialog open={dialogOpen} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
        <DialogTitle>{editingClassification ? 'Editar Clasificación' : 'Nueva Clasificación'}</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            label="Descripción"
            value={formData.description}
            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
            margin="normal"
            required
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancelar</Button>
          <Button onClick={() => void handleSave()} variant="contained" color="primary">
            Guardar
          </Button>
        </DialogActions>
      </Dialog>

      {/* Delete Confirm Dialog */}
      <Dialog open={confirmDialogOpen} onClose={handleCloseConfirmDialog}>
        <DialogTitle>Confirmar Eliminación</DialogTitle>
        <DialogContent>
          <Typography>
            {`¿Eliminar clasificación '${deletingClassification?.description}'?`}
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseConfirmDialog}>Cancelar</Button>
          <Button onClick={() => void handleConfirmDelete()} variant="contained" color="error">
            Eliminar
          </Button>
        </DialogActions>
      </Dialog>

      <Snackbar
        open={snackbar.open}
        autoHideDuration={6000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
      >
        <Alert severity={snackbar.severity} onClose={() => setSnackbar({ ...snackbar, open: false })}>
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
};
