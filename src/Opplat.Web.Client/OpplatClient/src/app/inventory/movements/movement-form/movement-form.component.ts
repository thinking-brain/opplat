import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { LoadingDialogComponent } from 'src/app/commons/loading-dialog/loading-dialog.component';
import { MovementType, ProductMovement } from '../../entities/movements';
import { Product } from '../../entities/product';
import { UnitOfMeasurement } from '../../entities/product-clasification';
import { Warehouse } from '../../entities/warehouse';
import { MovementTypesService, ProductMovementsService } from '../../services/movements.service';
import { ProductsService } from '../../services/products.service';
import { StorageService } from '../../services/storage.service';
import { UnitService } from '../../services/unit.service';

@Component({
  selector: 'app-product-movement-form',
  templateUrl: './movement-form.component.html',
  styleUrls: ['./movement-form.component.sass']
})
export class ProductMovementFormComponent implements OnInit {
  productForm = this.fb.group({
    storageId: ['', Validators.required],
    productId: ['', Validators.required],
    type: [0, Validators.required],
    quantity: [1, Validators.required],
    unit: ['', Validators.required],
    observations: ['', Validators.required],
  });

  title = '';
  storages: Warehouse[] = [];
  products: Product[] = [];
  types: MovementType[] = [];
  units: UnitOfMeasurement[] = [];
  filteredUnits: UnitOfMeasurement[] = [];

  constructor(private fb: FormBuilder, public service: ProductMovementsService,private unitServ: UnitService,
    public storageService: StorageService,public typeService: MovementTypesService, private prodServ: ProductsService,
    private router: Router, private loading: LoadingDialogComponent, private _snackBar: MatSnackBar) {
      storageService.list().subscribe(r => {
        this.storages = r;
      });
      prodServ.list().subscribe(r => {
        this.products = r;
      });
      typeService.list().subscribe(r => {
        this.types = r;
      });
      unitServ.list().subscribe(r => {
        this.units = r;
        this.filteredUnits = r;
      });
  }

  ngOnInit(): void {
    if (this.service.selectedEntity === null) {
      this.title = 'Nuevo movimiento de producto';
    } else {
      this.title = 'Editar movimiento de producto'
      const toEdit = this.service.selectedEntity;
      this.productForm.controls['storageId'].setValue(toEdit.storageId);
      this.productForm.controls['productId'].setValue(toEdit.productId);
      this.productForm.controls['type'].setValue(toEdit.type);
      this.productForm.controls['quantity'].setValue(toEdit.quantity);
      this.productForm.controls['unit'].setValue(toEdit.unit);
      this.productForm.controls['observations'].setValue(toEdit.observations);
    }
  }

  onProductChange(): void{
    const prodId = this.productForm.controls['productId'].value;
    const prod = this.products.find(r => {
      return r.id === prodId;
    });
    const prodUnit = this.units.find(u => {
      return u.abbreviation === prod?.unit;
    })
    this.filteredUnits = this.units.filter(u => {
        return u.unitType === prodUnit?.unitType;
    });
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      this.loading.open();
      if (this.service.selectedEntity !== null) {
        this.service.selectedEntity.storageId = this.productForm.controls['storageId'].value as string;
        this.service.selectedEntity.productId = this.productForm.controls['productId'].value as string;
        this.service.selectedEntity.type = this.productForm.controls['type'].value as number;
        this.service.selectedEntity.quantity = this.productForm.controls['quantity'].value as number;
        this.service.selectedEntity.unit = this.productForm.controls['unit'].value as string;
        this.service.selectedEntity.observations = this.productForm.controls['observations'].value as string;
        this.service.put(this.service.selectedEntity).subscribe(r => {
          this.loading.close();
          if (r.status) {
            this.router.navigate(['inventory', 'product-movements']);
          } else {
            this._snackBar.open(r.message, 'Close');
          }
        });
      } else {
        this.service.post({
          storageId: this.productForm.controls['storageId'].value,
          productId: this.productForm.controls['productId'].value,
          type: this.productForm.controls['type'].value,
          quantity: this.productForm.controls['quantity'].value,
          unit: this.productForm.controls['unit'].value,
          observations: this.productForm.controls['observations'].value,
        } as ProductMovement).subscribe(r => {
          this.loading.close();
          if (r.status) {
            this.router.navigate(['inventory', 'product-movements']);
          } else {
            this._snackBar.open(r.message, 'Close', { duration: 3000 });
          }
        });
      }
    }
  }
}
