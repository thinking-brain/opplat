const formatter = new Intl.NumberFormat('es-ES', { style: 'currency', currency: 'EUR' });

export const formatMoney = (value: number): string => formatter.format(value);
