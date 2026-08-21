import { Component, input } from '@angular/core';
import { Icon } from '../../symbols/icon/icon';
import { IconType } from '../../symbols/icon/icon-type';

@Component({
  selector: 'app-icon-button',
  imports: [Icon],
  templateUrl: './icon-button.html',
  styleUrl: './icon-button.scss',
})
export class IconButton {
    icon = input<IconType>();
}