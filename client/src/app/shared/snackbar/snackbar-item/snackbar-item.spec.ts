import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SnackbarItem } from './snackbar-item';

describe('SnackbarItem', () => {
  let component: SnackbarItem;
  let fixture: ComponentFixture<SnackbarItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SnackbarItem],
    }).compileComponents();

    fixture = TestBed.createComponent(SnackbarItem);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
