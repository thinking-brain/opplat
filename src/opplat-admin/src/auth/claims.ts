import type { User as OidcUser } from 'oidc-client-ts';
import type { User } from '../types';
import { normalizeRoles } from './roles';

type ClaimProfile = Record<string, unknown>;

const ROLE_CLAIM_KEYS = [
  'roles',
  'role',
  'realm_access.roles',
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
  'https://opplat.com/roles',
];
const TENANT_ID_KEYS = ['tenant_id', 'https://opplat.com/tenant_id'];
const TENANT_IDENTIFIER_KEYS = ['tenant_identifier', 'https://opplat.com/tenant_identifier'];
const SELECTED_TENANT_KEY = 'opplat_admin_tenant';

const decodeJwtClaims = (token?: string): ClaimProfile => {
  if (!token) {
    return {};
  }

  try {
    const payload = token.split('.')[1];
    if (!payload) {
      return {};
    }

    const base64 = payload.replace(/-/g, '+').replace(/_/g, '/');
    const normalized = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), '=');
    return JSON.parse(window.atob(normalized)) as ClaimProfile;
  } catch {
    return {};
  }
};

const getClaimSources = (oidcUser?: OidcUser | null): ClaimProfile[] => [
  decodeJwtClaims(oidcUser?.id_token),
  decodeJwtClaims(oidcUser?.access_token),
  ((oidcUser?.profile as ClaimProfile | undefined) ?? {}),
];

const getMergedClaims = (oidcUser?: OidcUser | null): ClaimProfile => ({
  ...getClaimSources(oidcUser).reduce<ClaimProfile>((mergedClaims, claimSource) => ({
    ...mergedClaims,
    ...claimSource,
  }), {}),
});

const asString = (value: unknown): string | null =>
  typeof value === 'string' && value.trim().length > 0 ? value : null;

const asStringArray = (value: unknown): string[] => {
  if (Array.isArray(value)) {
    return value.filter((item): item is string => typeof item === 'string' && item.length > 0);
  }

  if (typeof value === 'string' && value.length > 0) {
    return [value];
  }

  return [];
};

const readClaim = (claims: ClaimProfile, keys: string[]): unknown => {
  for (const key of keys) {
    if (key in claims) {
      return claims[key];
    }
  }

  return undefined;
};

const getRealmRoles = (claims: ClaimProfile): string[] => {
  const realmAccess = claims.realm_access;
  const nestedRoles =
    realmAccess && typeof realmAccess === 'object' && 'roles' in realmAccess
      ? asStringArray((realmAccess as { roles?: unknown }).roles)
      : [];

  return [...nestedRoles, ...asStringArray(claims['realm_access.roles'])];
};

const getResourceRoles = (claims: ClaimProfile): string[] => {
  const resourceAccess = claims.resource_access;
  const nestedRoles =
    resourceAccess && typeof resourceAccess === 'object'
      ? Object.values(resourceAccess as Record<string, { roles?: unknown }>)
        .flatMap((resourceClaim) => asStringArray(resourceClaim?.roles))
      : [];

  const flatRoles = Object.entries(claims)
    .filter(([claimType]) => /^resource_access\.[^.]+\.roles$/i.test(claimType))
    .flatMap(([, claimValue]) => asStringArray(claimValue));

  return [...nestedRoles, ...flatRoles];
};

export const getRolesFromClaims = (claims: ClaimProfile): string[] => {
  const collected = [
    ...ROLE_CLAIM_KEYS.flatMap((key) => asStringArray(claims[key])),
    ...getRealmRoles(claims),
    ...getResourceRoles(claims),
  ];

  return normalizeRoles(collected);
};

export const getTenantIdentifierFromClaims = (claims: ClaimProfile): string | null =>
  asString(readClaim(claims, TENANT_IDENTIFIER_KEYS));

export const getTenantIdentifierFromUser = (oidcUser?: OidcUser | null): string | null =>
  getTenantIdentifierFromClaims(getMergedClaims(oidcUser));

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

export const buildAdminUser = (oidcUser: OidcUser): User => {
  const claims = getMergedClaims(oidcUser);
  const username =
    asString(readClaim(claims, ['preferred_username', 'unique_name', 'name', 'email'])) ?? 'admin';
  const fullName = asString(readClaim(claims, ['name'])) ?? username;
  const givenName = asString(readClaim(claims, ['given_name'])) ?? fullName.split(' ')[0] ?? username;
  const familyName = asString(readClaim(claims, ['family_name'])) ?? fullName.split(' ').slice(1).join(' ');

  return {
    userId: asString(readClaim(claims, ['user_id', 'sub'])) ?? username,
    username,
    name: givenName,
    lastName: familyName,
    email: asString(readClaim(claims, ['email'])) ?? '',
    active: true,
    roles: getRolesFromClaims(claims),
    tenantId: asString(readClaim(claims, TENANT_ID_KEYS)) ?? undefined,
    tenantIdentifier: getTenantIdentifierFromClaims(claims) ?? undefined,
  };
};
