import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VisibilityButton } from './visibility-button';

describe('VisibilityButton', () => {
  let component: VisibilityButton;
  let fixture: ComponentFixture<VisibilityButton>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VisibilityButton],
    }).compileComponents();

    fixture = TestBed.createComponent(VisibilityButton);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
