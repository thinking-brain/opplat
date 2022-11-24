
export interface ProductClassification
{
  id:number;
  description:string;
}

export interface ProductGroup{
  id:string;
  description:string;
  classification: ProductClassification;
  classificationId: number;
}

export interface UnitOfMeasurement{
  abbreviation: string;
  name: string;
  unitType: number;
}
