import React, { useState, useEffect } from 'react';
import {
  Box,
  Button,
  Card,
  CardContent,
  Grid,
  Typography,
  TextField,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  IconButton,
  Alert,
  Snackbar,
  Autocomplete,
} from '@mui/material';
import {
  Add as AddIcon,
  Remove as RemoveIcon,
  Delete as DeleteIcon,
  ShoppingCart as CartIcon,
} from '@mui/icons-material';
import { productsApi } from '../api/products.api';
import { salesApi } from '../api/sales.api';
import { ProductForSale, SaleItem } from '../types';
import { LoadingSpinner } from '../components/LoadingSpinner';

export const SellPage: React.FC = () => {
  const [products, setProducts] = useState<ProductForSale[]>([]);
  const [cart, setCart] = useState<SaleItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [saleDetails, setSaleDetails] = useState({
    dependiente: '',
    posicion: '',
    comanda: '',
    observaciones: '',
  });
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });

  const dependienteOptions = ['Juan Pérez', 'María García', 'Carlos López', 'Ana Martínez'];
  const posicionOptions = ['Mesa 1', 'Mesa 2', 'Mesa 3', 'Mesa 4', 'Mesa 5', 'Barra', 'Para Llevar'];

  useEffect(() => {
    loadProducts();
  }, []);

  const loadProducts = async () => {
    try {
      setLoading(true);
      const data = await productsApi.list();
      setProducts(data);
    } catch (error) {
      setSnackbar({ open: true, message: 'Error loading products', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const addToCart = (product: ProductForSale) => {
    const existingItem = cart.find((item) => item.productId === product.id);

    if (existingItem) {
      setCart(
        cart.map((item) =>
          item.productId === product.id
            ? {
                ...item,
                quantity: item.quantity + 1,
                subtotal: (item.quantity + 1) * item.price,
              }
            : item
        )
      );
    } else {
      setCart([
        ...cart,
        {
          productId: product.id!,
          productName: product.name,
          quantity: 1,
          price: product.price,
          subtotal: product.price,
        },
      ]);
    }
  };

  const updateQuantity = (productId: string, change: number) => {
    setCart(
      cart
        .map((item) => {
          if (item.productId === productId) {
            const newQuantity = item.quantity + change;
            if (newQuantity <= 0) return null;
            return {
              ...item,
              quantity: newQuantity,
              subtotal: newQuantity * item.price,
            };
          }
          return item;
        })
        .filter(Boolean) as SaleItem[]
    );
  };

  const removeFromCart = (productId: string) => {
    setCart(cart.filter((item) => item.productId !== productId));
  };

  const calculateTotal = () => {
    return cart.reduce((sum, item) => sum + item.subtotal, 0);
  };

  const handleCheckout = async () => {
    if (cart.length === 0) {
      setSnackbar({ open: true, message: 'El carrito está vacío', severity: 'error' });
      return;
    }

    try {
      setSubmitting(true);
      await salesApi.create({
        date: new Date().toISOString(),
        total: calculateTotal(),
        items: cart,
      });
      setSnackbar({ open: true, message: '¡Venta registrada exitosamente!', severity: 'success' });
      setCart([]);
      setSaleDetails({ dependiente: '', posicion: '', comanda: '', observaciones: '' });
    } catch (error) {
      setSnackbar({ open: true, message: 'Error al procesar la venta', severity: 'error' });
    } finally {
      setSubmitting(false);
    }
  };

  const filteredProducts = products.filter((product) =>
    product.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (loading) return <LoadingSpinner />;

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Punto de Venta
      </Typography>

      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Detalles de Venta
          </Typography>
          <Grid container spacing={2}>
            <Grid item xs={12} md={6}>
              <Autocomplete
                options={dependienteOptions}
                value={saleDetails.dependiente}
                onChange={(_, newValue) =>
                  setSaleDetails({ ...saleDetails, dependiente: newValue || '' })
                }
                renderInput={(params) => <TextField {...params} label="Dependiente" fullWidth />}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <Autocomplete
                options={posicionOptions}
                value={saleDetails.posicion}
                onChange={(_, newValue) =>
                  setSaleDetails({ ...saleDetails, posicion: newValue || '' })
                }
                renderInput={(params) => <TextField {...params} label="Posición" fullWidth />}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <TextField
                fullWidth
                label="Comanda"
                value={saleDetails.comanda}
                onChange={(e) => setSaleDetails({ ...saleDetails, comanda: e.target.value })}
              />
            </Grid>
            <Grid item xs={12} md={6}>
              <TextField
                fullWidth
                label="Observaciones"
                multiline
                rows={2}
                value={saleDetails.observaciones}
                onChange={(e) => setSaleDetails({ ...saleDetails, observaciones: e.target.value })}
              />
            </Grid>
          </Grid>
        </CardContent>
      </Card>

      <Grid container spacing={3}>
        <Grid item xs={12} md={7}>
          <Card>
            <CardContent>
              <TextField
                fullWidth
                label="Buscar Productos"
                variant="outlined"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                margin="normal"
              />

              <Grid container spacing={2} sx={{ mt: 1 }}>
                {filteredProducts.map((product) => (
                  <Grid item xs={12} sm={6} md={4} key={product.id}>
                    <Card
                      sx={{
                        cursor: 'pointer',
                        '&:hover': { boxShadow: 6 },
                      }}
                      onClick={() => addToCart(product)}
                    >
                      <CardContent>
                        <Typography variant="h6" gutterBottom>
                          {product.name}
                        </Typography>
                        <Typography variant="body1" color="primary">
                          ${product.price.toFixed(2)}
                        </Typography>
                        <Typography variant="caption" color="textSecondary">
                          Stock: {product.stock || 0}
                        </Typography>
                      </CardContent>
                    </Card>
                  </Grid>
                ))}
              </Grid>
            </CardContent>
          </Card>
        </Grid>

        <Grid item xs={12} md={5}>
          <Card>
            <CardContent>
              <Box display="flex" alignItems="center" mb={2}>
                <CartIcon sx={{ mr: 1 }} />
                <Typography variant="h6">Carrito</Typography>
              </Box>

              {cart.length === 0 ? (
                <Typography color="textSecondary">El carrito está vacío</Typography>
              ) : (
                <>
                  <TableContainer>
                    <Table size="small">
                      <TableHead>
                        <TableRow>
                          <TableCell>Producto</TableCell>
                          <TableCell align="center">Cant.</TableCell>
                          <TableCell align="right">Precio</TableCell>
                          <TableCell align="right">Total</TableCell>
                          <TableCell></TableCell>
                        </TableRow>
                      </TableHead>
                      <TableBody>
                        {cart.map((item) => (
                          <TableRow key={item.productId}>
                            <TableCell>{item.productName}</TableCell>
                            <TableCell align="center">
                              <Box display="flex" alignItems="center" justifyContent="center">
                                <IconButton
                                  size="small"
                                  onClick={() => updateQuantity(item.productId, -1)}
                                >
                                  <RemoveIcon fontSize="small" />
                                </IconButton>
                                <Typography sx={{ mx: 1 }}>{item.quantity}</Typography>
                                <IconButton
                                  size="small"
                                  onClick={() => updateQuantity(item.productId, 1)}
                                >
                                  <AddIcon fontSize="small" />
                                </IconButton>
                              </Box>
                            </TableCell>
                            <TableCell align="right">${item.price.toFixed(2)}</TableCell>
                            <TableCell align="right">${item.subtotal.toFixed(2)}</TableCell>
                            <TableCell>
                              <IconButton
                                size="small"
                                color="error"
                                onClick={() => removeFromCart(item.productId)}
                              >
                                <DeleteIcon fontSize="small" />
                              </IconButton>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </TableContainer>

                  <Box sx={{ mt: 3, pt: 2, borderTop: 1, borderColor: 'divider' }}>
                    <Typography variant="h5" align="right" gutterBottom>
                      Importe Total: ${calculateTotal().toFixed(2)}
                    </Typography>
                    <Button
                      fullWidth
                      variant="contained"
                      color="primary"
                      size="large"
                      onClick={handleCheckout}
                      disabled={submitting}
                    >
                      {submitting ? 'Procesando...' : 'Registrar Venta'}
                    </Button>
                  </Box>
                </>
              )}
            </CardContent>
          </Card>
        </Grid>
      </Grid>

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
