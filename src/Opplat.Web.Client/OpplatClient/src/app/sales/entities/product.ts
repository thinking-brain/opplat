
export interface Product {
  id: string;
  code: string;
  name: string;
  description: string;
  price:number;
  active: boolean;
}


export interface CostTab {
  product: Product;
  preparation: string;
  presentation: string;
  expectedRate: number;
  plannedCost: number;
  fixedCost: number;
  cost: number;
}



export interface CostTabDetail {
  product: Product;
  quantity: number;
  unit: string;
  cost: number;
}


