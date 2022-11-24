import { Component, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Warehouse } from '../../entities/warehouse';
import { MovementTypesService } from '../../services/movements.service';
import { StorageService } from '../../services/storage.service';

@Component({
  selector: 'app-inventory-storage-selector',
  templateUrl: './storage-selector.component.html',
  styleUrls: ['./storage-selector.component.sass']
})
export class InventoriesStorageSelectorComponent implements OnInit {
  title = '';
  storages: Warehouse[] = [];
  selectedStorage:string = 'undefined';
  storageId: BehaviorSubject<string> = new BehaviorSubject('undefined');

  constructor(public storageService: StorageService) {
      storageService.list().subscribe(r => {
        this.storages = r;
      });
  }

  ngOnInit(): void {
    this.title = 'Inventarios'
  }

  onChangeStorage(): void{
    this.storageId.next(this.selectedStorage);
  }
}
