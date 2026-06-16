import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EvaluationResultComponent } from './evaluation-result-component';

describe('EvaluationResultComponent', () => {
  let component: EvaluationResultComponent;
  let fixture: ComponentFixture<EvaluationResultComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EvaluationResultComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(EvaluationResultComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
