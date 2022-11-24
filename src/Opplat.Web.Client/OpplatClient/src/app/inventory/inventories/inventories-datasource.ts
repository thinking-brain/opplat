import { Injectable } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Inventory } from '../entities/inventory';
import { InventoryService } from '../services/inventory.service';

/**
 * Data source for the Products view. This class should
 * encapsulate all logic for fetching and manipulating the displayed data
 * (including sorting, pagination, and filtering).
 */
 @Injectable({
  providedIn: 'root'
})
export class InventoriesDataSource extends MatTableDataSource<Inventory> {

  constructor() {
    super();
  }

  // override sortData(data: Inventory[], sort: MatSort) : Inventory[]{

  // }
}
