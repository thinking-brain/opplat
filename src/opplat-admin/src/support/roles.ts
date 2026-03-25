export const SUPER_ADMIN_ROLE = 'SuperAdmin';
export const TENANT_ADMIN_ROLE = 'TenantAdmin';
export const TENANT_USER_ROLE = 'TenantUser';

const legacyRoleMap: Record<string, string> = {
  admin: SUPER_ADMIN_ROLE,
  adminonly: SUPER_ADMIN_ROLE,
  operator: TENANT_ADMIN_ROLE,
  manager: TENANT_ADMIN_ROLE,
  user: TENANT_USER_ROLE,
};

export const normalizeRole = (role: string): string => {
  const normalized = role.trim();
  if (!normalized) {
    return normalized;
  }

  return legacyRoleMap[normalized.toLowerCase()] ?? normalized;
};

export const normalizeRoles = (roles: string[]): string[] =>
  [...new Set(roles.map(normalizeRole).filter(Boolean))];
