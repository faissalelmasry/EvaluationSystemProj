import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Assignmentform } from './assignmentform';

describe('Assignmentform', () => {
  let component: Assignmentform;
  let fixture: ComponentFixture<Assignmentform>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Assignmentform],
    }).compileComponents();

    fixture = TestBed.createComponent(Assignmentform);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
