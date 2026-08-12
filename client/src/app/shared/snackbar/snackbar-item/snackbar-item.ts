import { Component, computed, inject, input, output } from '@angular/core';
import { Snackbar, SnackbarService, SnackbarTypeEnum } from '../../../core/snackbar/snackbar.service';
import { ProgressBar } from '../../progress-indicators/progress-bar/progress-bar';
import { SnackbarHelper } from '../../../core/snackbar/snackbar.helper';
import { SnackbarItemHelper } from './snackbar-item.helper';
import { CloseButton } from '../../buttons/close-button/close-button';

@Component({
    selector: 'app-snackbar-item',
    imports: [ProgressBar, CloseButton],
    templateUrl: './snackbar-item.html',
    styleUrl: './snackbar-item.scss',
})
export class SnackbarItem {
    id = input.required<string>();

    close = output();

    service = inject(SnackbarService);

    snackbarTypeEnum = SnackbarTypeEnum;
    readonly defaultSnackbar = new Snackbar();

    currentProgress = computed(() =>
        SnackbarHelper.calculateProgressStartingAtIncrement(this.snackbar(), this.service.INTERVAL));

    snackbar = computed(() => {
        var id = this.id();
        return this.service.snackbars().find((item) => item.id == id) ?? this.defaultSnackbar
    });

    progressBarColor = computed(() => {
        const type = this.snackbar().type;
        return SnackbarItemHelper.fromSnackbarTypeEnumToProgressBarColor(type);
    });

    classes = computed(() => {
        const type = this.snackbar().type;
        return SnackbarItemHelper.fromSnackbarTypeEnumToSnackbarItemColorCssClass(type);
    });
}
