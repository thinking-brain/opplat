import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ProductCostTabComponent } from './product-costtab/product-costtab.component';
import { ProductFormComponent } from './products/product-form/product-form.component';
import { ProductsComponent } from './products/products.component';

const routes: Routes = [
  {
    path: 'products',
    component: ProductsComponent
  },
  {
    path: 'product/form',
    component: ProductFormComponent
  },
  {
    path: 'product/costtab',
    component: ProductCostTabComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SalesRoutingModule { }
