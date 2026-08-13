import { TreeValidationResult, ValidationError } from "@angular/forms/signals";

type Result = TreeValidationResult<ValidationError.WithOptionalFieldTree>;

export const unknown: Result = { kind: 'unkown', message: 'An unkown error has occurred. Please try again later.' };