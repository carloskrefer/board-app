import { Component, computed, effect, inject } from '@angular/core';
import { DurationEnum, SnackbarService } from '../../../core/snackbar/snackbar.service';
import { SnackbarItem } from '../snackbar-item/snackbar-item';
import { SnackbarHelper } from '../../../core/snackbar/snackbar.helper';

@Component({
    selector: 'app-snackbar-list',
    imports: [SnackbarItem],
    templateUrl: './snackbar-list.html',
    styleUrl: './snackbar-list.scss',
})
export class SnackbarList {
    service = inject(SnackbarService);

    constructor() {
        effect(() => {
            const first = this.service.snackbars()[0];

            if (SnackbarHelper.shouldStartCountdown(first))
                this.service.tryStartCountdown(first.id);
        });
    }
}
