import { Product } from "./product";
import { Warehouse } from "./warehouse";

export interface MovementType{
  id: number;
  name: string;
}

export interface ProductMovement {
  id: string;
  storage: Warehouse;
  storageId: string;
  product: Product;
  productId: string;
  type: number;
  quantity: number;
  unit: string;
  observations: string;
  date: Date;
  user: string;
}
