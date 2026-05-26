import { inventoryAxiosClient } from './axiosClient';
import type { InventoryProduct, ProductMovement, Warehouse, ProductClassification, ProductGroup, ResponseDto } from '../types';

export type CreateMovementData = Omit<ProductMovement, 'id' | 'date'>;

export const inventoryApi = {
  getProducts: () => inventoryAxiosClient.get<InventoryProduct[]>('/inventory/products'),
  getMovements: () => inventoryAxiosClient.get<ProductMovement[]>('/inventory/movements'),
  createMovement: (data: CreateMovementData) =>
    inventoryAxiosClient.post<ProductMovement>('/inventory/movements', data),

  // Warehouses
  listWarehouses: () => inventoryAxiosClient.get<Warehouse[]>('/inventory/storages'),
  createWarehouse: (data: Omit<Warehouse, 'id'>) =>
    inventoryAxiosClient.post<ResponseDto>('/inventory/storages', data),
  updateWarehouse: (data: Warehouse) =>
    inventoryAxiosClient.put<ResponseDto>('/inventory/storages', data),
  deleteWarehouse: (id: string) =>
    inventoryAxiosClient.delete<ResponseDto>(`/inventory/storages/${id}`),

  // Product Classifications
  listClassifications: () =>
    inventoryAxiosClient.get<ProductClassification[]>('/inventory/productclassifications'),
  createClassification: (data: Omit<ProductClassification, 'id'>) =>
    inventoryAxiosClient.post<ResponseDto>('/inventory/productclassifications', data),
  updateClassification: (data: ProductClassification) =>
    inventoryAxiosClient.put<ResponseDto>('/inventory/productclassifications', data),
  deleteClassification: (id: number) =>
    inventoryAxiosClient.delete<ResponseDto>(`/inventory/productclassifications/${id}`),

  // Product Groups
  listProductGroups: () =>
    inventoryAxiosClient.get<ProductGroup[]>('/inventory/productgroups'),
  createProductGroup: (data: Omit<ProductGroup, 'id' | 'classification'>) =>
    inventoryAxiosClient.post<ResponseDto>('/inventory/productgroups', data),
  updateProductGroup: (data: Omit<ProductGroup, 'classification'>) =>
    inventoryAxiosClient.put<ResponseDto>('/inventory/productgroups', data),
  deleteProductGroup: (id: string) =>
    inventoryAxiosClient.delete<ResponseDto>(`/inventory/productgroups/${id}`),
};
