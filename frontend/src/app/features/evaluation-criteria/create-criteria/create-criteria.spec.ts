import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateCriteria } from './create-criteria';

describe('CreateCriteria', () => {
  let component: CreateCriteria;
  let fixture: ComponentFixture<CreateCriteria>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateCriteria],
    }).compileComponents();

    fixture = TestBed.createComponent(CreateCriteria);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
