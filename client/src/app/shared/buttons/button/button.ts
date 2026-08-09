import { Component, computed, input } from '@angular/core';
import { Spinner } from '../../symbols/spinner/spinner';

@Component({
  selector: 'app-button',
  imports: [Spinner],
  templateUrl: './button.html',
  styleUrl: './button.scss',
})
export class Button {
    label = input.required<string>();
    type = input<'button' | 'submit'>('button');
    mainColor = input<'primary' | 'secondary'>('primary');
    mode = input<'fill' | 'outline'>('fill');
    size = input<'small' | 'medium' | 'large'>('medium');
    borderHidden = input<boolean>(false);
    isLoading = input<boolean>(false);

    classes = computed(() => ([
        `${this.mainColor()}-main-color`,
        this.mode(),
        this.size(),
        this.borderHidden() ? 'border-hidden' : '',
    ]));

    spinnerColor = computed(() => {
        let color = this.mainColor();
        let mode = this.mode();
        
        if (mode == 'fill')
            return color === 'primary' ? 'on-primary' : 'on-secondary';

        return color;
    });
}
