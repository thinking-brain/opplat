import React, { useState, useEffect, useRef } from 'react';
import {
  Box,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
  Alert,
  Snackbar,
  Tooltip,
  Avatar,
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Image as ImageIcon,
  History as HistoryIcon,
  ToggleOff,
  ToggleOn,
} from '@mui/icons-material';
import { productsApi } from '../api/products.api';
import { ProductForSale } from '../types';
import { LoadingSpinner } from '../components/LoadingSpinner';

export const ProductsPage: React.FC = () => {
  const [products, setProducts] = useState<ProductForSale[]>([]);
  const [filteredProducts, setFilteredProducts] = useState<ProductForSale[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [confirmDialogOpen, setConfirmDialogOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState<ProductForSale | null>(null);
  const [deletingProductId, setDeletingProductId] = useState<string | null>(null);
  const [formData, setFormData] = useState<ProductForSale>({
    name: '',
    price: 0,
    stock: 0,
    description: '',
    active: true,
  });
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' as 'success' | 'error' });
  const fileInputRef = useRef<HTMLInputElement>(null);
  const [uploadingProductId, setUploadingProductId] = useState<string | null>(null);

  useEffect(() => {
    loadProducts();
  }, []);

  useEffect(() => {
    const filtered = products.filter((product) =>
      product.name.toLowerCase().includes(searchTerm.toLowerCase())
    );
    setFilteredProducts(filtered);
  }, [searchTerm, products]);

  const loadProducts = async () => {
    try {
      setLoading(true);
      const data = await productsApi.list();
      setProducts(data);
      setFilteredProducts(data);
    } catch (error) {
      setSnackbar({ open: true, message: 'Error al cargar productos', severity: 'error' });
    } finally {
      setLoading(false);
    }
  };

  const handleOpenDialog = (product?: ProductForSale) => {
    if (product) {
      setEditingProduct(product);
      setFormData(product);
    } else {
      setEditingProduct(null);
      setFormData({ name: '', price: 0, stock: 0, description: '', active: true });
    }
    setDialogOpen(true);
  };

  const handleCloseDialog = () => {
    setDialogOpen(false);
    setEditingProduct(null);
  };

  const handleSave = async () => {
    try {
      if (editingProduct) {
        await productsApi.update({ ...formData, id: editingProduct.id });
        setSnackbar({ open: true, message: 'Producto actualizado exitosamente', severity: 'success' });
      } else {
        await productsApi.create(formData);
        setSnackbar({ open: true, message: 'Producto creado exitosamente', severity: 'success' });
      }
      handleCloseDialog();
      loadProducts();
    } catch (error) {
      setSnackbar({ open: true, message: 'Error al guardar producto', severity: 'error' });
    }
  };

  const handleOpenConfirmDialog = (productId: string) => {
    setDeletingProductId(productId);
    setConfirmDialogOpen(true);
  };

  const handleCloseConfirmDialog = () => {
    setConfirmDialogOpen(false);
    setDeletingProductId(null);
  };

  const handleConfirmDelete = async () => {
    if (!deletingProductId) return;

    try {
      await productsApi.delete(deletingProductId);
      setSnackbar({ open: true, message: 'Producto eliminado exitosamente', severity: 'success' });
      handleCloseConfirmDialog();
      loadProducts();
    } catch (error) {
      setSnackbar({ open: true, message: 'Error al eliminar producto', severity: 'error' });
    }
  };

  const handleToggleActive = async (product: ProductForSale) => {
    try {
      await productsApi.toggleActive(product.id!, !product.active);
      setSnackbar({
        open: true,
        message: product.active ? 'Producto deshabilitado' : 'Producto habilitado',
        severity: 'success',
      });
      loadProducts();
    } catch (error) {
      setSnackbar({ open: true, message: 'Error al cambiar estado del producto', severity: 'error' });
    }
  };

  const handleImageUploadClick = (productId: string) => {
    setUploadingProductId(productId);
    fileInputRef.current?.click();
  };

  const handleImageUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file || !uploadingProductId) return;

    try {
      await productsApi.uploadImage(uploadingProductId, file);
      setSnackbar({ open: true, message: 'Imagen subida exitosamente', severity: 'success' });
      loadProducts();
    } catch (error) {
      setSnackbar({ open: true, message: 'Error al subir imagen', severity: 'error' });
    } finally {
      setUploadingProductId(null);
      if (fileInputRef.current) {
        fileInputRef.current.value = '';
      }
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <Box>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Productos</Typography>
        <Button
          variant="contained"
          color="primary"
          startIcon={<AddIcon />}
          onClick={() => handleOpenDialog()}
        >
          Agregar Producto
        </Button>
      </Box>

      <Box mb={2}>
        <TextField
          fullWidth
          label="Buscar productos"
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
              <TableCell>Imagen</TableCell>
              <TableCell>Descripción</TableCell>
              <TableCell>Precio</TableCell>
              <TableCell>Stock</TableCell>
              <TableCell align="right">Acciones</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {filteredProducts.map((product) => (
              <TableRow key={product.id}>
                <TableCell>
                  <Avatar
                    src={product.imageUrl ? `/api/uploads/${product.imageUrl}` : undefined}
                    variant="rounded"
                    sx={{ width: 60, height: 60 }}
                  >
                    {!product.imageUrl && <ImageIcon />}
                  </Avatar>
                </TableCell>
                <TableCell>
                  <Typography variant="body1">{product.name}</Typography>
                  <Typography variant="caption" color="textSecondary">
                    {product.description || '-'}
                  </Typography>
                </TableCell>
                <TableCell>${product.price.toFixed(2)}</TableCell>
                <TableCell>{product.stock || 0}</TableCell>
                <TableCell align="right">
                  <Tooltip title={product.active ? 'Editar' : 'Producto deshabilitado'}>
                    <span>
                      <IconButton
                        color="primary"
                        onClick={() => handleOpenDialog(product)}
                        size="small"
                        disabled={!product.active}
                      >
                        <EditIcon />
                      </IconButton>
                    </span>
                  </Tooltip>
                  <Tooltip title={product.active ? 'Deshabilitar' : 'Habilitar'}>
                    <IconButton
                      color={product.active ? 'warning' : 'success'}
                      onClick={() => handleToggleActive(product)}
                      size="small"
                    >
                      {product.active ? <ToggleOn /> : <ToggleOff />}
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Subir imagen">
                    <IconButton
                      color="info"
                      onClick={() => handleImageUploadClick(product.id!)}
                      size="small"
                    >
                      <ImageIcon />
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Historial">
                    <IconButton color="default" size="small">
                      <HistoryIcon />
                    </IconButton>
                  </Tooltip>
                  <Tooltip title="Eliminar">
                    <IconButton
                      color="error"
                      onClick={() => handleOpenConfirmDialog(product.id!)}
                      size="small"
                    >
                      <DeleteIcon />
                    </IconButton>
                  </Tooltip>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      <input
        type="file"
        ref={fileInputRef}
        style={{ display: 'none' }}
        accept="image/*"
        onChange={handleImageUpload}
      />

      <Dialog open={dialogOpen} onClose={handleCloseDialog} maxWidth="sm" fullWidth>
        <DialogTitle>{editingProduct ? 'Editar Producto' : 'Agregar Producto'}</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            label="Nombre"
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            margin="normal"
            required
          />
          <TextField
            fullWidth
            label="Precio"
            type="number"
            value={formData.price}
            onChange={(e) => setFormData({ ...formData, price: parseFloat(e.target.value) })}
            margin="normal"
            required
          />
          <TextField
            fullWidth
            label="Stock"
            type="number"
            value={formData.stock || 0}
            onChange={(e) => setFormData({ ...formData, stock: parseInt(e.target.value) })}
            margin="normal"
          />
          <TextField
            fullWidth
            label="Descripción"
            value={formData.description || ''}
            onChange={(e) => setFormData({ ...formData, description: e.target.value })}
            margin="normal"
            multiline
            rows={3}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseDialog}>Cancelar</Button>
          <Button onClick={handleSave} variant="contained" color="primary">
            Guardar
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={confirmDialogOpen} onClose={handleCloseConfirmDialog}>
        <DialogTitle>Confirmar Eliminación</DialogTitle>
        <DialogContent>
          <Typography>¿Está seguro de que desea eliminar este producto?</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseConfirmDialog}>Cancelar</Button>
          <Button onClick={handleConfirmDelete} variant="contained" color="error">
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
