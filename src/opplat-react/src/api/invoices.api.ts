import { salesAxiosClient } from './axiosClient';
import { Invoice, TenantFiscalSettings, ResponseDto } from '../types';

export const invoicesApi = {
  list: async (): Promise<Invoice[]> => {
    const response = await salesAxiosClient.get<Invoice[]>('/invoices');
    return response.data;
  },

  get: async (id: string): Promise<Invoice> => {
    const response = await salesAxiosClient.get<Invoice>(`/invoices/${id}`);
    return response.data;
  },

  create: async (invoice: Omit<Invoice, 'id' | 'number' | 'fullNumber' | 'fiscalRecord'>): Promise<ResponseDto> => {
    const response = await salesAxiosClient.post<ResponseDto>('/invoices', invoice);
    return response.data;
  },

  issue: async (id: string): Promise<ResponseDto> => {
    const response = await salesAxiosClient.post<ResponseDto>(`/invoices/${id}/issue`);
    return response.data;
  },

  cancel: async (id: string): Promise<ResponseDto> => {
    const response = await salesAxiosClient.post<ResponseDto>(`/invoices/${id}/cancel`);
    return response.data;
  },

  getSettings: async (): Promise<TenantFiscalSettings> => {
    const response = await salesAxiosClient.get<TenantFiscalSettings>('/invoicing/settings');
    return response.data;
  },

  saveSettings: async (settings: TenantFiscalSettings): Promise<ResponseDto> => {
    const response = await salesAxiosClient.put<ResponseDto>('/invoicing/settings', settings);
    return response.data;
  },
};
