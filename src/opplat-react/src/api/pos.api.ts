import { salesAxiosClient } from './axiosClient';
import {
  PaymentTerminal,
  PosCustomer,
  PosPayment,
  PosRegister,
  PosSale,
  PosShift,
  ShiftReport,
} from '../types/pos';

export const posApi = {
  listRegisters: async (): Promise<PosRegister[]> =>
    (await salesAxiosClient.get<PosRegister[]>('/pos/registers')).data,

  createRegister: async (register: { code: string; name: string; invoiceSeries: string }): Promise<PosRegister> =>
    (await salesAxiosClient.post<PosRegister>('/pos/registers', register)).data,

  listTerminals: async (): Promise<PaymentTerminal[]> =>
    (await salesAxiosClient.get<PaymentTerminal[]>('/pos/terminals')).data,

  listProviders: async (): Promise<string[]> =>
    (await salesAxiosClient.get<string[]>('/pos/payment-providers')).data,

  createTerminal: async (terminal: {
    registerId?: string | null;
    provider: string;
    providerTerminalId: string;
    displayName: string;
  }): Promise<PaymentTerminal> =>
    (await salesAxiosClient.post<PaymentTerminal>('/pos/terminals', terminal)).data,

  deactivateTerminal: async (id: string): Promise<void> => {
    await salesAxiosClient.delete(`/pos/terminals/${id}`);
  },

  currentShift: async (registerId: string): Promise<PosShift | null> => {
    const response = await salesAxiosClient.get<PosShift>(`/pos/registers/${registerId}/shift`, {
      validateStatus: (status) => status === 200 || status === 404,
    });
    return response.status === 404 ? null : response.data;
  },

  openShift: async (registerId: string, openingFloat: number): Promise<PosShift> =>
    (await salesAxiosClient.post<PosShift>(`/pos/registers/${registerId}/shifts`, { openingFloat })).data,

  closeShift: async (shiftId: string, countedCash: number): Promise<ShiftReport> =>
    (await salesAxiosClient.post<ShiftReport>(`/pos/shifts/${shiftId}/close`, { countedCash })).data,

  shiftReport: async (shiftId: string): Promise<ShiftReport> =>
    (await salesAxiosClient.get<ShiftReport>(`/pos/shifts/${shiftId}/report`)).data,

  addCashMovement: async (shiftId: string, type: 'PayIn' | 'PayOut', amount: number, reason: string): Promise<void> => {
    await salesAxiosClient.post(`/pos/shifts/${shiftId}/cash-movements`, { type, amount, reason });
  },

  createSale: async (
    registerId: string,
    lines: { productId: string; quantity: number; discount?: number }[],
  ): Promise<PosSale> => (await salesAxiosClient.post<PosSale>('/pos/sales', { registerId, lines })).data,

  getSale: async (id: string): Promise<PosSale> => (await salesAxiosClient.get<PosSale>(`/pos/sales/${id}`)).data,

  listSales: async (shiftId?: string): Promise<PosSale[]> =>
    (await salesAxiosClient.get<PosSale[]>('/pos/sales', { params: { shiftId } })).data,

  addPayment: async (
    saleId: string,
    payment: { method: 'Cash' | 'Card' | 'Other'; amount?: number; terminalId?: string },
    idempotencyKey?: string,
  ): Promise<PosPayment> =>
    (
      await salesAxiosClient.post<PosPayment>(`/pos/sales/${saleId}/payments`, payment, {
        headers: idempotencyKey ? { 'Idempotency-Key': idempotencyKey } : undefined,
      })
    ).data,

  getPayment: async (id: string): Promise<PosPayment> =>
    (await salesAxiosClient.get<PosPayment>(`/pos/payments/${id}`)).data,

  cancelPayment: async (id: string): Promise<PosPayment> =>
    (await salesAxiosClient.post<PosPayment>(`/pos/payments/${id}/cancel`)).data,

  completeSale: async (saleId: string, customer?: PosCustomer): Promise<PosSale> =>
    (await salesAxiosClient.post<PosSale>(`/pos/sales/${saleId}/complete`, { customer: customer ?? null })).data,

  voidSale: async (saleId: string): Promise<PosSale> =>
    (await salesAxiosClient.post<PosSale>(`/pos/sales/${saleId}/void`)).data,

  ticketPdf: async (invoiceId: string): Promise<Blob> =>
    (await salesAxiosClient.get<Blob>(`/invoices/${invoiceId}/pdf`, { responseType: 'blob' })).data,
};

/** Problem-details `detail` from a failed POS call, falling back to a generic message. */
export const posErrorMessage = (error: unknown, fallback: string): string => {
  const detail = (error as { response?: { data?: { detail?: string } } })?.response?.data?.detail;
  return detail ?? fallback;
};
