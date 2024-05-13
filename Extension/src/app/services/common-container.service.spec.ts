import { TestBed } from '@angular/core/testing';

import { CommonContainerService } from './common-container.service';

describe('CommonContainerService', () => {
  let service: CommonContainerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CommonContainerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
