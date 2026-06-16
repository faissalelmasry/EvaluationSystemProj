import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReviewDashboard } from './review-dashboard';

describe('ReviewDashboard', () => {
  let component: ReviewDashboard;
  let fixture: ComponentFixture<ReviewDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReviewDashboard],
    }).compileComponents();

    fixture = TestBed.createComponent(ReviewDashboard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
