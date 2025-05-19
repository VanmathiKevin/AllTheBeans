import { TestBed } from '@angular/core/testing';

import { BeanService } from './bean.service';

describe('BeanService', () => {
  let service: BeanService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BeanService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
