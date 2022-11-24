import { AfterViewInit, ChangeDetectorRef, Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import { Router } from '@angular/router';
import { catchError, of } from 'rxjs';
import { ConfirmDeleteDialogComponent } from 'src/app/commons/confirm-delete-dialog/confirm-delete-dialog.component';
import { LoadingDialogComponent } from 'src/app/commons/loading-dialog/loading-dialog.component';
import { ProductClassification } from '../entities/product-clasification';
import { ProductClassificationService } from '../services/product-classification.service';
import { ProductClassificationsDataSource } from './product-classifications-datasource';

@Component({
  selector: 'app-product-classifications',
  templateUrl: './product-classifications.component.html',
  styleUrls: ['./product-classifications.component.sass']
})
export class ProductClassificationsComponent implements AfterViewInit {
  title = 'Clasificaciones de productos';
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatTable) table!: MatTable<ProductClassification>;

  /** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
  displayedColumns = ['description', 'actions'];



  constructor(public dataSource: ProductClassificationsDataSource, private router: Router,
     private productServ: ProductClassificationService, private loading: LoadingDialogComponent,
     private _snackBar: MatSnackBar, private confirmDelete: ConfirmDeleteDialogComponent) {

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
      return of([] as ProductClassification[]);
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


  createNew(): void {
    this.productServ.selectedEntity = null;
    this.router.navigate(['inventory', 'product-classification', 'form']);
  }

  edit(classification: ProductClassification): void {
    this.productServ.selectedEntity = classification;
    this.router.navigate(['inventory', 'product-classification', 'form']);
  }

  delete(classification: ProductClassification): void {
    this.confirmDelete.open('Clasificacion de producto', classification.description , {}).subscribe(r => {
      if (r) {
        this.productServ.delete(classification.id).subscribe(result => {
          if (result.status) {
            const index = this.dataSource.data.indexOf(classification);
            this.dataSource.data.splice(index, 1);
            this.table.renderRows();
            this._snackBar.open('Eliminado correctamente', 'Cerrar', {duration: 3000});
          } else {
            this._snackBar.open(`No se pudo eliminar la clasificacion.\n\r${result.message}`, 'Cerrar', {duration: 3000});
          }
        })
      }
    });
  }
}
