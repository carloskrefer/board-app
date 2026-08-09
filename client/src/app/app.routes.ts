import { Routes } from '@angular/router';
import { LoginPage } from './features/auth/login-page/login-page';

export const routes: Routes = [
  {
    path: '',
    component: LoginPage,
  },
  {
    path: 'login',
    component: LoginPage,
  }, 
];
