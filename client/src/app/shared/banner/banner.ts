import { Component, input } from '@angular/core';

@Component({
  selector: 'app-banner',
  imports: [],
  templateUrl: './banner.html',
  styleUrl: './banner.scss',
})
export class Banner {
    message = input.required<string>();
    type = input<'info' | 'success' | 'warning' | 'error'>('info');
}
