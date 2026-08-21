import { Component, input } from '@angular/core';
import { IconType } from './icon-type';

@Component({
  selector: 'app-icon',
  imports: [],
  templateUrl: './icon.html',
  styleUrl: './icon.scss',
})
export class Icon {
    icon = input<IconType>();
}
