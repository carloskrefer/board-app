import { FieldTree } from "@angular/forms/signals";

export function focusOnFirstInvalidField<T>(field: FieldTree<T>): void {
    const errors = field().errorSummary();

    if (errors.length)
        errors[0].fieldTree().focusBoundControl();
}