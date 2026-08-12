import { Service, signal } from '@angular/core';
import { SnackbarHelper } from './snackbar.helper';
import { SignalHelper } from '../signal/signal.helper';

export enum SnackbarTypeEnum {
    Info,
    Success,
    Warning,
    Error
}

export enum DurationEnum {
    UntilDismissed = -1,
    Short = 3000,
    Medium = 5000,
    Long = 10000
}

export class Snackbar {
    id = crypto.randomUUID();
    type = SnackbarTypeEnum.Info;
    text = '';
    duration = DurationEnum.Medium;
    countdown: number = this.duration;
    isCountingDown = false;
}

@Service()
export class SnackbarService {
    private _snackbars = signal<Snackbar[]>([]);

    snackbars = this._snackbars.asReadonly();

    readonly INTERVAL = 1000;

    addSnackbar(snackbar: Snackbar) {
        this._snackbars.update(snackbars => SignalHelper.appendIfNotExistsById(snackbar, snackbars));
    }

    removeSnackbar = (id: string) => {
        this._snackbars.update(snackbars => SignalHelper.removeItemById(id, snackbars));
    }

    tryStartCountdown(id: string) {
        const snack = this._snackbars().find(snackbar => snackbar.id === id);

        if (!snack || !SnackbarHelper.shouldStartCountdown(snack))
            return;

        this.startCountdown(id);
    }

    private startCountdown(id: string) {
        this.updateIsCountingDown(id, true);
        this.setInterval(id);
    }

    private updateIsCountingDown(id: string, value: boolean) {
        this._snackbars.update(snackbars => 
            SignalHelper.updateItemById(id, snackbars, snackbar => ({ ...snackbar, isCountingDown: value })));
    }

    private setInterval(id: string) {
        const interval = setInterval(() => {
            this._snackbars.update(snackbars => {
                const snackbar = snackbars.find(snackbar => snackbar.id === id);

                if (!snackbar) {
                    clearInterval(interval);
                    return snackbars;
                }

                snackbar.countdown -= this.INTERVAL;

                if (snackbar.countdown <= 0) {
                    clearInterval(interval);
                    return SignalHelper.removeItemById(id, snackbars);
                }

                return SignalHelper.updateItemById(id, snackbars, snackbar => snackbar);
            });
        }, this.INTERVAL);
    }
}