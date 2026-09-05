import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { LoginRequest, LoginResponse, RefreshResponse, UserProfile, ResetPasswordRequest, ChangePasswordRequest } from './auth.models';
import { AuthStore } from './auth.store';
import { environment } from '../../../environments/environment';

// make auth-service singleton at application level 
// so it can be injected anywhere in the application

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // inject HttpClient to make HTTP requests
  readonly #http = inject(HttpClient);
  readonly #authStore = inject(AuthStore);
  readonly #router = inject(Router);

  login(credentials: LoginRequest): Observable<LoginResponse> {
    //set loading state to true when login request is sent
    this.#authStore.setLoading(true);

    //make a POST request to the login endpoint and handle the response with tap()
    return this.#http.post<LoginResponse>(
      `${environment.apiUrl}/api/v1/auth/login`,
      credentials
    ).pipe(
      tap({
        //if response is successful, update the store and redirect to dashboard
        next: (response) => {
          this.#authStore.setLogin(response);
          this.#router.navigate(['/dashboard']);
        },
        error: () => {
          //if response is error, set loading state to false
          this.#authStore.setLoading(false);
        }
      })
    );
  }

  refreshToken(refreshToken: string): Observable<RefreshResponse> {
    //make a POST request to the refresh endpoint and handle the response with tap()
    return this.#http.post<RefreshResponse>(
      `${environment.apiUrl}/api/v1/auth/refresh`,
      { refreshToken }
    ).pipe(
      //if response is successful, update the token
      tap({
        next: (response) => {
          this.#authStore.updateToken(response.accessToken, response.refreshToken);
        }
      })
    );
  }

  logout(refreshToken: string): Observable<any> {
    return this.#http.post<any>(`${environment.apiUrl}/api/v1/auth/logout`, { refreshToken })
      .pipe(
        tap({
          next: () => {
            this.#authStore.logout();
            this.#router.navigate(['/login']);
          }
        })
      );
  }

  getMe(): Observable<UserProfile> {
    return this.#http.get<UserProfile>(`${environment.apiUrl}/api/v1/auth/me`);
  }

  forgotPassword(email: string): Observable<any> {
    return this.#http.post<any>(`${environment.apiUrl}/api/v1/auth/forgot-password`, { email });
  }

  resetPassword(payload: ResetPasswordRequest): Observable<any> {
    return this.#http.post<any>(`${environment.apiUrl}/api/v1/auth/reset-password`, payload);
  }

  changePassword(payload: ChangePasswordRequest): Observable<any> {
    return this.#http.post<any>(`${environment.apiUrl}/api/v1/auth/change-password`, payload);
  }
}
