import { adminAxiosClient } from './axiosClient';
import type {
  AdminTenant,
  UpsertTenantRequest,
} from '../types';

export const adminApi = {
  listTenants: async (): Promise<AdminTenant[]> => {
    const response = await adminAxiosClient.get<AdminTenant[]>('/admin/tenants');
    return response.data;
  },

  createTenant: async (request: UpsertTenantRequest): Promise<AdminTenant> => {
    const response = await adminAxiosClient.post<AdminTenant>('/admin/tenants', request);
    return response.data;
  },

  updateTenant: async (identifier: string, request: UpsertTenantRequest): Promise<AdminTenant> => {
    const response = await adminAxiosClient.put<AdminTenant>(`/admin/tenants/${identifier}`, request);
    return response.data;
  },

  deactivateTenant: async (identifier: string): Promise<void> => {
    await adminAxiosClient.delete(`/admin/tenants/${identifier}`);
  },
};
