import axios, { AxiosInstance, InternalAxiosRequestConfig } from 'axios';

const axiosClient: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5000',
  headers: {
    'Content-Type': 'application/json',
  },
});

axiosClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem('opplat_token');
    if (token && config.headers) {
      config.headers.Authorization = token;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

axiosClient.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('opplat_token');
      localStorage.removeItem('opplat_user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default axiosClient;
