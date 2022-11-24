import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { InventoriesStorageSelectorComponent } from './inventories/storage-selector/storage-selector.component';
import { ProductMovementFormComponent } from './movements/movement-form/movement-form.component';
import { ProductMovementsComponent } from './movements/movements.component';
import { ProductMovementStorageSelectorComponent } from './movements/storage-selector/storage-selector.component';
import { ProductClassificationFormComponent } from './product-classifications/product-classification-form/product-classification-form.component';
import { ProductClassificationsComponent } from './product-classifications/product-classifications.component';
import { ProductGroupFormComponent } from './product-groups/product-group-form/product-group-form.component';
import { ProductGroupsComponent } from './product-groups/product-groups.component';
import { ProductFormComponent } from './products/product-form/product-form.component';
import { ProductsComponent } from './products/products.component';
import { StorageFormComponent } from './storages/storage-form/storage-form.component';
import { StoragesComponent } from './storages/storages.component';

const routes: Routes = [
  {
    path: 'storages',
    component: StoragesComponent
  },
  {
    path: 'storage/form',
    component: StorageFormComponent
  },
  {
    path: 'product-classifications',
    component: ProductClassificationsComponent
  },
  {
    path: 'product-classification/form',
    component: ProductClassificationFormComponent
  },
  {
    path: 'product-groups',
    component: ProductGroupsComponent
  },
  {
    path: 'product-group/form',
    component: ProductGroupFormComponent
  },
  {
    path: 'products',
    component: ProductsComponent
  },
  {
    path: 'product/form',
    component: ProductFormComponent
  },
  // {
  //   path: 'product-movements',
  //   component: ProductMovementsComponent
  // },
  {
    path: 'product-movements',
    component: ProductMovementStorageSelectorComponent
  },
  {
    path: 'product-movement/form',
    component: ProductMovementFormComponent
  },
  {
    path: 'inventories',
    component: InventoriesStorageSelectorComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InventoryRoutingModule { }
