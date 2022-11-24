import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { LoadingDialogComponent } from 'src/app/commons/loading-dialog/loading-dialog.component';
import { ProductClassification, ProductGroup } from '../../entities/product-clasification';
import { ProductClassificationService } from '../../services/product-classification.service';
import { ProductGroupService } from '../../services/product-group.service';

@Component({
  selector: 'app-product-group-form',
  templateUrl: './product-group-form.component.html',
  styleUrls: ['./product-group-form.component.sass']
})
export class ProductGroupFormComponent implements OnInit {
  productForm = this.fb.group({
    description: ['', Validators.required],
    productClassification: [{}, Validators.required],
  });

  title = ''
  classifications: ProductClassification[] = [];

  constructor(private fb: FormBuilder, classificationServ: ProductClassificationService,
      public productServ: ProductGroupService, private router:Router, private loading: LoadingDialogComponent,
      private _snackBar: MatSnackBar) {
        classificationServ.list().subscribe(p => {
      this.classifications = p
    });
  }

  ngOnInit(): void {
    if (this.productServ.selectedEntity === null) {
      this.title = 'Nuevo grupo de producto';
    }else{
      this.title = 'Editar grupo de producto'
      const toEdit = this.productServ.selectedEntity;
      this.productForm.controls['description'].setValue(toEdit.description);
      this.productForm.controls['productClassification'].setValue(toEdit.classification.id);
    }
  }

  onSubmit(): void {
    if (this.productForm.valid) {
      this.loading.open();
      if (this.productServ.selectedEntity !== null) {
        this.productServ.selectedEntity.description = this.productForm.controls['description'].value as string;
        this.productServ.selectedEntity.classificationId = this.productForm.controls['productClassification'].value as number;
        this.productServ.put(this.productServ.selectedEntity).subscribe(r => {
          this.loading.close();
          if (r.status) {
            this.router.navigate(['inventory', 'product-groups']);
          }else{
            this._snackBar.open(r.message, 'Close');
          }
        });
      } else {
        this.productServ.post({
          description: this.productForm.controls['description'].value,
          classificationId: this.productForm.controls['productClassification'].value
        } as ProductGroup).subscribe(r => {
          this.loading.close();
          if (r.status) {
            this.router.navigate(['inventory', 'product-groups']);
          }else{
            this._snackBar.open(r.message, 'Close', { duration: 3000});
          }
        });
      }
    }
  }
}
