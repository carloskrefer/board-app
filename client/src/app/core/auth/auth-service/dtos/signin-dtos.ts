export interface SignInRequest {
    email: string;
    name: string;
    password: string;
}

export interface SignInResponse {
    userId: string;
}