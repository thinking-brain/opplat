import { salesAxiosClient } from './axiosClient';
import { Sale } from '../types';

export const salesApi = {
  list: async (): Promise<Sale[]> => {
    const response = await salesAxiosClient.get<Sale[]>('/Sales');
    return response.data;
  },

  create: async (sale: Sale): Promise<Sale> => {
    const response = await salesAxiosClient.post<Sale>('/Sales', sale);
    return response.data;
  },
};
