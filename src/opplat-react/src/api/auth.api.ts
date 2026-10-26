import { adminPublicAxiosClient, authAxiosClient } from './axiosClient';
import type { RedsysCardSetupRedirect, RegisterUser, SubscriptionPlan, TenantAccessContext, TenantRegistrationRequest, TenantRegistrationResult, User } from '../types';

export const registerUser = async (data: RegisterUser): Promise<{ userId: string; message: string }> => {
  const response = await adminPublicAxiosClient.post<{ userId: string; message: string }>(
    '/auth/register',
    {
      username: data.username,
      email: data.email,
      firstName: data.name,
      lastName: data.lastName,
      password: data.password,
    }
  );
  return response.data;
};

export const authApi = {
  getCurrentUser: async (username: string): Promise<User> => {
    const response = await authAxiosClient.get<User>(`/auth/Account/profile/${username}`);
    return response.data;
  },

  getTenantContext: async (): Promise<TenantAccessContext> => {
    const response = await authAxiosClient.get<TenantAccessContext>('/auth/account/tenant-context');
    return response.data;
  },

  updateProfile: async (name: string, lastName: string): Promise<void> => {
    await authAxiosClient.post('/auth/account/profile', { name, lastName });
  },
};

export const getSubscriptionPlans = async (): Promise<SubscriptionPlan[]> => {
  const response = await adminPublicAxiosClient.get<SubscriptionPlan[]>('/admin/subscription-plans');
  return response.data;
};

export const registerTenant = async (data: TenantRegistrationRequest): Promise<TenantRegistrationResult> => {
  const response = await adminPublicAxiosClient.post<TenantRegistrationResult>('/public/register', data);
  return response.data;
};

export const initiateTenantCardSetup = async (tenantId: string): Promise<RedsysCardSetupRedirect> => {
  const response = await adminPublicAxiosClient.get<RedsysCardSetupRedirect>(`/public/tenants/${encodeURIComponent(tenantId)}/card-setup`);
  return response.data;
};
