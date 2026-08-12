import { DurationEnum, Snackbar } from "./snackbar.service";

export class SnackbarHelper {
    static calculateProgressStartingAtIncrement(snackbar: Snackbar | undefined, increment: number): number {
        if (!snackbar)
            return 0;

        if (snackbar.duration == DurationEnum.UntilDismissed)
            return 0;

        if (snackbar.countdown == snackbar.duration)
            return increment;

        return increment + snackbar.duration - snackbar.countdown;
    }

    static shouldStartCountdown(snackbar?: Snackbar): boolean {
        return (!!snackbar) && (!snackbar.isCountingDown) && (snackbar.duration != DurationEnum.UntilDismissed);
    }
}