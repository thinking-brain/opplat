import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { LoadingDialogComponent } from 'src/app/commons/loading-dialog/loading-dialog.component';
import { ProductClassification } from '../../entities/product-clasification';
import { ProductClassificationService } from '../../services/product-classification.service';

@Component({
  selector: 'app-product-classification-form',
  templateUrl: './product-classification-form.component.html',
  styleUrls: ['./product-classification-form.component.sass']
})
export class ProductClassificationFormComponent implements OnInit {
  productForm = this.fb.group({
    description: ['', Validators.required]
  });

  isEdit = false;
  title = ''

  constructor(private fb: FormBuilder,
    public productServ: ProductClassificationService, private router: Router, private loading: LoadingDialogComponent,
    private _snackBar: MatSnackBar) {

  }

  ngOnInit(): void {
    if (this.productServ.selectedEntity === null) {
      this.title = 'Nueva clasificacion';
    } else {
      this.title = 'Editar clasificacion'
      this.isEdit = true;
      const toEdit = this.productServ.selectedEntity;
      this.productForm.controls['description'].setValue(toEdit.description);
    }
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      this.loading.open();
      if (this.productServ.selectedEntity !== null) {
        this.productServ.selectedEntity.description = this.productForm.controls['description'].value as string;
        this.productServ.put(this.productServ.selectedEntity).subscribe(r => {
          this.loading.close();
          if (r.status) {
            this.router.navigate(['inventory', 'product-classifications']);
          }else{
            this._snackBar.open(r.message, 'Close');
          }
        });
      } else {
        this.productServ.post({
          description: this.productForm.controls['description'].value
        } as ProductClassification).subscribe(r => {
          this.loading.close();
          if (r.status) {
            this.router.navigate(['inventory', 'product-classifications']);
          }else{
            this._snackBar.open(r.message, 'Close', { duration: 3000});
          }
        });
      }
    }
  }
}
