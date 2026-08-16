import { Routes } from '@angular/router';
import { LoginPage } from './features/auth/login-page/login-page';
import { SigninPage } from './features/auth/signin-page/signin-page';
import { AuthPage } from './features/auth/auth-page/auth-page';

export const routes: Routes = [
    {
        path: 'auth',
        component: AuthPage,
        children: [
            {
                path: 'login',
                component: LoginPage,
                title: 'Login - BoardApp',
            },
            {
                path: 'signin',
                component: SigninPage,
                title: 'Sign in - BoardApp',
            },
        ],
    },
    // TODO: Create a 404 page and redirect to it instead of redirecting to login page
    { 
        path: '**', 
        redirectTo: 'auth/login' 
    },
];
