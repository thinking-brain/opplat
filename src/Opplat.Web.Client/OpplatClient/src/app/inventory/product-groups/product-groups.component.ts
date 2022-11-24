import { AfterViewInit, Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import { Router } from '@angular/router';
import { catchError, of } from 'rxjs';
import { ConfirmDeleteDialogComponent } from 'src/app/commons/confirm-delete-dialog/confirm-delete-dialog.component';
import { LoadingDialogComponent } from 'src/app/commons/loading-dialog/loading-dialog.component';
import { ProductGroup } from '../entities/product-clasification';
import { ProductGroupService } from '../services/product-group.service';
import { ProductGroupsDataSource } from './product-groups-datasource';

@Component({
  selector: 'app-product-groups',
  templateUrl: './product-groups.component.html',
  styleUrls: ['./product-groups.component.sass']
})
export class ProductGroupsComponent implements AfterViewInit {
  title = 'Grupos de productos';
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatTable) table!: MatTable<ProductGroup>;

  /** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
  displayedColumns = ['description', 'classification', 'actions'];



  constructor(public dataSource: ProductGroupsDataSource, private router: Router, private productServ: ProductGroupService,
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
      return of([] as ProductGroup[]);
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
    this.router.navigate(['inventory', 'product-group', 'form']);
  }

  edit(group: ProductGroup): void{
    this.productServ.selectedEntity = group;
    this.router.navigate(['inventory','product-group', 'form']);
  }

  delete(group: ProductGroup): void {
    this.confirmDelete.open('Grupo de producto', group.description , {}).subscribe(r => {
      if (r) {
        this.productServ.delete(group.id).subscribe(result => {
          if (result.status) {
            const index = this.dataSource.data.indexOf(group);
            this.dataSource.data.splice(index, 1);
            this.table.renderRows();
            this._snackBar.open('Eliminado correctamente', 'Cerrar', {duration: 3000});
          } else {
            this._snackBar.open(`No se pudo eliminar el grupo.\n\r${result.message}`, 'Cerrar', {duration: 3000});
          }
        })
      }
    });
  }
}
