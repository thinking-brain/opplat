import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import { Router } from '@angular/router';
import { catchError, of } from 'rxjs';
import { ConfirmDeleteDialogComponent } from 'src/app/commons/confirm-delete-dialog/confirm-delete-dialog.component';
import { LoadingDialogComponent } from 'src/app/commons/loading-dialog/loading-dialog.component';
import { Warehouse } from '../entities/warehouse';
import { StorageService } from '../services/storage.service';
import { StoragesDataSource } from './storages-datasource';

@Component({
  selector: 'app-storages',
  templateUrl: './storages.component.html',
  styleUrls: ['./storages.component.sass']
})
export class StoragesComponent implements AfterViewInit {
  title = 'Almacenes';
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatTable) table!: MatTable<Warehouse>;

  /** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
  displayedColumns = ['code', 'description', 'isCostCenter', 'actions'];



  constructor(public dataSource: StoragesDataSource, private router: Router, private productServ: StorageService,
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
      return of([] as Warehouse[]);
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
    this.router.navigate(['inventory', 'storage', 'form']);
  }

  edit(entity: Warehouse): void{
    this.productServ.selectedEntity = entity;
    this.router.navigate(['inventory','storage', 'form']);
  }

  delete(entity: Warehouse): void {
    this.confirmDelete.open('Almacen', entity.description , {}).subscribe(r => {
      if (r) {
        this.productServ.delete(entity.id).subscribe(result => {
          if (result.status) {
            const index = this.dataSource.data.indexOf(entity);
            this.dataSource.data.splice(index, 1);
            this.table.renderRows();
            this._snackBar.open('Eliminado correctamente', 'Cerrar', {duration: 3000});
          } else {
            this._snackBar.open(`No se pudo eliminar el almacen.\n\r${result.message}`, 'Cerrar', {duration: 3000});
          }
        })
      }
    });
  }
}
