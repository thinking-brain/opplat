export interface AdminTenant {
  id: string;
  identifier: string;
  name: string;
  databaseName: string;
  databaseSchema: string;
  userCount: number;
  isActive: boolean;
}

export interface UpsertTenantRequest {
  id?: string;
  identifier: string;
  name: string;
  databaseName: string;
  databaseSchema: string;
  isActive: boolean;
}
