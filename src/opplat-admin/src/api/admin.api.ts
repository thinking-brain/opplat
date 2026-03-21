import { adminAxiosClient, withTenantConfig } from './axiosClient';
import type {
  AdminCreateUserRequest,
  AdminSetUserActiveRequest,
  AdminSetUserRolesRequest,
  AdminTenant,
  AdminUpdateUserRequest,
  UpsertTenantRequest,
  User,
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

  listUsers: async (tenantIdentifier?: string): Promise<User[]> => {
    const response = await adminAxiosClient.get<User[]>('/admin/users', {
      params: tenantIdentifier ? { tenantIdentifier } : undefined,
    });
    return response.data;
  },

  listTenantUsers: async (tenantIdentifier: string): Promise<User[]> => {
    const response = await adminAxiosClient.get<User[]>(
      `/admin/tenants/${tenantIdentifier}/users`,
      withTenantConfig(tenantIdentifier),
    );
    return response.data;
  },

  createTenantUser: async (tenantIdentifier: string, request: AdminCreateUserRequest): Promise<User> => {
    const response = await adminAxiosClient.post<User>(
      `/admin/tenants/${tenantIdentifier}/users`,
      request,
      withTenantConfig(tenantIdentifier),
    );
    return response.data;
  },

  updateTenantUser: async (tenantIdentifier: string, userId: string, request: AdminUpdateUserRequest): Promise<void> => {
    await adminAxiosClient.put(
      `/admin/tenants/${tenantIdentifier}/users/${userId}`,
      request,
      withTenantConfig(tenantIdentifier),
    );
  },

  setTenantUserRoles: async (
    tenantIdentifier: string,
    userId: string,
    request: AdminSetUserRolesRequest,
  ): Promise<void> => {
    await adminAxiosClient.put(
      `/admin/tenants/${tenantIdentifier}/users/${userId}/roles`,
      request,
      withTenantConfig(tenantIdentifier),
    );
  },

  setTenantUserStatus: async (
    tenantIdentifier: string,
    userId: string,
    request: AdminSetUserActiveRequest,
  ): Promise<void> => {
    await adminAxiosClient.put(
      `/admin/tenants/${tenantIdentifier}/users/${userId}/status`,
      request,
      withTenantConfig(tenantIdentifier),
    );
  },
};
