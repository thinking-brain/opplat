import { authAxiosClient } from './axiosClient';
import type { User } from '../types';

export const authApi = {
  getCurrentUser: async (username: string): Promise<User> => {
    const response = await authAxiosClient.get<User>(`/auth/Account/profile/${username}`);
    return response.data;
  },
};
