import { authAxiosClient } from './axiosClient';
import type { LicenseInfo } from '../types';

export const licenseApi = {
  get: () => authAxiosClient.get<LicenseInfo>('/admin/licencia'),
  upload: (file: File) => {
    const form = new FormData();
    form.append('licence', file);
    return authAxiosClient.post('/admin/licencia', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },
  delete: () => authAxiosClient.delete('/admin/licencia'),
};
