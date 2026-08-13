import { email, FieldTree, maxLength, minLength, pattern, required, schema, TreeValidationResult, validate, ValidationError } from "@angular/forms/signals";
import { Signin } from "./signin-page.models";
import { HttpErrorResponse, HttpStatusCode } from "@angular/common/http";
import { isApiProblemDetails } from "../../../core/api/api-problem-details/api-problem-details.helper";
import { unknown } from "../../../core/form/validation-errors";

export const signInSchema = schema<Signin>((path) => {
    required(path.name, { message: 'Name is required.' });
    maxLength(path.name, 100, { message: 'Name must have less than 100 characters.' });
    pattern(path.name, /^.*[a-zA-Z0-9 ]+.*$/, { message: 'Name can not contain only special characters.' });
    email(path.email, { message: 'Expected pattern: user@domain.com' });
    required(path.email, { message: 'Email is required.' });
    required(path.password, { message: 'Password is required.' });
    maxLength(path.password, 100, { message: 'Password must not have more than 100 characters.' });
    minLength(path.password, 6, { message: 'Password must not have less than 6 characters' });
    pattern(path.password, /^.*\d+.*$/, { message: 'Password must contain at least one number.' });
    pattern(path.password, /^.*[!@#$%¢¨¬&*\(\)\-_=+§´`\[\{\}\]\}~^/?;:.>,<\\|]+.*$/, { message: 'Password must contain at least one special character.' });
    pattern(path.password, /^.*(([a-z]+.*[A-Z]+)|([A-Z]+.*[a-z]+))+.*$/, { message: 'Password must contain at least one uppercase and one lowercase letter.' });
    required(path.passwordConfirm, { message: 'Password must be repeated in this field.' });
    validate(path.passwordConfirm, (ctx) => {
        if (ctx.value() != ctx.valueOf(path.password))
            return { kind: 'passwordConfirmError', message: 'Password must be the same as in the previous field.' };
        return;
    });
});

export function toValidationError(
    response: unknown, 
    form: FieldTree<Signin>): TreeValidationResult<ValidationError.WithOptionalFieldTree> {
    const isCorrectType = response instanceof HttpErrorResponse && isApiProblemDetails(response.error);

    if (!isCorrectType)
        return unknown;

    var body = response.error;

    if (response.status == HttpStatusCode.BadRequest) {
        if (body.errors.length) {
            return body.errors.map(error => {
                switch (error.field) {
                    case 'name': return { kind: 'nameError', message: error.message, fieldTree: form.name };
                    case 'email': return { kind: 'emailError', message: error.message, fieldTree: form.email };
                    case 'password': return { kind: 'passwordError', message: error.message, fieldTree: form.password };
                    default: return { kind: 'generalError', message: error.message };
                }
            });
        }
    }

    return unknown;
}