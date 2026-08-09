export interface LogInRequest {
    email: string;
    password: string;
}

export interface LogInResponse {
    accessToken: string;
}