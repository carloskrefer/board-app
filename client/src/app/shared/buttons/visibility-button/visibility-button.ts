import { Component, input, model } from '@angular/core';

@Component({
  selector: 'app-visibility-button',
  imports: [],
  templateUrl: './visibility-button.html',
  styleUrl: './visibility-button.scss',
})
export class VisibilityButton {
    isVisible = model.required<boolean>();
    isInsideInput = input(false);
}