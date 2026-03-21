import axios, { AxiosHeaders, type AxiosInstance, type InternalAxiosRequestConfig } from 'axios';
import { getStoredTenantIdentifier, getTenantIdentifierFromUser, persistTenantIdentifier } from '../auth/claims';
import { getOidcUser, removeOidcUser } from '../auth/oidc';
import { appConfig } from '../runtimeConfig';
import { prefixTenantPath } from './tenantPath';

const isAbsoluteUrl = (value: string): boolean => /^https?:\/\//i.test(value);

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

const createAxiosClient = (baseURL: string, tenantScoped = true): AxiosInstance => {
  const client = axios.create({
    baseURL,
    headers: {
      'Content-Type': 'application/json',
    },
  });

  client.interceptors.request.use(
    async (config: InternalAxiosRequestConfig) => {
      const oidcUser = await getOidcUser();
      const accessToken = oidcUser?.access_token;
      if (accessToken) {
        setHeader(config, 'Authorization', `Bearer ${accessToken}`);
      }

      const tenantIdentifier = getStoredTenantIdentifier() ?? getTenantIdentifierFromUser(oidcUser);
      if (tenantIdentifier) {
        persistTenantIdentifier(tenantIdentifier);
        setHeader(config, 'X-Tenant-Identifier', tenantIdentifier);

        if (tenantScoped && config.url && !isAbsoluteUrl(config.url)) {
          config.url = prefixTenantPath(config.url, tenantIdentifier);
        }
      }

      return config;
    },
    (error) => Promise.reject(error)
  );

  client.interceptors.response.use(
    (response) => response,
    async (error) => {
      if (error.response?.status === 401) {
        await removeOidcUser();
        persistTenantIdentifier(null);
        if (!window.location.pathname.startsWith('/login')) {
          window.location.assign(appConfig.authLogoutRedirectPath);
        }
      }
      return Promise.reject(error);
    }
  );

  return client;
};

export const authAxiosClient = createAxiosClient(appConfig.authApiUrl, true);

export const salesAxiosClient = createAxiosClient(appConfig.salesApiUrl, true);

export const inventoryAxiosClient = createAxiosClient(appConfig.inventoryApiUrl, true);

export default authAxiosClient;
