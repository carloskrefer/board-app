import { Component } from '@angular/core';
import { Button } from '../button/button';

@Component({
  selector: 'app-button-group',
  imports: [Button],
  templateUrl: './button-group.html',
  styleUrl: './button-group.scss',
})
export class ButtonGroup {}
