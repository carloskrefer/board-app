import { Component, computed, input } from '@angular/core';
import { Spinner } from '../../symbols/spinner/spinner';
import { IconType } from '../../symbols/icon/icon-type';
import { Icon } from '../../symbols/icon/icon';

@Component({
  selector: 'app-button',
  imports: [Spinner, Icon],
  templateUrl: './button.html',
  styleUrl: './button.scss',
})
export class Button {
    label = input.required<string>();
    type = input<'button' | 'submit'>('button');
    mainColor = input<'primary' | 'secondary' | 'error' | 'gray-medium' | 'gray-light'>('primary');
    mode = input<'fill' | 'outline'>('fill');
    size = input<'small' | 'medium' | 'large'>('medium');
    icon = input<IconType>();
    borderHidden = input<boolean>(false);
    isLoading = input<boolean>(false);
    widthType = input<'auto-width' | 'full-width'>('full-width');
    hoverEffect = input<'underline' | 'background-color'>('background-color');
    alignment = input<'start' | 'center' | 'end'>('center');
    paddingHorizontal = input<'none' | 'small'>('small');

    classes = computed(() => ([
        `${this.mainColor()}-main-color`,
        this.mode(),
        this.size(),
        this.borderHidden() ? 'border-hidden' : '',
        this.widthType(),
        `hover-${this.hoverEffect()}`,
        `justify-content-${this.alignment()}`,
        `padding-horizontal-${this.paddingHorizontal()}`
    ]));

    spinnerColor = computed(() => {
        let color = this.mainColor();
        let mode = this.mode();
        
        if (mode == 'fill')
            return color === 'primary' ? 'on-primary' : 'on-secondary';

        if (color === 'primary' || color === 'secondary' || color === 'error')
            return color;

        return 'primary';
    });
}
