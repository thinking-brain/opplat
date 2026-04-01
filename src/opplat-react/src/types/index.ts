export interface User {
  userId: string;
  name: string;
  lastName: string;
  username: string;
  email: string;
  active: boolean;
  roles: string[];
  profilePicture?: string;
  tenantId?: string;
  tenantIdentifier?: string;
}

export interface ProductForSale {
  id?: string;
  name: string;
  price: number;
  stock?: number;
  description?: string;
  active?: boolean;
  imageUrl?: string;
}

export interface Sale {
  id?: string;
  date: string;
  total: number;
  items: SaleItem[];
}

export interface SaleItem {
  productId: string;
  productName: string;
  quantity: number;
  price: number;
  subtotal: number;
}

export interface ResponseDto {
  status: boolean;
  message: string;
  errors: string[];
}

export interface RegisterUser {
  name: string;
  lastName: string;
  username: string;
  email: string;
  password: string;
}

export interface Tenant {
  id: string;
  identifier: string;
  name: string;
}

export interface TenantAccessContext {
  isResolved: boolean;
  isActive: boolean;
  status: 'active' | 'inactive' | 'unresolved';
  tenantId?: string | null;
  tenantIdentifier?: string | null;
  tenantName?: string | null;
  message: string;
}

export interface LicenseInfo {
  subscriptor: string;
  fechaVencimiento: string;
}

export interface InventoryProduct {
  id: number;
  nombre: string;
  grupo: { id: number; descripcion: string };
  unidadDeMedida: { nombre: string; siglas: string };
  active: boolean;
  existencias?: Array<{ lugar: string; cantidad: number }>;
  existenciaTotal?: number;
}

export interface ProductMovement {
  id: number;
  date: string;
  productId: number;
  storageId: number;
  quantity: number;
  type: string;
  observations: string;
}

export interface SubscriptionPlan {
  id: string;
  name: string;
  description?: string;
  pricingMonthly: number;
  maxActiveUsers: number;
  isActive: boolean;
}

export interface TenantRegistrationRequest {
  firstName: string;
  lastName: string;
  username: string;
  email: string;
  password: string;
  businessName: string;
  tenantIdentifier: string;
  subscriptionPlanId?: string;
}

export interface TenantRegistrationResult {
  succeeded: boolean;
  tenantIdentifier?: string;
  message?: string;
}
