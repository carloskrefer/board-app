import { Component, computed, input } from '@angular/core';
import { FieldTree, FormField } from '@angular/forms/signals';
import { ErrorList } from '../../error-list/error-list';
import { Label } from '../../label/label';

@Component({
    selector: 'app-name-field',
    imports: [FormField, Label, ErrorList],
    templateUrl: './name-field.html',
    styleUrl: './name-field.scss',
})
export class NameField {
    formField = input.required<FieldTree<string>>();

    showError = computed(() => {
        const touched = this.formField()().touched();
        const invalid = this.formField()().invalid();
        return invalid && touched;
    });
}
