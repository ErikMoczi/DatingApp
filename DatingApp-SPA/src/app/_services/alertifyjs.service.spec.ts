import {TestBed, inject} from '@angular/core/testing';

import {AlertifyjsService} from './alertifyjs.service';

describe('AlertifyjsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AlertifyjsService]
    });
  });

  it('should be created', inject([AlertifyjsService], (service: AlertifyjsService) => {
    expect(service).toBeTruthy();
  }));
});
