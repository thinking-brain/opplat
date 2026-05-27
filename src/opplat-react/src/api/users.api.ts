import { authAxiosClient } from './axiosClient';
import { User, RegisterUser, ResponseDto } from '../types';

export const usersApi = {
  list: async (): Promise<User[]> => {
    const response = await authAxiosClient.get<User[]>('/auth/Account/user-list');
    return response.data;
  },

  create: async (user: RegisterUser): Promise<User> => {
    const response = await authAxiosClient.post<User>('/auth/Account/add-user', user);
    return response.data;
  },

  edit: async (userId: string, name: string, lastName: string): Promise<void> => {
    await authAxiosClient.post('/auth/Account/edit-user', {
      id: userId,
      name,
      lastName,
    });
  },

  changeRoles: async (userId: string, roles: string[]): Promise<ResponseDto> => {
    const response = await authAxiosClient.post<ResponseDto>('/auth/Account/change-roles', {
      userId: userId,
      roles: roles,
    });
    return response.data;
  },

  toggleActive: async (userId: string): Promise<void> => {
    await authAxiosClient.get(`/auth/account/toggle-user-status?userId=${userId}`);
  },

  delete: async (userId: string): Promise<void> => {
    await authAxiosClient.delete(`/auth/account/delete-user?userId=${userId}`);
  },
};
