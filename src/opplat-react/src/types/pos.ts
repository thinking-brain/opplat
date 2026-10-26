export type SalePaymentMethod = 'Cash' | 'Card' | 'Other';
export type SalePaymentStatus = 'Pending' | 'Captured' | 'Failed' | 'Cancelled';
export type PaymentAttemptStatus = 'Pending' | 'Approved' | 'Declined' | 'Cancelled' | 'TimedOut' | 'Unknown';
export type SaleStatus = 'Completed' | 'Draft' | 'Voided';

export interface PosRegister {
  id: string;
  code: string;
  name: string;
  invoiceSeries: string;
  isActive: boolean;
  storageId?: string | null;
}

export interface PaymentTerminal {
  id: string;
  registerId?: string | null;
  provider: string;
  providerTerminalId: string;
  displayName: string;
  isActive: boolean;
}

export interface PosShift {
  id: string;
  registerId: string;
  status: 'Open' | 'Closed';
  openedBy: string;
  openedAtUtc: string;
  openingFloat: number;
  closedAtUtc?: string | null;
  expectedCash?: number | null;
  countedCash?: number | null;
}

export interface ShiftReport {
  shift: PosShift;
  salesCount: number;
  totalSales: number;
  byPaymentMethod: { key: string; count: number; amount: number }[];
  byTaxRate: { rate: number; taxableBase: number; taxAmount: number }[];
  payIn: number;
  payOut: number;
  expectedCash: number;
}

export interface PosAttempt {
  id: string;
  status: PaymentAttemptStatus;
  provider: string;
  providerReference?: string | null;
  authorizationCode?: string | null;
  cardBrand?: string | null;
  cardLast4?: string | null;
  failureReason?: string | null;
  customerReceiptText?: string | null;
}

export interface PosPayment {
  id: string;
  saleId: string;
  method: SalePaymentMethod;
  status: SalePaymentStatus;
  amount: number;
  tenderedAmount?: number | null;
  changeGiven?: number | null;
  attempt?: PosAttempt | null;
}

export interface PosSaleLine {
  productId: string;
  name: string;
  quantity: number;
  unitPrice: number;
  taxRate: number;
  discount: number;
  amount: number;
}

export interface PosSale {
  id: string;
  status: SaleStatus;
  date: string;
  registerId?: string | null;
  shiftId?: string | null;
  cashier?: string | null;
  currency: string;
  total: number;
  paid: number;
  due: number;
  lines: PosSaleLine[];
  payments: PosPayment[];
  invoiceId?: string | null;
  invoiceNumber?: string | null;
}

export interface PosCustomer {
  name: string;
  taxId?: string;
  address?: string;
  country?: string;
}
