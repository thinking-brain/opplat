import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/services/base.service';
import { Product } from '../entities/product';


@Injectable({
  providedIn: 'root'
})
export class ProductsService extends BaseService<Product>{

  constructor(httpClient: HttpClient) {
    super(httpClient);
    this.baseUrl += 'inventory/Products/';
  }
}
