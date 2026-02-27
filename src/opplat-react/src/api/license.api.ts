import axiosClient from './axiosClient';
import type { LicenseInfo } from '../types';

export const licenseApi = {
  get: () => axiosClient.get<LicenseInfo>('/admin/licencia'),
  upload: (file: File) => {
    const form = new FormData();
    form.append('licence', file);
    return axiosClient.post('/admin/licencia', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },
  delete: () => axiosClient.delete('/admin/licencia'),
};
