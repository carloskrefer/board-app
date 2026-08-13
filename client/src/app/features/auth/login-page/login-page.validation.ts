import { email, FieldTree, required, schema, TreeValidationResult, ValidationError } from "@angular/forms/signals";
import { Login } from "./login-page.models";
import { HttpErrorResponse, HttpStatusCode } from "@angular/common/http";
import { isApiProblemDetails } from "../../../core/api/api-problem-details/api-problem-details.helper";
import { unknown } from "../../../core/form/validation-errors";

export const logInSchema = schema<Login>((path) => {
    email(path.email, { message: 'Expected pattern: user@domain.com' });
    required(path.email, { message: 'Email is required' });
    required(path.password, { message: 'Password is required' });
});

export function toValidationError(
    response: unknown,
    form: FieldTree<Login>): TreeValidationResult<ValidationError.WithOptionalFieldTree> {
    const isCorrectType = response instanceof HttpErrorResponse && isApiProblemDetails(response.error);
    if (!isCorrectType)
        return unknown;

    var body = response.error;

    if (response.status == HttpStatusCode.BadRequest) {
        if (body.errors.length) {
            return body.errors.map(error => {
                switch (error.field) {
                    case 'email': return { kind: 'emailError', message: error.message, fieldTree: form.email };
                    case 'password': return { kind: 'passwordError', message: error.message, fieldTree: form.password };
                    default: return { kind: 'generalError', message: error.message };
                }
            });
        }
        return { kind: 'credentialsError', message: body.detail };
    }

    return unknown;
}