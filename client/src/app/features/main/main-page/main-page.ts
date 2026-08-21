import { Component } from '@angular/core';
import { MainHeader } from '../main-header/main-header';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-main-page',
  imports: [MainHeader, RouterOutlet],
  templateUrl: './main-page.html',
  styleUrl: './main-page.scss',
})
export class MainPage {}
