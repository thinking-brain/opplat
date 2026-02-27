import React, { useState, useEffect } from 'react';
import {
  Alert,
  Box,
  Chip,
  CircularProgress,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Tabs,
  Tab,
  Typography,
} from '@mui/material';
import { inventoryApi } from '../api/inventory.api';
import type { InventoryProduct, ProductMovement } from '../types';

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
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const loadData = async () => {
      try {
        setLoading(true);
        const [prodRes, movRes] = await Promise.all([
          inventoryApi.getProducts(),
          inventoryApi.getMovements(),
        ]);
        setProducts(prodRes.data);
        setMovements(movRes.data);
      } catch {
        setError('Failed to load inventory data.');
      } finally {
        setLoading(false);
      }
    };
    void loadData();
  }, []);

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
        <TableContainer component={Paper}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Date</TableCell>
                <TableCell>Product ID</TableCell>
                <TableCell>Storage ID</TableCell>
                <TableCell align="right">Quantity</TableCell>
                <TableCell>Type</TableCell>
                <TableCell>Observations</TableCell>
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
    </Box>
  );
};
