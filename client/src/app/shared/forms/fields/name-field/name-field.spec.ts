import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NameField } from './name-field';

describe('NameField', () => {
  let component: NameField;
  let fixture: ComponentFixture<NameField>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NameField],
    }).compileComponents();

    fixture = TestBed.createComponent(NameField);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
