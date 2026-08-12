import { SnackbarTypeEnum } from "../../../core/snackbar/snackbar.service";
import { ProgressBarColor } from "../../progress-indicators/progress-bar/progress-bar";

export class SnackbarItemHelper {
    static fromSnackbarTypeEnumToProgressBarColor(type: SnackbarTypeEnum): ProgressBarColor {
        switch (type) {
            case SnackbarTypeEnum.Info:
                return 'info';
            case SnackbarTypeEnum.Success:
                return 'success';
            case SnackbarTypeEnum.Warning:
                return 'warning';
            case SnackbarTypeEnum.Error:
                return 'error';
            default:
                return 'default';
        }
    }

    static fromSnackbarTypeEnumToSnackbarItemColorCssClass(type: SnackbarTypeEnum): string {
        switch (type) {
            case SnackbarTypeEnum.Info:
                return 'info';
            case SnackbarTypeEnum.Success:
                return 'success';
            case SnackbarTypeEnum.Warning:
                return 'warning';
            case SnackbarTypeEnum.Error:
                return 'error';
            default:
                return 'info';
        }
    }
}