import { Component, inject, signal } from '@angular/core';
import { Logo } from '../../../shared/symbols/logo/logo';
import { email, FieldTree, form, FormRoot, required, TreeValidationResult, ValidationError } from '@angular/forms/signals';
import { EmailField } from '../../../shared/forms/fields/email-field/email-field';
import { PasswordField } from '../../../shared/forms/fields/password-field/password-field';
import { AuthService } from '../../../core/auth/auth-service/auth-service';
import { firstValueFrom } from 'rxjs';
import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { isApiProblemDetails } from '../../../core/api/api-problem-details/api-problem-details.helper';
import { Banner } from '../../../shared/banner/banner';
import { Button } from '../../../shared/buttons/button/button';

class Login {
    email = '';
    password = '';
}

const LOGIN_INITIAL: Login = {
    email: '',
    password: '',
};

@Component({
    selector: 'app-login-page',
    imports: [Logo, EmailField, PasswordField, FormRoot, Button, Banner],
    templateUrl: './login-page.html',
    styleUrl: './login-page.scss',
})
export class LoginPage {
    auth = inject(AuthService);
    
    loginModel = signal<Login>(LOGIN_INITIAL);

    loginForm = form<Login>(
        this.loginModel,
        path => {
            email(path.email, { message: 'Expected pattern: user@domain.com' });
            required(path.email, { message: 'Email is required' });
            required(path.password, { message: 'Password is required' });
        },
        {
            submission: {
                action: this.login.bind(this),
                ignoreValidators: 'pending',
                onInvalid: (field) => this.focusOnFirstInvalidField(field),
            }
        }
    );

    focusOnFirstInvalidField(field: FieldTree<Login>): void {
        const errors = field().errorSummary();

        if (errors.length)
            errors[0].fieldTree().focusBoundControl();
    }

    // TODO: A cada 3s mudar o valor do email, setar ele, pra ver se o angular atualiza ele no template, pois o signal
    // só é feito () no template do filho. Se funfar, anotar isso no docs.

    async login(): Promise<TreeValidationResult<ValidationError.WithOptionalFieldTree>> {
        try {
            await firstValueFrom(this.auth.login(this.loginModel()));
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
                            case 'email': return { kind: 'emailError', message: error.message, fieldTree: this.loginForm.email };
                            case 'password': return { kind: 'passwordError', message: error.message, fieldTree: this.loginForm.password };
                            default: return { kind: 'generalError', message: error.message };
                        }
                    });
                }
                return { kind: 'credentialsError', message: body.detail };
            }

            return { kind: 'serverError', message: 'An unkown error has occurred. Please try again later.' };
        }
    }
}
