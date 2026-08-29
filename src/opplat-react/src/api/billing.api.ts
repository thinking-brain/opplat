import { authAxiosClient } from './axiosClient';
import type { ChangeSubscriptionRequest, SubscriptionPaymentHistoryItem, SubscriptionPlan, TenantSubscriptionDetails } from '../types';

export const billingApi = {
  cancel: async (): Promise<void> => {
    await authAxiosClient.post('/billing/cancel');
  },

  createPortalSession: async (returnUrl = window.location.href): Promise<{ url?: string }> => {
    const response = await authAxiosClient.post<{ url?: string }>('/billing/portal-session', { returnUrl });
    return response.data;
  },

  getSubscription: async (): Promise<TenantSubscriptionDetails> => {
    const response = await authAxiosClient.get<TenantSubscriptionDetails>('/billing/subscription');
    return response.data;
  },

  changeSubscription: async (payload: ChangeSubscriptionRequest): Promise<TenantSubscriptionDetails> => {
    const response = await authAxiosClient.post<TenantSubscriptionDetails>('/billing/subscription', payload);
    return response.data;
  },

  listPlans: async (): Promise<SubscriptionPlan[]> => {
    const response = await authAxiosClient.get<SubscriptionPlan[]>('/admin/subscription-plans');
    return response.data;
  },

  history: async (): Promise<SubscriptionPaymentHistoryItem[]> => {
    const response = await authAxiosClient.get<SubscriptionPaymentHistoryItem[]>('/billing/history');
    return response.data;
  },

  downloadInvoice: async (invoiceId: string): Promise<Blob> => {
    const response = await authAxiosClient.get(`/billing/invoices/${encodeURIComponent(invoiceId)}/pdf`, {
      responseType: 'blob',
    });
    return response.data;
  },
};