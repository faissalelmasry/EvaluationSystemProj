import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EvaluationForm } from './evaluation-form';

describe('EvaluationForm', () => {
  let component: EvaluationForm;
  let fixture: ComponentFixture<EvaluationForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EvaluationForm],
    }).compileComponents();

    fixture = TestBed.createComponent(EvaluationForm);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
