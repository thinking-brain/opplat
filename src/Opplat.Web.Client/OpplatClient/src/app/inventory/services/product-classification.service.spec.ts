import { TestBed } from '@angular/core/testing';

import { ProductClassificationService } from './product-classification.service';

describe('ProductClassificationService', () => {
  let service: ProductClassificationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ProductClassificationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
