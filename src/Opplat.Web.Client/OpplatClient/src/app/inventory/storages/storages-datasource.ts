import { Injectable } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { Warehouse } from '../entities/warehouse';
import { StorageService } from '../services/storage.service';

/**
 * Data source for the Products view. This class should
 * encapsulate all logic for fetching and manipulating the displayed data
 * (including sorting, pagination, and filtering).
 */
 @Injectable({
  providedIn: 'root'
})
export class StoragesDataSource extends MatTableDataSource<Warehouse> {

  constructor(private service: StorageService) {
    super();
    service.list().subscribe(d => {this.data = d});
  }

  /**
   * Paginate the data (client-side). If you're using server-side pagination,
   * this would be replaced by requesting the appropriate data from the server.
   */
  private getPagedData(data: Warehouse[]): Warehouse[] {
    if (this.paginator) {
      const startIndex = this.paginator.pageIndex * this.paginator.pageSize;
      return data.splice(startIndex, this.paginator.pageSize);
    } else {
      return data;
    }
  }

  /**
   * Sort the data (client-side). If you're using server-side sorting,
   * this would be replaced by requesting the appropriate data from the server.
   */
  private getSortedData(data: Warehouse[]): Warehouse[] {
    if (!this.sort || !this.sort.active || this.sort.direction === '') {
      return data;
    }

    return data.sort((a, b) => {
      const isAsc = this.sort?.direction === 'asc';
      switch (this.sort?.active) {
        case 'description': return compare(a.description, b.description, isAsc);
        case 'code': return compare(+a.code, +b.code, isAsc);
        default: return 0;
      }
    });
  }
}

/** Simple sort comparator for example ID/Name columns (for client-side sorting). */
function compare(a: string | number, b: string | number, isAsc: boolean): number {
  return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
}
