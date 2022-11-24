import { DataSource } from '@angular/cdk/collections';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { map } from 'rxjs/operators';
import { Observable, of as observableOf, merge, BehaviorSubject } from 'rxjs';
import { ProductsService } from '../services/products.service';
import { Injectable } from '@angular/core';
import { Product } from '../entities/product';
import { MatTableDataSource } from '@angular/material/table';

/**
 * Data source for the Products view. This class should
 * encapsulate all logic for fetching and manipulating the displayed data
 * (including sorting, pagination, and filtering).
 */
 @Injectable({
  providedIn: 'root'
})
export class ProductsDataSource extends MatTableDataSource<Product> {
  // data: Product[] = [];
  // paginator: MatPaginator | undefined;
  // sort: MatSort | undefined;

  constructor(private service: ProductsService) {
    super();
  }

}
