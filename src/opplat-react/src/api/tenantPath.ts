import { getStoredTenantIdentifier } from '../auth/claims';
import { appConfig } from '../runtimeConfig';

const normalizePath = (path: string): string => (path.startsWith('/') ? path : `/${path}`);
const trimTrailingSlash = (value: string): string => value.replace(/\/+$/, '');

export const prefixTenantPath = (
  path: string,
  tenantIdentifier = getStoredTenantIdentifier(),
): string => {
  const normalizedPath = normalizePath(path);
  if (!tenantIdentifier) {
    return normalizedPath;
  }

  return normalizedPath.startsWith(`/${tenantIdentifier}/`)
    ? normalizedPath
    : `/${tenantIdentifier}${normalizedPath}`;
};

export const buildServiceUrl = (
  baseUrl: string,
  path: string,
  tenantIdentifier = getStoredTenantIdentifier(),
): string => `${trimTrailingSlash(baseUrl)}${prefixTenantPath(path, tenantIdentifier)}`;

export const buildAuthAssetUrl = (path: string): string => buildServiceUrl(appConfig.authApiUrl, path);
export const buildSalesAssetUrl = (path: string): string => buildServiceUrl(appConfig.salesApiUrl, path);
