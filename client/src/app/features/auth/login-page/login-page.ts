import { Component, inject, signal } from '@angular/core';
import { apply, form, FormRoot, TreeValidationResult, ValidationError } from '@angular/forms/signals';
import { EmailField } from '../../../shared/forms/fields/email-field/email-field';
import { PasswordField } from '../../../shared/forms/fields/password-field/password-field';
import { AuthService } from '../../../core/auth/auth-service/auth-service';
import { firstValueFrom } from 'rxjs';
import { Banner } from '../../../shared/banner/banner';
import { Button } from '../../../shared/buttons/button/button';
import { Router } from '@angular/router';
import { Login, LOGIN_INITIAL } from './login-page.models';
import { focusOnFirstInvalidField } from '../../../core/form/focus';
import { logInSchema, toValidationError } from './login-page.validation';

@Component({
    selector: 'app-login-page',
    imports: [EmailField, PasswordField, FormRoot, Button, Banner],
    templateUrl: './login-page.html',
    styleUrl: './login-page.scss',
})
export class LoginPage {
    auth = inject(AuthService);
    router = inject(Router)
    
    loginModel = signal<Login>(LOGIN_INITIAL);

    loginForm = form<Login>(
        this.loginModel,
        path => apply(path, logInSchema),
        {
            submission: {
                action: this.login.bind(this),
                ignoreValidators: 'pending',
                onInvalid: (field) => focusOnFirstInvalidField(field),
            }
        }
    );

    async login(): Promise<TreeValidationResult<ValidationError.WithOptionalFieldTree>> {
        try {
            await firstValueFrom(this.auth.login(this.loginModel()));
            return;
        } catch (response) {
            return toValidationError(response, this.loginForm);
        }
    }

    navigateToSignInPage() {
        this.router.navigate(['/auth/signin']);
    }
}
