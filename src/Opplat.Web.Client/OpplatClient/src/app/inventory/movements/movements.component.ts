import { AfterViewInit, Component, Input, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, of } from 'rxjs';
import { ConfirmDeleteDialogComponent } from 'src/app/commons/confirm-delete-dialog/confirm-delete-dialog.component';
import { LoadingDialogComponent } from 'src/app/commons/loading-dialog/loading-dialog.component';
import { ProductMovement } from '../entities/movements';
import { ProductMovementsService } from '../services/movements.service';
import { MovementsDataSource } from './movements-datasource';

@Component({
  selector: 'app-product-movements',
  templateUrl: './movements.component.html',
  styleUrls: ['./movements.component.sass']
})
export class ProductMovementsComponent implements AfterViewInit {
  title = 'Movimientos de productos';
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatTable) table!: MatTable<ProductMovement>;

  @Input('storage-id') storageId: BehaviorSubject<string> = new BehaviorSubject("a");

  /** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
  displayedColumns = ['date', 'storage', 'product', 'type', 'quatity', 'observations'];



  constructor(public dataSource: MovementsDataSource, private router: Router, private service: ProductMovementsService,
    private loading: LoadingDialogComponent,private _snackBar: MatSnackBar, private confirmDelete: ConfirmDeleteDialogComponent) {
  }

  ngAfterViewInit(): void {
    this.loadData();
  }

  loadData():void{
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
    this.storageId.subscribe(id => {
      if (id === 'undefined') {
        return;
      }
      this.loading.open();
      this.service.getByStorage(id).pipe(catchError((err)=>{
        this._snackBar.open('Error de conexion con el servidor','Cerrar', {duration:3000});
        return of([] as ProductMovement[]);
      })).subscribe(r => {
        this.dataSource.data = r;
        this.table.dataSource = this.dataSource;
        this.loading.close();
      });
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
    this.service.selectedEntity = null;
    this.router.navigate(['inventory', 'product-movement', 'form']);
  }
}
