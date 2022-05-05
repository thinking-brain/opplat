export interface Group {
  id: number;
  descripcion: string;
}
export interface UnidadDeMedida {
  nombre: string;
  siglas: string;
}
export interface Product {
  id: number;
  nombre: string;
  cantidad: string;
  grupo: Group;
  precioDeVenta: number;
  precioUnitario: number;
  unidadDeMedida: UnidadDeMedida;
  esInventariable: boolean;
  stockMinimo: number;
  proporcionDeMerma: number;
  active: boolean;
  picture?: File;
}
export interface BottomNavigations {
  name: string;
  route: string;
  icon: string;
}
export interface Icon {
  nombre: string;
  link: string;
  color: string;
}
export interface Filters {
  category_id?: any;
  offset?: number;
  limit?: number;
}
export interface Options {
  page: number;
  itemsPerPage: number;
  sortBy: string[];
  sortDesc: boolean[];
  groupBy: string[];
  groupDesc: boolean[];
  multiSort: boolean;
  mustSort: boolean;
}
export interface Pagination {
  limits: number[];
  defaultLimit?: number;
  length: number;
  totalVisible?: number;
}

export type VForm = Vue & {
  validate: () => boolean;
  resetValidation: () => boolean;
  reset: () => void;
};
