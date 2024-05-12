import { TestBed } from '@angular/core/testing';

import { ServicesCommunicationService } from './service-communication.service';

describe('ServiceCommunicationService', () => {
  let service: ServicesCommunicationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ServicesCommunicationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
