import { Component, inject, signal } from '@angular/core';
import { Logo } from '../../../shared/symbols/logo/logo';
import { email, FieldTree, form, FormRoot, maxLength, minLength, pattern, required, TreeValidationResult, validate, ValidationError } from '@angular/forms/signals';
import { EmailField } from '../../../shared/forms/fields/email-field/email-field';
import { PasswordField } from '../../../shared/forms/fields/password-field/password-field';
import { AuthService } from '../../../core/auth/auth-service/auth-service';
import { firstValueFrom } from 'rxjs';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { isApiProblemDetails } from '../../../core/api/api-problem-details/api-problem-details.helper';
import { Banner } from '../../../shared/banner/banner';
import { Button } from '../../../shared/buttons/button/button';
import { NameField } from '../../../shared/forms/fields/name-field/name-field';
import { Router } from '@angular/router';
import { Snackbar, SnackbarService, SnackbarTypeEnum } from '../../../core/snackbar/snackbar.service';

class Signin {
    name = '';
    email = '';
    password = '';
    passwordConfirm = '';
}

const SIGNIN_INITIAL: Signin = {
    name: '',
    email: '',
    password: '',
    passwordConfirm: '',
};

@Component({
    selector: 'app-signin-page',
    imports: [Logo, EmailField, PasswordField, FormRoot, Button, Banner, NameField],
    templateUrl: './signin-page.html',
    styleUrl: './signin-page.scss',
})
export class SigninPage {
    auth = inject(AuthService);
    router = inject(Router);
    snackbar = inject(SnackbarService);
    
    signinModel = signal<Signin>(SIGNIN_INITIAL);

    signinForm = form<Signin>(
        this.signinModel,
        path => {
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
        },
        {
            submission: {
                action: this.signin.bind(this),
                ignoreValidators: 'pending',
                onInvalid: (field) => this.focusOnFirstInvalidField(field),
            }
        }
    );

    focusOnFirstInvalidField(field: FieldTree<Signin>): void {
        const errors = field().errorSummary();

        if (errors.length)
            errors[0].fieldTree().focusBoundControl();
    }

    async signin(): Promise<TreeValidationResult<ValidationError.WithOptionalFieldTree>> {
        try {
            await firstValueFrom(this.auth.signin(this.signinModel()));
            this.addAccountCreatedSnackbar();
            this.navigateToLoginPage();
            return;
        } catch (response) {
            const isCorrectType = response instanceof HttpErrorResponse && isApiProblemDetails(response.error);
            if (!isCorrectType) 
                return { kind: 'unkown', message: 'An unkown error has occurred. Please try again later.' };

            var body = response.error;
            
            if (response.status == HttpStatusCode.BadRequest) {
                if (body.errors.length) {
                    return body.errors.map(error => {
                        switch (error.field) {
                            case 'name': return { kind: 'nameError', message: error.message, fieldTree: this.signinForm.name };
                            case 'email': return { kind: 'emailError', message: error.message, fieldTree: this.signinForm.email };
                            case 'password': return { kind: 'passwordError', message: error.message, fieldTree: this.signinForm.password };
                            default: return { kind: 'generalError', message: error.message };
                        }
                    });
                }
            }

            return { kind: 'serverError', message: 'An unkown error has occurred. Please try again later.' };
        }
    }

    navigateToLoginPage() {
        this.router.navigate(['/login']);
    }

    addAccountCreatedSnackbar() {
        let snack = new Snackbar();
        snack.type = SnackbarTypeEnum.Success;
        snack.text = 'Account created successfully!';
        this.snackbar.addSnackbar(snack);
    }
}
