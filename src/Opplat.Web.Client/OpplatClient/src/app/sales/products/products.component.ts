import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import { Router } from '@angular/router';
import { catchError, of } from 'rxjs';
import { ConfirmDeleteDialogComponent } from 'src/app/commons/confirm-delete-dialog/confirm-delete-dialog.component';
import { LoadingDialogComponent } from 'src/app/commons/loading-dialog/loading-dialog.component';
import { Product } from '../entities/product';
import { ProductsService } from '../services/products.service';
import { ProductsDataSource } from './products-datasource';

@Component({
  selector: 'app-products-for-sale',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.sass']
})
export class ProductsComponent implements AfterViewInit {
  title = 'Productos';
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatTable) table!: MatTable<Product>;

  /** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
  displayedColumns = ['code','name', 'description', 'price', 'active', 'actions'];



  constructor(public dataSource: ProductsDataSource, private router: Router, private productServ: ProductsService,
     private loading: LoadingDialogComponent,private _snackBar: MatSnackBar, private confirmDelete: ConfirmDeleteDialogComponent) {
  }

  ngAfterViewInit(): void {
    this.loadData();
  }


  loadData():void{
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
    this.loading.open();
    this.productServ.list().pipe(catchError((err)=>{
      this._snackBar.open('Error de conexion con el servidor','Cerrar', {duration:3000});
      return of([] as Product[]);
    })).subscribe(r => {
      this.dataSource.data = r;
      this.table.dataSource = this.dataSource;
      this.loading.close();
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }


  createNew(): void{
    this.productServ.selectedEntity = null;
    this.router.navigate(['sales', 'product', 'form']);
  }

  edit(product: Product): void{
    this.productServ.selectedEntity = product;
    this.router.navigate(['sales','product', 'form']);
  }

  costtab(product: Product): void{
    this.productServ.selectedEntity = product;
    this.router.navigate(['sales','product', 'costtab']);
  }


  delete(product: Product): void {
    this.confirmDelete.open('Producto', product.name , {}).subscribe(r => {
      if (r) {
        this.productServ.delete(product.id).subscribe(result => {
          if (result.status) {
            const index = this.dataSource.data.indexOf(product);
            this.dataSource.data.splice(index, 1);
            this.table.renderRows();
            // this.dataSource.paginator?.firstPage();
            this._snackBar.open('Eliminado correctamente', 'Cerrar', {duration: 3000});
          } else {
            this._snackBar.open(`No se pudo eliminar el producto.\n\r${result.message}`, 'Cerrar', {duration: 3000});
          }
        })
      }
    });
  }
}
