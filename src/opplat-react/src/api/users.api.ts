import axiosClient from './axiosClient';
import { User, RegisterUser, ResponseDto } from '../types';

export const usersApi = {
  list: async (): Promise<User[]> => {
    const response = await axiosClient.get<User[]>('/auth/Account/user-list');
    return response.data;
  },

  create: async (user: RegisterUser): Promise<User> => {
    const response = await axiosClient.post<User>('/auth/Account/add-user', user);
    return response.data;
  },

  edit: async (userId: string, name: string, lastName: string): Promise<void> => {
    await axiosClient.post('/auth/Account/edit-user', {
      id: userId,
      name,
      lastName,
    });
  },

  changeRoles: async (userId: string, roles: string[]): Promise<ResponseDto> => {
    const response = await axiosClient.post<ResponseDto>('/auth/Account/cambiar-roles', {
      idUsuario: userId,
      Roles: roles,
    });
    return response.data;
  },

  toggleActive: async (userId: string): Promise<void> => {
    await axiosClient.get(`/auth/Account/cambiar-estado?idUsuario=${userId}`);
  },

  delete: async (userId: string): Promise<void> => {
    await axiosClient.delete(`/auth/Account/delete-user?idUsuario=${userId}`);
  },
};
