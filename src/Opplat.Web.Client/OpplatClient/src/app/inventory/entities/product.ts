import { ProductGroup, UnitOfMeasurement } from "./product-clasification";


export interface Product {
  id: string;
  code: string;
  name: string;
  description: string;
  itsInventoriable: boolean;
  groupId: number;
  group: ProductGroup;
  unitOfMeasurement: UnitOfMeasurement;
  unit: string;
  containerQuantity: number;
  shrinkRatio: number;
  unitValue: number;
  cost:number;
}
