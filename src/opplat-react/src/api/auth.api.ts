import axiosClient from './axiosClient';
import { LoginRequest, LoginResponse, User } from '../types';

export const authApi = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    const response = await axiosClient.post<LoginResponse>('/auth/Account/Login', credentials);
    return response.data;
  },

  getCurrentUser: async (username: string): Promise<User> => {
    const response = await axiosClient.get<User>(`/auth/Account/profile/${username}`);
    return response.data;
  },

  logout: () => {
    localStorage.removeItem('opplat_token');
    localStorage.removeItem('opplat_user');
  },
};
