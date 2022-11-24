import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BaseService } from 'src/app/services/base.service';
import { ConfigService } from 'src/app/services/config.service';
import { ProductGroup } from '../entities/product-clasification';

@Injectable({
  providedIn: 'root'
})
export class ProductGroupService extends BaseService<ProductGroup> {

  constructor(httpClient: HttpClient) {
    super(httpClient);
    this.baseUrl += 'inventory/ProductGroups/';
  }
}
