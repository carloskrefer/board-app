import { Component, input } from '@angular/core';

@Component({
  selector: 'app-logo',
  imports: [],
  templateUrl: './logo.html',
  styleUrl: './logo.scss',
})
export class Logo {
    size = input<'small' | 'medium' | 'large'>('large');
    color = input<'primary' | 'gray-lighter'>('primary');
}
