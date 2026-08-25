import { CustomProblemDetails } from "./api-problem-details.model";

export function isCustomProblemDetails(value: unknown): value is CustomProblemDetails {
    return (
        typeof value === 'object' &&
        value !== null &&
        'title' in value &&
        'status' in value
    );
}