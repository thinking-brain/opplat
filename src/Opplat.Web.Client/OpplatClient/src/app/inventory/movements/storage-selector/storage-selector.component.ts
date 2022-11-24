import { Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Warehouse } from '../../entities/warehouse';
import { MovementTypesService } from '../../services/movements.service';
import { StorageService } from '../../services/storage.service';

@Component({
  selector: 'app-storage-selector',
  templateUrl: './storage-selector..component.html',
  styleUrls: ['./storage-selector..component.sass']
})
export class ProductMovementStorageSelectorComponent implements OnInit {
  title = '';
  storages: Warehouse[] = [];
  selectedStorage:string = 'undefined';
  storageId: BehaviorSubject<string> = new BehaviorSubject('undefined');

  constructor(public storageService: StorageService,
      public typeService: MovementTypesService) {
      storageService.list().subscribe(r => {
        this.storages = r;
      });
  }

  ngOnInit(): void {
    this.title = 'Movimientos de productos'
  }

  onChangeStorage(): void{
    this.storageId.next(this.selectedStorage);
  }
}
