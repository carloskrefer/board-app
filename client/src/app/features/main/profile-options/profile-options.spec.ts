import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProfileOptions } from './profile-options';

describe('ProfileOptions', () => {
  let component: ProfileOptions;
  let fixture: ComponentFixture<ProfileOptions>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProfileOptions],
    }).compileComponents();

    fixture = TestBed.createComponent(ProfileOptions);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
