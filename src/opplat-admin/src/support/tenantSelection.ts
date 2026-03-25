const SELECTED_TENANT_KEY = 'opplat_admin_tenant';

export const getStoredTenantIdentifier = (): string | null =>
  typeof window === 'undefined' ? null : window.localStorage.getItem(SELECTED_TENANT_KEY);

export const persistTenantIdentifier = (tenantIdentifier: string | null): void => {
  if (typeof window === 'undefined') {
    return;
  }

  if (tenantIdentifier) {
    window.localStorage.setItem(SELECTED_TENANT_KEY, tenantIdentifier);
    return;
  }

  window.localStorage.removeItem(SELECTED_TENANT_KEY);
};
