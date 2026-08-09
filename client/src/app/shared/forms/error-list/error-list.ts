import { Component, input } from '@angular/core';
import { FieldTree } from '@angular/forms/signals';

@Component({
  selector: 'app-error-list',
  imports: [],
  templateUrl: './error-list.html',
  styleUrl: './error-list.scss',
})
export class ErrorList {
    formField = input.required<FieldTree<string>>();
}
