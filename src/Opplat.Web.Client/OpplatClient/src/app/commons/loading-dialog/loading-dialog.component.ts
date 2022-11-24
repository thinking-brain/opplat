import { Component, Injectable } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-loading-dialog',
  templateUrl: './loading-dialog.component.html',
  styleUrls: ['./loading-dialog.component.sass']
})
@Injectable({
  providedIn: 'root'
})
export class LoadingDialogComponent{
  dialogRef: MatDialogRef<LoadingDialogComponent> | null = null;
  constructor(public dialog: MatDialog
  ) {}
  open(): void{
    this.dialogRef = this.dialog.open(LoadingDialogComponent, {
      width: '250px',
      disableClose: true,
      data: {},
    })
  }
  close(): void{
    this.dialogRef?.close();
  }
}
