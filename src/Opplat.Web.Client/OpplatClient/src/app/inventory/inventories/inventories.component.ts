import { AfterViewInit, Component, Input, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTable } from '@angular/material/table';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, of } from 'rxjs';
import { LoadingDialogComponent } from 'src/app/commons/loading-dialog/loading-dialog.component';
import { Inventory } from '../entities/inventory';
import { InventoryService } from '../services/inventory.service';
import { InventoriesDataSource } from './inventories-datasource';

@Component({
  selector: 'app-inventories',
  templateUrl: './inventories.component.html',
  styleUrls: ['./inventories.component.sass']
})
export class InventoriesComponent implements AfterViewInit {
  title = 'Inventarios';
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatTable) table!: MatTable<Inventory>;

  @Input('storage-id') storageId: BehaviorSubject<string> = new BehaviorSubject("undefined");

  /** Columns displayed in the table. Columns IDs can be added, removed, or reordered. */
  displayedColumns = ['product', 'quatity', 'unit'];



  constructor(public dataSource: InventoriesDataSource, private router: Router, private service: InventoryService,
    private loading: LoadingDialogComponent,private _snackBar: MatSnackBar) {
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
        return of([] as Inventory[]);
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

  movements(inventory: Inventory): void{
  }
}
