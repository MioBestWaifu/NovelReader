import { TestBed } from '@angular/core/testing';

import { ExtensionCommunicationService } from './extension-communication.service';

describe('ExtensionCommunicationService', () => {
  let service: ExtensionCommunicationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ExtensionCommunicationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
