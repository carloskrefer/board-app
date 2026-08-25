export enum ErrorLocationEnum {
    Body,
    Query,
    Path,
    Header
}

export interface DetailedError {
    code: string;
    message: string;
    field?: string;
    location?: ErrorLocationEnum;
    rejectedValue?: string;
}

export interface CustomProblemDetails {
    traceId: string;
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
    instance?: string;
    errors: DetailedError[];
}