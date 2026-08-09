import { Component, computed, input } from '@angular/core';
import { FieldTree, FormField } from '@angular/forms/signals';
import { Label } from '../../label/label';
import { ErrorList } from '../../error-list/error-list';

@Component({
    selector: 'app-email-field',
    imports: [FormField, Label, ErrorList],
    templateUrl: './email-field.html',
    styleUrl: './email-field.scss',
})
export class EmailField {
    formField = input.required<FieldTree<string>>();

    showError = computed(() => {
        const touched = this.formField()().touched();
        const invalid = this.formField()().invalid();
        return invalid && touched;
    });
}