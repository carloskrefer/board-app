import { Component } from '@angular/core';
import { Logo } from '../../../shared/symbols/logo/logo';
import { ProfileButton } from '../profile-button/profile-button';
import { IconButton } from '../../../shared/buttons/icon-button/icon-button';

@Component({
  selector: 'app-main-header',
  imports: [Logo, ProfileButton, IconButton],
  templateUrl: './main-header.html',
  styleUrl: './main-header.scss',
})
export class MainHeader {}
