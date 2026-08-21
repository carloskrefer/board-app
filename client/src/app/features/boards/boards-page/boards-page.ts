import { Component } from '@angular/core';
import { BoardItem } from '../board-item/board-item';
import { ButtonGroup } from '../../../shared/buttons/button-group/button-group';

@Component({
  selector: 'app-boards-page',
  imports: [BoardItem, ButtonGroup],
  templateUrl: './boards-page.html',
  styleUrl: './boards-page.scss',
})
export class BoardsPage {}
