import { TestBed } from '@angular/core/testing';

import { ProductMovementsService, MovementTypesService } from './movements.service';

describe('ProductMovementsService', () => {
  let service: ProductMovementsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProductMovementsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

describe('MovementTypesService', () => {
  let service: MovementTypesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MovementTypesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
