export interface User {
  userId: string;
  name: string;
  lastName: string;
  username: string;
  email: string;
  active: boolean;
  roles: string[];
  tenantId?: string;
  tenantIdentifier?: string;
  tenantName?: string;
}

export interface AdminTenant {
  id: string;
  identifier: string;
  name: string;
  connectionString: string;
  isActive: boolean;
}

export interface UpsertTenantRequest {
  id?: string;
  identifier: string;
  name: string;
  connectionString: string;
  isActive: boolean;
}

export interface AdminCreateUserRequest {
  name: string;
  lastName: string;
  username: string;
  email: string;
  roles: string[];
}

export interface AdminUpdateUserRequest {
  name: string;
  lastName: string;
  username: string;
  email: string;
  active: boolean;
}

export interface AdminSetUserRolesRequest {
  roles: string[];
}

export interface AdminSetUserActiveRequest {
  active: boolean;
}
