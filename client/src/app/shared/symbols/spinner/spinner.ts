import { Component, input } from '@angular/core';

@Component({
  selector: 'app-spinner',
  imports: [],
  templateUrl: './spinner.html',
  styleUrl: './spinner.scss',
})
export class Spinner {
    color = input<'primary' | 'on-primary' | 'secondary' | 'on-secondary'>('primary');
}
