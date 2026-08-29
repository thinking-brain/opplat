import { authAxiosClient } from './axiosClient';
import type { AddPaymentMethodRequest, TenantPaymentMethodDto } from '../types';

export const paymentMethodsApi = {
  list: async (): Promise<TenantPaymentMethodDto[]> => {
    const response = await authAxiosClient.get<TenantPaymentMethodDto[]>('/billing/payment-methods');
    return response.data;
  },

  add: async (request: AddPaymentMethodRequest): Promise<TenantPaymentMethodDto> => {
    const response = await authAxiosClient.post<TenantPaymentMethodDto>('/billing/payment-methods', request);
    return response.data;
  },

  remove: async (id: string): Promise<void> => {
    await authAxiosClient.delete(`/billing/payment-methods/${encodeURIComponent(id)}`);
  },

  setDefault: async (id: string): Promise<void> => {
    await authAxiosClient.put(`/billing/payment-methods/${encodeURIComponent(id)}/default`);
  },
};
