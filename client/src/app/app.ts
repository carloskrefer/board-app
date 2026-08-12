import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SnackbarList } from './shared/snackbar/snackbar-list/snackbar-list';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, SnackbarList],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('client');
}
