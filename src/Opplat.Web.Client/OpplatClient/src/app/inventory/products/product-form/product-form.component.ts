import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Route, Router } from '@angular/router';
import { LoadingDialogComponent } from 'src/app/commons/loading-dialog/loading-dialog.component';
import { Product } from '../../entities/product';
import { ProductGroup, UnitOfMeasurement } from '../../entities/product-clasification';
import { ProductGroupService } from '../../services/product-group.service';
import { ProductsService } from '../../services/products.service';
import { UnitService } from '../../services/unit.service';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.sass']
})
export class ProductFormComponent implements OnInit {
  productForm = this.fb.group({
    code: '',
    name: ['', Validators.required],
    description: '',
    itsInventoriable: [true, Validators.required],
    productGroup: [{}, Validators.required],
    unitOfMeasurement: [{}, Validators.required],
    containerQuantity: [1, Validators.required],
    shrinkRatio: [0, Validators.required]
  });

  title = ''
  groups: ProductGroup[] = [];
  units: UnitOfMeasurement[] = [];

  constructor(private fb: FormBuilder, unitServ: UnitService, groupServ: ProductGroupService,
      public productServ: ProductsService, private router:Router,
      private loading: LoadingDialogComponent, private _snackBar: MatSnackBar) {
    groupServ.list().subscribe(p => {
      this.groups = p
    });
    unitServ.list().subscribe(p => {
      this.units = p
    });
  }

  ngOnInit(): void {
    if (this.productServ.selectedEntity === null) {
      this.title = 'Nuevo producto';
    }else{
      this.title = 'Editar producto'
      const toEdit = this.productServ.selectedEntity;
      this.productForm.controls['name'].setValue(toEdit.name);
      this.productForm.controls['description'].setValue(toEdit.description);
      this.productForm.controls['code'].setValue(toEdit.code);
      this.productForm.controls['itsInventoriable'].setValue(toEdit.itsInventoriable);
      this.productForm.controls['productGroup'].setValue(toEdit.groupId);
      this.productForm.controls['unitOfMeasurement'].setValue(toEdit.unit);
      this.productForm.controls['shrinkRatio'].setValue(toEdit.shrinkRatio);
      this.productForm.controls['containerQuantity'].setValue(toEdit.containerQuantity);
    }
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      this.loading.open();
      if (this.productServ.selectedEntity !== null) {
        this.productServ.selectedEntity.code = this.productForm.controls['code'].value as string;
        this.productServ.selectedEntity.name = this.productForm.controls['name'].value as string;
        this.productServ.selectedEntity.description = this.productForm.controls['description'].value as string;
        this.productServ.selectedEntity.itsInventoriable = this.productForm.controls['itsInventoriable'].value as boolean;
        this.productServ.selectedEntity.groupId = this.productForm.controls['productGroup'].value as number;
        this.productServ.selectedEntity.unit = this.productForm.controls['unitOfMeasurement'].value as string;
        this.productServ.selectedEntity.shrinkRatio = this.productForm.controls['shrinkRatio'].value as number;
        this.productServ.selectedEntity.containerQuantity = this.productForm.controls['containerQuantity'].value as number;
        this.productServ.put(this.productServ.selectedEntity).subscribe(r => {
          this.loading.close();
          if (r.status) {
            this.router.navigate(['inventory', 'products']);
          }else{
            this._snackBar.open(r.message, 'Close');
          }
        });
      } else {
        this.productServ.post({
          code: this.productForm.controls['code'].value,
          name: this.productForm.controls['name'].value,
          description: this.productForm.controls['description'].value,
          itsInventoriable: this.productForm.controls['itsInventoriable'].value,
          groupId: this.productForm.controls['productGroup'].value,
          unit: this.productForm.controls['unitOfMeasurement'].value,
          shrinkRatio: this.productForm.controls['shrinkRatio'].value,
          containerQuantity: this.productForm.controls['containerQuantity'].value,
        } as Product).subscribe(r => {
          this.loading.close();
          if (r.status) {
            this.router.navigate(['inventory', 'products']);
          }else{
            this._snackBar.open(r.message, 'Close', { duration: 3000});
          }
        });
      }
    }
  }
}
