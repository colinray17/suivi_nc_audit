import { TestBed } from '@angular/core/testing';
import { NonConformites } from './non-conformites';

describe('NonConformites', () => {
  let service: NonConformites;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NonConformites);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
