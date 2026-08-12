import { Component, computed, effect, input, signal } from '@angular/core';

export type ProgressBarColor = 'default' | 'info' | 'success' | 'warning' | 'error';

@Component({
    selector: 'app-progress-bar',
    imports: [],
    templateUrl: './progress-bar.html',
    styleUrl: './progress-bar.scss',
})
export class ProgressBar {
    current = input<number>(0);
    maximum = input<number>(100);
    transitionDuration = input<number>(1000);
    color = input<ProgressBarColor>('default');

    started = signal(false);

    percentage = computed(() => {
        if (!this.started())
            return 0;

        return (this.current() / this.maximum()) * 100;
    });

    isLightBackground = true;

    styles = computed(() => {
        return {
            width: `${this.percentage()}%`,
            transitionDuration: `${this.transitionDuration()}ms`
        };
    });

    constructor() {
        setTimeout(() => this.started.set(true), 0);
    }
}
