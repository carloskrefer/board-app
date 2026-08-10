import { Component, computed, input, signal } from '@angular/core';
import { FieldTree, FormField } from '@angular/forms/signals';
import { Label } from '../../label/label';
import { ErrorList } from '../../error-list/error-list';
import { VisibilityButton } from '../../../buttons/visibility-button/visibility-button';

@Component({
  selector: 'app-password-field',
  imports: [FormField, Label, ErrorList, VisibilityButton],
  templateUrl: './password-field.html',
  styleUrl: './password-field.scss',
})
export class PasswordField {
    formField = input.required<FieldTree<string>>();
    label = input('Password');

    showError = computed(() => {
        const touched = this.formField()().touched();
        const invalid = this.formField()().invalid();
        return invalid && touched;
    });

    isVisible = signal(false);

    type = computed(() => this.isVisible() ? 'text' : 'password');
}
