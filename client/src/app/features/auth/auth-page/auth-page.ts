import { Component, signal } from '@angular/core';
import { Logo } from '../../../shared/symbols/logo/logo';
import { Button } from '../../../shared/buttons/button/button';
import { RouterOutlet } from '@angular/router';
import { LoginPage } from '../login-page/login-page';

@Component({
  selector: 'app-auth-page',
  imports: [Logo, Button, RouterOutlet],
  templateUrl: './auth-page.html',
  styleUrl: './auth-page.scss',
})
export class AuthPage {
    isLoginPage = signal(false);

    onActivate(component: unknown) {
        this.isLoginPage.set(component instanceof LoginPage);
    }  
}
