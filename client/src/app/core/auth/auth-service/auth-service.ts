import { inject, Service } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { LogInRequest, LogInResponse } from './dtos/login-dtos';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { SignInRequest, SignInResponse } from './dtos/signin-dtos';

@Service()
export class AuthService {
    readonly url = `${environment.api.board.url}/api/users`;

    http = inject(HttpClient);

    login(request: LogInRequest): Observable<LogInResponse> {
        return this.http.post<LogInResponse>(`${this.url}/login`, request);
    }

    signin(request: SignInRequest): Observable<SignInResponse> {
        return this.http.post<SignInResponse>(`${this.url}`, request);
    }
}