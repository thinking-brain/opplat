import { authAxiosClient } from './axiosClient';
import type { RedsysCardSetupRedirect, TenantPaymentMethodDto } from '../types';

export const paymentMethodsApi = {
  list: async (): Promise<TenantPaymentMethodDto[]> => {
    const response = await authAxiosClient.get<TenantPaymentMethodDto[]>('/billing/payment-methods');
    return response.data;
  },

  initiateCardSetup: async (): Promise<RedsysCardSetupRedirect> => {
    const response = await authAxiosClient.post<RedsysCardSetupRedirect>('/billing/payment-methods');
    return response.data;
  },

  remove: async (id: string): Promise<void> => {
    await authAxiosClient.delete(`/billing/payment-methods/${encodeURIComponent(id)}`);
  },

  setDefault: async (id: string): Promise<void> => {
    await authAxiosClient.put(`/billing/payment-methods/${encodeURIComponent(id)}/default`);
  },
};

