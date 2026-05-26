import React, { useState, useEffect } from 'react';
import {
  Alert,
  Box,
  Button,
  Chip,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  MenuItem,
  Paper,
  Snackbar,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tabs,
  Tab,
  TextField,
  Typography,
} from '@mui/material';
import { Add as AddIcon } from '@mui/icons-material';
import { inventoryApi } from '../api/inventory.api';
import type { CreateMovementData } from '../api/inventory.api';
import type { InventoryProduct, ProductMovement, Warehouse } from '../types';

interface TabPanelProps {
  children: React.ReactNode;
  index: number;
  value: number;
}

const TabPanel: React.FC<TabPanelProps> = ({ children, value, index }) => (
  <div role="tabpanel" hidden={value !== index} aria-labelledby={`inventory-tab-${index}`}>
    {value === index && <Box sx={{ pt: 2 }}>{children}</Box>}
  </div>
);

const movementChipColor = (
  type: string,
): 'success' | 'error' | 'warning' | 'default' => {
  switch (type.toLowerCase()) {
    case 'in':
    case 'entrada':
      return 'success';
    case 'out':
    case 'salida':
      return 'error';
    case 'adjustment':
    case 'ajuste':
      return 'warning';
    default:
      return 'default';
  }
};

export const InventoryPage: React.FC = () => {
  const [tab, setTab] = useState(0);
  const [products, setProducts] = useState<InventoryProduct[]>([]);
  const [movements, setMovements] = useState<ProductMovement[]>([]);
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [movementDialogOpen, setMovementDialogOpen] = useState(false);
  const [movementForm, setMovementForm] = useState<CreateMovementData>({
    productId: 0,
    storageId: 0,
    quantity: 1,
    type: '',
    observations: '',
  });
  const [snackbar, setSnackbar] = useState({
    open: false,
    message: '',
    severity: 'success' as 'success' | 'error',
  });

  const MOVEMENT_TYPES = [
    { value: 'in', label: 'Entrada' },
    { value: 'out', label: 'Salida' },
    { value: 'adjustment', label: 'Ajuste' },
  ];

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);
        const [prodRes, movRes, whRes] = await Promise.all([
          inventoryApi.getProducts(),
          inventoryApi.getMovements(),
          inventoryApi.listWarehouses(),
        ]);
        setProducts(prodRes.data);
        setMovements(movRes.data);
        setWarehouses(whRes.data);
      } catch {
        setError('Failed to load inventory data.');
      } finally {
        setLoading(false);
      }
    };
    void loadData();
  }, []);

  const loadMovements = async () => {
    try {
      const res = await inventoryApi.getMovements();
      setMovements(res.data);
    } catch {
      setSnackbar({ open: true, message: 'Error al cargar movimientos', severity: 'error' });
    }
  };

  const handleOpenMovementDialog = () => {
    setMovementForm({ productId: 0, storageId: 0, quantity: 1, type: '', observations: '' });
    setMovementDialogOpen(true);
  };

  const handleSaveMovement = async () => {
    try {
      await inventoryApi.createMovement(movementForm);
      setSnackbar({ open: true, message: 'Movimiento registrado correctamente', severity: 'success' });
      setMovementDialogOpen(false);
      void loadMovements();
    } catch {
      setSnackbar({ open: true, message: 'Error al conectar con el servidor', severity: 'error' });
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" pt={8}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Inventory
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
        <Tabs
          value={tab}
          onChange={(_event: React.SyntheticEvent, newValue: number) => setTab(newValue)}
          aria-label="inventory tabs"
        >
          <Tab label="Productos" id="inventory-tab-0" aria-controls="tabpanel-0" />
          <Tab label="Movimientos" id="inventory-tab-1" aria-controls="tabpanel-1" />
        </Tabs>
      </Box>

      {/* Products Tab */}
      <TabPanel value={tab} index={0}>
        <TableContainer component={Paper}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>Group</TableCell>
                <TableCell>Unit</TableCell>
                <TableCell align="right">Total Stock</TableCell>
                <TableCell>Status</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {products.map((p) => (
                <TableRow key={p.id} hover>
                  <TableCell>{p.nombre}</TableCell>
                  <TableCell>{p.grupo.descripcion}</TableCell>
                  <TableCell>{p.unidadDeMedida.siglas}</TableCell>
                  <TableCell align="right">{p.existenciaTotal ?? 0}</TableCell>
                  <TableCell>
                    <Chip
                      label={p.active ? 'Active' : 'Inactive'}
                      color={p.active ? 'success' : 'default'}
                      size="small"
                    />
                  </TableCell>
                </TableRow>
              ))}
              {products.length === 0 && (
                <TableRow>
                  <TableCell colSpan={5} align="center" sx={{ py: 4, color: 'text.secondary' }}>
                    No products found.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </TabPanel>

      {/* Movements Tab */}
      <TabPanel value={tab} index={1}>
        <Box display="flex" justifyContent="flex-end" mb={2}>
          <Button variant="contained" color="primary" startIcon={<AddIcon />} onClick={handleOpenMovementDialog}>
            Nuevo Movimiento
          </Button>
        </Box>
        <TableContainer component={Paper}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Fecha</TableCell>
                <TableCell>Producto ID</TableCell>
                <TableCell>Almacén ID</TableCell>
                <TableCell align="right">Cantidad</TableCell>
                <TableCell>Tipo</TableCell>
                <TableCell>Observaciones</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {movements.map((m) => (
                <TableRow key={m.id} hover>
                  <TableCell>{new Date(m.date).toLocaleDateString()}</TableCell>
                  <TableCell>{m.productId}</TableCell>
                  <TableCell>{m.storageId}</TableCell>
                  <TableCell align="right">{m.quantity}</TableCell>
                  <TableCell>
                    <Chip
                      label={m.type}
                      color={movementChipColor(m.type)}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>{m.observations || '—'}</TableCell>
                </TableRow>
              ))}
              {movements.length === 0 && (
                <TableRow>
                  <TableCell colSpan={6} align="center" sx={{ py: 4, color: 'text.secondary' }}>
                    No movements found.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </TableContainer>
      </TabPanel>

      {/* New Movement Dialog */}
      <Dialog open={movementDialogOpen} onClose={() => setMovementDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Nuevo Movimiento</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            select
            label="Almacén"
            value={movementForm.storageId || ''}
            onChange={(e) => setMovementForm({ ...movementForm, storageId: Number(e.target.value) })}
            margin="normal"
            required
          >
            {warehouses.map((w) => (
              <MenuItem key={w.id} value={w.id}>
                {w.description}
              </MenuItem>
            ))}
          </TextField>
          <TextField
            fullWidth
            select
            label="Producto"
            value={movementForm.productId || ''}
            onChange={(e) => setMovementForm({ ...movementForm, productId: Number(e.target.value) })}
            margin="normal"
            required
          >
            {products.map((p) => (
              <MenuItem key={p.id} value={p.id}>
                {p.nombre}
              </MenuItem>
            ))}
          </TextField>
          <TextField
            fullWidth
            select
            label="Tipo"
            value={movementForm.type}
            onChange={(e) => setMovementForm({ ...movementForm, type: e.target.value })}
            margin="normal"
            required
          >
            {MOVEMENT_TYPES.map((t) => (
              <MenuItem key={t.value} value={t.value}>
                {t.label}
              </MenuItem>
            ))}
          </TextField>
          <TextField
            fullWidth
            label="Cantidad"
            type="number"
            value={movementForm.quantity}
            onChange={(e) => setMovementForm({ ...movementForm, quantity: Math.max(1, parseInt(e.target.value) || 1) })}
            margin="normal"
            required
            inputProps={{ min: 1 }}
          />
          <TextField
            fullWidth
            label="Observaciones"
            value={movementForm.observations}
            onChange={(e) => setMovementForm({ ...movementForm, observations: e.target.value })}
            margin="normal"
            multiline
            rows={3}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setMovementDialogOpen(false)}>Cancelar</Button>
          <Button onClick={() => void handleSaveMovement()} variant="contained" color="primary">
            Guardar
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
