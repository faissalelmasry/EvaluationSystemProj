import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UpdateTemplate } from './update-template';

describe('UpdateTemplate', () => {
  let component: UpdateTemplate;
  let fixture: ComponentFixture<UpdateTemplate>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UpdateTemplate],
    }).compileComponents();

    fixture = TestBed.createComponent(UpdateTemplate);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
