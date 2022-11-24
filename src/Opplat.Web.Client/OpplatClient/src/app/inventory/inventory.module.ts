import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { InventoryRoutingModule } from './inventory-routing.module';
import { StoragesComponent } from './storages/storages.component';
import { StorageFormComponent } from './storages/storage-form/storage-form.component';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { ProductsComponent } from './products/products.component';
import { ProductFormComponent } from './products/product-form/product-form.component';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatRadioModule } from '@angular/material/radio';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatGridListModule } from '@angular/material/grid-list';
import { ReactiveFormsModule } from '@angular/forms';
import { ProductClassificationsComponent } from './product-classifications/product-classifications.component';
import { ProductClassificationFormComponent } from './product-classifications/product-classification-form/product-classification-form.component';
import { ProductGroupsComponent } from './product-groups/product-groups.component';
import { ProductGroupFormComponent } from './product-groups/product-group-form/product-group-form.component';
import { ProductMovementsComponent } from './movements/movements.component';
import { ProductMovementFormComponent } from './movements/movement-form/movement-form.component';
import { ProductMovementStorageSelectorComponent } from './movements/storage-selector/storage-selector.component';
import { InventoriesComponent } from './inventories/inventories.component';
import { InventoriesStorageSelectorComponent } from './inventories/storage-selector/storage-selector.component';


@NgModule({
  declarations: [
    StoragesComponent,
    StorageFormComponent,
    ProductsComponent,
    ProductFormComponent,
    ProductClassificationsComponent,
    ProductClassificationFormComponent,
    ProductGroupsComponent,
    ProductGroupFormComponent,
    ProductMovementsComponent,
    ProductMovementFormComponent,
    ProductMovementStorageSelectorComponent,
    InventoriesComponent,
    InventoriesStorageSelectorComponent
  ],
  imports: [
    CommonModule,
    InventoryRoutingModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatRadioModule,
    MatCardModule,
    MatIconModule,
    MatGridListModule,
    MatCheckboxModule,
    ReactiveFormsModule
  ]
})
export class InventoryModule { }
