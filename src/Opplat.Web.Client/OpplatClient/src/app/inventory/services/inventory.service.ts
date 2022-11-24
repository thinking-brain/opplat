import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from 'src/app/services/base.service';
import { Inventory } from '../entities/inventory';
import { MovementType, ProductMovement } from '../entities/movements';


@Injectable({
  providedIn: 'root'
})
export class InventoryService extends BaseService<Inventory>{

  constructor(httpClient: HttpClient) {
    super(httpClient);
    this.baseUrl += 'inventory/Inventories/';
  }

  getByStorage(storageId: string): Observable<Inventory[]>{
    return this.httpClient.get<Inventory[]>(this.baseUrl + storageId);
  }
}
