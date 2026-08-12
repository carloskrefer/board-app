import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SnackbarList } from './snackbar-list';

describe('SnackbarList', () => {
  let component: SnackbarList;
  let fixture: ComponentFixture<SnackbarList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SnackbarList],
    }).compileComponents();

    fixture = TestBed.createComponent(SnackbarList);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
