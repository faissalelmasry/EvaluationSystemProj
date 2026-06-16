import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyEvaluations } from './my-evaluations';

describe('MyEvaluations', () => {
  let component: MyEvaluations;
  let fixture: ComponentFixture<MyEvaluations>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyEvaluations],
    }).compileComponents();

    fixture = TestBed.createComponent(MyEvaluations);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
