import axiosClient from './axiosClient';
import { ProductForSale, ResponseDto } from '../types';

export const productsApi = {
  list: async (): Promise<ProductForSale[]> => {
    const response = await axiosClient.get<ProductForSale[]>('/sales/Products');
    return response.data;
  },

  create: async (product: ProductForSale): Promise<ResponseDto> => {
    const response = await axiosClient.post<ResponseDto>('/sales/Products', product);
    return response.data;
  },

  update: async (product: ProductForSale): Promise<ResponseDto> => {
    const response = await axiosClient.put<ResponseDto>('/sales/Products', product);
    return response.data;
  },

  delete: async (id: string): Promise<ResponseDto> => {
    const response = await axiosClient.delete<ResponseDto>(`/sales/Products?id=${id}`);
    return response.data;
  },

  toggleActive: async (id: string, active: boolean): Promise<ResponseDto> => {
    const response = await axiosClient.put<ResponseDto>('/sales/Products', { id, active });
    return response.data;
  },

  uploadImage: async (id: string, file: File): Promise<ResponseDto> => {
    const formData = new FormData();
    formData.append('id', id);
    formData.append('file', file);
    const response = await axiosClient.post<ResponseDto>('/sales/Products/upload-image', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },
};
