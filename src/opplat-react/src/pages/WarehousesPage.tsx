import React, { useState, useEffect } from 'react';
import {
  Alert,
  Box,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControlLabel,
  IconButton,
  Paper,
  Snackbar,
  Switch,
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
import type { Warehouse } from '../types';
import { LoadingSpinner } from '../components/LoadingSpinner';

type WarehouseFormData = Omit<Warehouse, 'id'>;

const emptyForm: WarehouseFormData = {
  code: '',
  description: '',
  isCostCenter: true,
};

export const WarehousesPage: React.FC = () => {
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [filteredWarehouses, setFilteredWarehouses] = useState<Warehouse[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [confirmDialogOpen, setConfirmDialogOpen] = useState(false);
  const [editingWarehouse, setEditingWarehouse] = useState<Warehouse | null>(null);
  const [deletingWarehouse, setDeletingWarehouse] = useState<Warehouse | null>(null);
  const [formData, setFormData] = useState<WarehouseFormData>(emptyForm);
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: '',
    severity: 'success' as 'success' | 'error',
  });

  useEffect(() => {
    void loadWarehouses();
  }, []);

  useEffect(() => {
    const lower = searchTerm.toLowerCase();
    setFilteredWarehouses(
      warehouses.filter(
        (w) =>
          w.description.toLowerCase().includes(lower) ||
          w.code.toLowerCase().includes(lower),
      ),
    );
  }, [searchTerm, warehouses]);

  const loadWarehouses = async () => {
    try {
      setLoading(true);
      const res = await inventoryApi.listWarehouses();
      setWarehouses(res.data);
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (warehouse?: Warehouse) => {
    if (warehouse) {
      setEditingWarehouse(warehouse);
      setFormData({ code: warehouse.code, description: warehouse.description, isCostCenter: warehouse.isCostCenter });
    } else {
      setEditingWarehouse(null);
      setFormData(emptyForm);
    }
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setEditingWarehouse(null);
  };

  const handleSave = async () => {
    try {
      if (editingWarehouse) {
        await inventoryApi.updateWarehouse({ ...formData, id: editingWarehouse.id });
      } else {
        await inventoryApi.createWarehouse(formData);
      }
      setSnackbar({ open: true, message: 'Almacén guardado correctamente', severity: 'success' });
      handleCloseDialog();
      void loadWarehouses();
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    }
  };

  const handleOpenConfirmDialog = (warehouse: Warehouse) => {
    setDeletingWarehouse(warehouse);
    setConfirmDialogOpen(true);
  };

  const handleCloseConfirmDialog = () => {
    setConfirmDialogOpen(false);
    setDeletingWarehouse(null);
  };

  const handleConfirmDelete = async () => {
    if (!deletingWarehouse) return;
    try {
      await inventoryApi.deleteWarehouse(deletingWarehouse.id);
      setSnackbar({ open: true, message: 'Almacén eliminado correctamente', severity: 'success' });
      handleCloseConfirmDialog();
      void loadWarehouses();
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Almacenes</Typography>
        <Button variant="contained" color="primary" startIcon={<AddIcon />} onClick={() => handleOpenDialog()}>
          Nuevo Almacén
        </Button>
      </Box>

      <Box mb={2}>
        <TextField
          fullWidth
          label="Buscar almacenes"
          variant="outlined"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          size="small"
        />
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Código</TableCell>
              <TableCell>Descripción</TableCell>
              <TableCell>Centro de Costo</TableCell>
              <TableCell align="right">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {filteredWarehouses.length === 0 ? (
              <TableRow>
                <TableCell colSpan={4} align="center" sx={{ py: 4 }}>
                  <Typography color="text.secondary">No se encontraron almacenes.</Typography>
                </TableCell>
              </TableRow>
            ) : (
              filteredWarehouses.map((w) => (
                <TableRow key={w.id} hover>
                  <TableCell>{w.code}</TableCell>
                  <TableCell>{w.description}</TableCell>
                  <TableCell>
                    <Chip
                      label={w.isCostCenter ? 'Sí' : 'No'}
                      color={w.isCostCenter ? 'success' : 'default'}
                      size="small"
                    />
                  </TableCell>
                  <TableCell align="right">
                    <Tooltip title="Editar">
                      <IconButton color="primary" size="small" onClick={() => handleOpenDialog(w)}>
                        <EditIcon />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Eliminar">
                      <IconButton color="error" size="small" onClick={() => handleOpenConfirmDialog(w)}>
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
        <DialogTitle>{editingWarehouse ? 'Editar Almacén' : 'Nuevo Almacén'}</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            label="Descripción"
            value={formData.description}
            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
            margin="normal"
            required
          />
          <TextField
            fullWidth
            label="Código"
            value={formData.code}
            onChange={(e) => setFormData({ ...formData, code: e.target.value })}
            margin="normal"
          />
          <FormControlLabel
            control={
              <Switch
                checked={formData.isCostCenter}
                onChange={(e) => setFormData({ ...formData, isCostCenter: e.target.checked })}
              />
            }
            label="Centro de Costo"
            sx={{ mt: 1 }}
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
            {`¿Eliminar almacén '${deletingWarehouse?.description}'?`}
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
