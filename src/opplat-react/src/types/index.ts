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
  pricingAnnual: number;
  currency: string;
  stripePriceIdMonthly?: string;
  stripePriceIdAnnual?: string;
  maxActiveUsers: number;
  isActive: boolean;
}

export interface TenantSubscriptionDetails {
  tenantIdentifier: string;
  subscriptionPlanId: string;
  subscriptionPlanName: string;
  billingInterval: string;
  pricingMonthly: number;
  pricingAnnual: number;
  currency: string;
  billingStatus: string;
  nextBillingDate?: string;
  cancelAtPeriodEnd: boolean;
}

export interface ChangeSubscriptionRequest {
  subscriptionPlanId: string;
  billingInterval: string;
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
  billingInterval?: 'Monthly' | 'Annual';
  cardNumber?: string;
  cardExpMonth?: number;
  cardExpYear?: number;
  cardCvc?: string;
}

export interface TenantPaymentMethodDto {
  id: string;
  brand?: string;
  last4?: string;
  expMonth?: number;
  expYear?: number;
  isDefault: boolean;
}

export interface AddPaymentMethodRequest {
  cardNumber: string;
  expMonth: number;
  expYear: number;
  cvc: string;
  setAsDefault?: boolean;
}

export type TenantBillingStatus = 'Trialing' | 'Active' | 'PastDue' | 'Cancelled';

export interface SubscriptionPaymentHistoryItem {
  invoiceId: string;
  amount: number;
  currency: string;
  status: 'Paid' | 'Open' | 'Failed' | 'Void';
  periodStart: string;
  periodEnd: string;
  paidAt?: string;
  hostedInvoiceUrl?: string;
}

export interface TenantRegistrationResult {
  succeeded: boolean;
  tenantIdentifier?: string;
  message?: string;
}

export interface Warehouse {
  id: string;
  code: string;
  description: string;
  isCostCenter: boolean;
}

export interface ProductClassification {
  id: number;
  description: string;
}

export interface ProductGroup {
  id: string;
  description: string;
  classification?: ProductClassification;
  classificationId: number;
}

export interface MovementType {
  id: number;
  name: string;
}

// ── Invoicing ────────────────────────────────────────────────────────────────

export type InvoiceType = 'Simplified' | 'Full' | 'Rectifying' | 'Substitutive';
export type InvoiceStatus = 'Draft' | 'Issued' | 'Sent' | 'Cancelled';
export type FiscalSubmissionStatus = 'NotApplicable' | 'Pending' | 'Submitted' | 'Accepted' | 'Rejected';
export type InvoicingMode = 'None' | 'Verifactu' | 'NonVerifactuSigned';

export interface CustomerSnapshot {
  name: string;
  taxId?: string;
  address?: string;
  country?: string;
  isFinalConsumer: boolean;
}

export interface InvoiceLine {
  id?: string;
  productId?: string;
  description: string;
  quantity: number;
  unitPrice: number;
  discountAmount: number;
  taxRate: number;
  lineTotal: number;
}

export interface InvoiceTaxBreakdown {
  taxType: string;
  rate: number;
  taxableBase: number;
  taxAmount: number;
}

export interface InvoiceFiscalRecord {
  previousRecordHash?: string;
  recordHash: string;
  generatedAtUtc: string;
  submissionMode: string;
  submissionStatus: FiscalSubmissionStatus;
  aeatCsv?: string;
  aeatSubmittedAtUtc?: string;
  qrCodePayload?: string;
  retryCount: number;
  nextRetryAtUtc?: string;
  lastErrorMessage?: string;
}

export interface Invoice {
  id?: string;
  series: string;
  number: number;
  fullNumber: string;
  issueDate: string;
  invoiceType: InvoiceType;
  status: InvoiceStatus;
  saleId?: string;
  customerId?: string;
  customerSnapshot: CustomerSnapshot;
  currency: string;
  subtotal: number;
  totalAmount: number;
  paymentMethod?: string;
  notes?: string;
  lines: InvoiceLine[];
  taxBreakdowns?: InvoiceTaxBreakdown[];
  fiscalRecord?: InvoiceFiscalRecord;
}

export interface TenantFiscalSettings {
  legalName: string;
  taxId: string;
  fiscalAddress: string;
  country?: string;
  businessSector?: string;
  defaultSeries: string;
  invoicingMode: InvoicingMode;
  simplifiedInvoiceThreshold: number;
  softwareLicenseId?: string;
  email?: string;
  phone?: string;
  website?: string;
}

