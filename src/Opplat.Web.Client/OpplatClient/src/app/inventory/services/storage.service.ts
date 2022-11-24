import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/services/base.service';
import { Warehouse } from '../entities/warehouse';

@Injectable({
  providedIn: 'root'
})
export class StorageService extends BaseService<Warehouse>{

  constructor(httpClient: HttpClient) {
    super(httpClient);
    this.baseUrl += 'inventory/storages/';
  }

}
