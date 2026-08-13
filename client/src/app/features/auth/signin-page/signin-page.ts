import { Component, inject, signal } from '@angular/core';
import { Logo } from '../../../shared/symbols/logo/logo';
import { apply, form, FormRoot, TreeValidationResult, ValidationError } from '@angular/forms/signals';
import { EmailField } from '../../../shared/forms/fields/email-field/email-field';
import { PasswordField } from '../../../shared/forms/fields/password-field/password-field';
import { AuthService } from '../../../core/auth/auth-service/auth-service';
import { firstValueFrom } from 'rxjs';
import { Banner } from '../../../shared/banner/banner';
import { Button } from '../../../shared/buttons/button/button';
import { NameField } from '../../../shared/forms/fields/name-field/name-field';
import { Router } from '@angular/router';
import { Snackbar, SnackbarService, SnackbarTypeEnum } from '../../../core/snackbar/snackbar.service';
import { Signin, SIGNIN_INITIAL } from './signin-page.models';
import { focusOnFirstInvalidField } from '../../../core/form/focus';
import { signInSchema, toValidationError } from './signin-page.validation';

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
        path => apply(path, signInSchema),
        {
            submission: {
                action: this.signin.bind(this),
                ignoreValidators: 'pending',
                onInvalid: (field) => focusOnFirstInvalidField(field),
            }
        }
    );

    async signin(): Promise<TreeValidationResult<ValidationError.WithOptionalFieldTree>> {
        try {
            await firstValueFrom(this.auth.signin(this.signinModel()));
            this.addAccountCreatedSnackbar();
            this.navigateToLoginPage();
            return;
        } catch (response) {
            return toValidationError(response, this.signinForm);
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
