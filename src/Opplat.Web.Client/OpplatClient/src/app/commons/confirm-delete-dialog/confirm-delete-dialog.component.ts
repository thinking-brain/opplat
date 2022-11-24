import { Component, Inject, Injectable } from '@angular/core';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Observable } from 'rxjs';

export interface DeleteDialogData {
  entity: string;
  description: string;
  properties: {};
}

@Component({
  selector: 'app-confirm-delete-dialog',
  templateUrl: './confirm-delete-dialog.component.html',
  styleUrls: ['./confirm-delete-dialog.component.sass']
})
@Injectable({
  providedIn: 'root'
})
export class ConfirmDeleteDialogComponent {
  dialogRef: MatDialogRef<ConfirmDeleteDialogComponent> | null = null;
  constructor(public dialog: MatDialog, @Inject(MAT_DIALOG_DATA) public data: DeleteDialogData
  ) { }
  open(entity: string, description: string, properties: any): Observable<any> {
    this.dialogRef = this.dialog.open(ConfirmDeleteDialogComponent, {
      width: '450px',
      data: {entity: entity, description: description, properties: properties},
    })
    return this.dialogRef.afterClosed();
  }
}
