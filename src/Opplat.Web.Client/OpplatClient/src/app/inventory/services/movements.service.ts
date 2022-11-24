import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from 'src/app/services/base.service';
import { MovementType, ProductMovement } from '../entities/movements';


@Injectable({
  providedIn: 'root'
})
export class ProductMovementsService extends BaseService<ProductMovement>{

  constructor(httpClient: HttpClient) {
    super(httpClient);
    this.baseUrl += 'inventory/ProductMovements/';
  }

  getByStorage(storageId: string): Observable<ProductMovement[]>{
    return this.httpClient.get<ProductMovement[]>(this.baseUrl + storageId);
  }
}


@Injectable({
  providedIn: 'root'
})
export class MovementTypesService extends BaseService<MovementType>{

  constructor(httpClient: HttpClient) {
    super(httpClient);
    this.baseUrl += 'inventory/MovementTypes/';
  }
}
