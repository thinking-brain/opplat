import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { LoadingDialogComponent } from 'src/app/commons/loading-dialog/loading-dialog.component';
import { ProductGroup } from '../../entities/product-clasification';
import { Warehouse } from '../../entities/warehouse';
import { StorageService } from '../../services/storage.service';

@Component({
  selector: 'app-storage-form',
  templateUrl: './storage-form.component.html',
  styleUrls: ['./storage-form.component.sass']
})
export class StorageFormComponent implements OnInit {
  productForm = this.fb.group({
    description: ['', Validators.required],
    code: '',
    isCostCenter: [true, Validators.required],
  });

  title = '';

  constructor(private fb: FormBuilder, public productServ: StorageService,
    private router: Router, private loading: LoadingDialogComponent, private _snackBar: MatSnackBar) {

  }

  ngOnInit(): void {
    if (this.productServ.selectedEntity === null) {
      this.title = 'Nuevo almacen';
    } else {
      this.title = 'Editar almacen'
      const toEdit = this.productServ.selectedEntity;
      this.productForm.controls['description'].setValue(toEdit.description);
      this.productForm.controls['code'].setValue(toEdit.code);
      this.productForm.controls['isCostCenter'].setValue(toEdit.isCostCenter);
    }
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      this.loading.open();
      if (this.productServ.selectedEntity !== null) {
        this.productServ.selectedEntity.description = this.productForm.controls['description'].value as string;
        this.productServ.selectedEntity.code = this.productForm.controls['code'].value as string;
        this.productServ.selectedEntity.isCostCenter = this.productForm.controls['isCostCenter'].value as boolean;
        this.productServ.put(this.productServ.selectedEntity).subscribe(r => {
          this.loading.close();
          if (r.status) {
            this.router.navigate(['inventory', 'storages']);
          } else {
            this._snackBar.open(r.message, 'Close');
          }
        });
      } else {
        this.productServ.post({
          description: this.productForm.controls['description'].value,
          code: this.productForm.controls['code'].value,
          isCostCenter: this.productForm.controls['isCostCenter'].value
        } as Warehouse).subscribe(r => {
          this.loading.close();
          if (r.status) {
            this.router.navigate(['inventory', 'storages']);
          } else {
            this._snackBar.open(r.message, 'Close', { duration: 3000 });
          }
        });
      }
    }
  }
}
