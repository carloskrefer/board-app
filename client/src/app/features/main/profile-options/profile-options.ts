import { Component, output } from '@angular/core';
import { Button } from '../../../shared/buttons/button/button';

@Component({
  selector: 'app-profile-options',
  imports: [Button],
  templateUrl: './profile-options.html',
  styleUrl: './profile-options.scss',
})
export class ProfileOptions {
    logout = output();
    openProfile = output();
    close = output();
}
