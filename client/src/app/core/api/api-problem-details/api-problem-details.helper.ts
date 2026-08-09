import { ApiProblemDetails } from "./api-problem-details.model";

export function isApiProblemDetails(value: unknown): value is ApiProblemDetails {
    return (
        typeof value === 'object' &&
        value !== null &&
        'title' in value &&
        'status' in value
    );
}