import axiosClient from './axiosClient';
import type { InventoryProduct, ProductMovement } from '../types';

export type CreateMovementData = Omit<ProductMovement, 'id'>;

export const inventoryApi = {
  getProducts: () => axiosClient.get<InventoryProduct[]>('/inventory/products'),
  getMovements: () => axiosClient.get<ProductMovement[]>('/inventory/movements'),
  createMovement: (data: CreateMovementData) =>
    axiosClient.post<ProductMovement>('/inventory/movements', data),
};
