import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Pendingassignment } from './pendingassignment';

describe('Pendingassignment', () => {
  let component: Pendingassignment;
  let fixture: ComponentFixture<Pendingassignment>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Pendingassignment],
    }).compileComponents();

    fixture = TestBed.createComponent(Pendingassignment);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
