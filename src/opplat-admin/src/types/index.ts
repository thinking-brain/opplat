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

export interface TenantProvisioningResult {
  tenantId: string;
  tenantIdentifier: string;
  databaseSchema: string;
  databaseName: string;
  databaseInstanceIdentifier: string;
  succeeded: boolean;
  alreadyProvisioned: boolean;
  schemaCreated: boolean;
  errorMessage?: string;
  executedAt: string;
}
