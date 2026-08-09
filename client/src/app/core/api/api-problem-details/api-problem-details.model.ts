export enum ErrorLocationEnum {
    Body,
    Query,
    Path,
    Header
}

export interface DetailedResponseError {
    code: string;
    message: string;
    field?: string;
    location?: ErrorLocationEnum;
    rejectedValue?: string;
}

export interface ApiProblemDetails {
    traceId: string;
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
    instance?: string;
    errors: DetailedResponseError[];
}