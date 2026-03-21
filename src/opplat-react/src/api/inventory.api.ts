import { inventoryAxiosClient } from './axiosClient';
import type { InventoryProduct, ProductMovement } from '../types';

export type CreateMovementData = Omit<ProductMovement, 'id'>;

export const inventoryApi = {
  getProducts: () => inventoryAxiosClient.get<InventoryProduct[]>('/inventory/products'),
  getMovements: () => inventoryAxiosClient.get<ProductMovement[]>('/inventory/movements'),
  createMovement: (data: CreateMovementData) =>
    inventoryAxiosClient.post<ProductMovement>('/inventory/movements', data),
};
