import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Route, Router } from '@angular/router';
import { LoadingDialogComponent } from 'src/app/commons/loading-dialog/loading-dialog.component';
import { Product } from '../../entities/product';
import { ProductsService } from '../../services/products.service';

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
    price: [0, Validators.required],
    active: [true, Validators.required],
  });

  title = ''

  constructor(private fb: FormBuilder, public productServ: ProductsService, private router:Router,
      private loading: LoadingDialogComponent, private _snackBar: MatSnackBar) {
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
      this.productForm.controls['active'].setValue(toEdit.active);
      this.productForm.controls['price'].setValue(toEdit.price);
    }
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      this.loading.open();
      if (this.productServ.selectedEntity !== null) {
        this.productServ.selectedEntity.code = this.productForm.controls['code'].value as string;
        this.productServ.selectedEntity.name = this.productForm.controls['name'].value as string;
        this.productServ.selectedEntity.description = this.productForm.controls['description'].value as string;
        this.productServ.selectedEntity.active = this.productForm.controls['active'].value as boolean;
        this.productServ.selectedEntity.price = this.productForm.controls['price'].value as number;
        this.productServ.put(this.productServ.selectedEntity).subscribe(r => {
          this.loading.close();
          if (r.status) {
            this.router.navigate(['sales', 'products']);
          }else{
            this._snackBar.open(r.message, 'Close');
          }
        });
      } else {
        this.productServ.post({
          code: this.productForm.controls['code'].value,
          name: this.productForm.controls['name'].value,
          description: this.productForm.controls['description'].value,
          active: this.productForm.controls['active'].value,
          price: this.productForm.controls['price'].value,
        } as Product).subscribe(r => {
          this.loading.close();
          if (r.status) {
            this.router.navigate(['sales', 'products']);
          }else{
            this._snackBar.open(r.message, 'Close', { duration: 3000});
          }
        });
      }
    }
  }
}
