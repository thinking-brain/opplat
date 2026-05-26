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
  MenuItem,
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
import type { ProductGroup, ProductClassification } from '../types';
import { LoadingSpinner } from '../components/LoadingSpinner';

type ProductGroupFormData = Omit<ProductGroup, 'id' | 'classification'>;

const emptyForm: ProductGroupFormData = {
  description: '',
  classificationId: 0,
};

export const ProductGroupsPage: React.FC = () => {
  const [groups, setGroups] = useState<ProductGroup[]>([]);
  const [classifications, setClassifications] = useState<ProductClassification[]>([]);
  const [loading, setLoading] = useState(true);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [confirmDialogOpen, setConfirmDialogOpen] = useState(false);
  const [editingGroup, setEditingGroup] = useState<ProductGroup | null>(null);
  const [deletingGroup, setDeletingGroup] = useState<ProductGroup | null>(null);
  const [formData, setFormData] = useState<ProductGroupFormData>(emptyForm);
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: '',
    severity: 'success' as 'success' | 'error',
  });

  useEffect(() => {
    void loadData();
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      const [groupsRes, classificationsRes] = await Promise.all([
        inventoryApi.listProductGroups(),
        inventoryApi.listClassifications(),
      ]);
      setGroups(groupsRes.data);
      setClassifications(classificationsRes.data);
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const loadGroups = async () => {
    try {
      const res = await inventoryApi.listProductGroups();
      setGroups(res.data);
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    }
  };

  const handleOpenDialog = (group?: ProductGroup) => {
    if (group) {
      setEditingGroup(group);
      setFormData({ description: group.description, classificationId: group.classificationId });
    } else {
      setEditingGroup(null);
      setFormData(emptyForm);
    }
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setEditingGroup(null);
  };

  const handleSave = async () => {
    try {
      if (editingGroup) {
        await inventoryApi.updateProductGroup({ ...formData, id: editingGroup.id });
      } else {
        await inventoryApi.createProductGroup(formData);
      }
      setSnackbar({ open: true, message: 'Grupo guardado correctamente', severity: 'success' });
      handleCloseDialog();
      void loadGroups();
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    }
  };

  const handleOpenConfirmDialog = (group: ProductGroup) => {
    setDeletingGroup(group);
    setConfirmDialogOpen(true);
  };

  const handleCloseConfirmDialog = () => {
    setConfirmDialogOpen(false);
    setDeletingGroup(null);
  };

  const handleConfirmDelete = async () => {
    if (!deletingGroup) return;
    try {
      await inventoryApi.deleteProductGroup(deletingGroup.id);
      setSnackbar({ open: true, message: 'Grupo eliminado correctamente', severity: 'success' });
      handleCloseConfirmDialog();
      void loadGroups();
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Grupos de Productos</Typography>
        <Button variant="contained" color="primary" startIcon={<AddIcon />} onClick={() => handleOpenDialog()}>
          Nuevo Grupo
        </Button>
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Descripción</TableCell>
              <TableCell>Clasificación</TableCell>
              <TableCell align="right">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {groups.length === 0 ? (
              <TableRow>
                <TableCell colSpan={3} align="center" sx={{ py: 4 }}>
                  <Typography color="text.secondary">No se encontraron grupos.</Typography>
                </TableCell>
              </TableRow>
            ) : (
              groups.map((g) => (
                <TableRow key={g.id} hover>
                  <TableCell>{g.description}</TableCell>
                  <TableCell>{g.classification?.description ?? '—'}</TableCell>
                  <TableCell align="right">
                    <Tooltip title="Editar">
                      <IconButton color="primary" size="small" onClick={() => handleOpenDialog(g)}>
                        <EditIcon />
                      </IconButton>
                    </Tooltip>
                    <Tooltip title="Eliminar">
                      <IconButton color="error" size="small" onClick={() => handleOpenConfirmDialog(g)}>
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
        <DialogTitle>{editingGroup ? 'Editar Grupo' : 'Nuevo Grupo'}</DialogTitle>
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
            select
            label="Clasificación"
            value={formData.classificationId || ''}
            onChange={(e) => setFormData({ ...formData, classificationId: Number(e.target.value) })}
            margin="normal"
            required
          >
            {classifications.map((c) => (
              <MenuItem key={c.id} value={c.id}>
                {c.description}
              </MenuItem>
            ))}
          </TextField>
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
            {`¿Eliminar grupo '${deletingGroup?.description}'?`}
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
