import axios, {
  AxiosHeaders,
  type AxiosInstance,
  type AxiosRequestConfig,
  type InternalAxiosRequestConfig,
} from 'axios';
import { appConfig } from '../runtimeConfig';
import { keycloak } from '../auth/keycloakClient';

declare module 'axios' {
  interface AxiosRequestConfig<D = any> {
    tenantIdentifier?: string;
  }

  interface InternalAxiosRequestConfig<D = any> {
    tenantIdentifier?: string;
  }
}

const setHeader = (
  config: InternalAxiosRequestConfig,
  headerName: string,
  value: string,
): void => {
  if (!config.headers) {
    config.headers = new AxiosHeaders();
  }

  config.headers.set(headerName, value);
};

const createAxiosClient = (baseURL: string): AxiosInstance => {
  const client = axios.create({
    baseURL,
    withCredentials: true,
    headers: {
      'Content-Type': 'application/json',
    },
  });

  client.interceptors.request.use(
    async (config: InternalAxiosRequestConfig) => {
      if (!config.headers?.has?.('X-Requested-With')) {
        setHeader(config, 'X-Requested-With', 'XMLHttpRequest');
      }

      if (keycloak.authenticated) {
        try {
          await keycloak.updateToken(30);
        } catch {
          void keycloak.login();
          return Promise.reject(new Error('Token refresh failed'));
        }
        if (keycloak.token) {
          setHeader(config, 'Authorization', `Bearer ${keycloak.token}`);
        }
      }

      const tenantIdentifier = config.tenantIdentifier;
      if (tenantIdentifier) {
        setHeader(config, 'X-Tenant-Identifier', tenantIdentifier);
      }

      return config;
    },
    (error) => Promise.reject(error),
  );

  client.interceptors.response.use(
    (response) => response,
    (error) => Promise.reject(error),
  );

  return client;
};

export const adminAxiosClient = createAxiosClient(appConfig.adminApiUrl);

export const withTenantConfig = (tenantIdentifier: string): AxiosRequestConfig => ({
  tenantIdentifier,
});
