import { Routes } from '@angular/router';
import { LoginPage } from './features/auth/login-page/login-page';
import { SigninPage } from './features/auth/signin-page/signin-page';

export const routes: Routes = [
  {
    path: '',
    component: LoginPage,
  },
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
];
