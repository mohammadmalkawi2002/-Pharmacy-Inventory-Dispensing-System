import { HttpInterceptorFn, HttpErrorResponse, HttpRequest, HttpHandlerFn, HttpEvent } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, catchError, filter, switchMap, take, throwError } from 'rxjs';
import { AuthStore } from '../auth/auth.store';
import { AuthService } from '../auth/auth.service';

let isRefreshing = false;
const refreshSubject = new BehaviorSubject<string | null>(null);

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authStore = inject(AuthStore);
  const authService = inject(AuthService);
  const router = inject(Router);

  // Skip auth header for login/refresh endpoints
  if (req.url.includes('/auth/login') || req.url.includes('/auth/refresh')) {
    return next(req);
  }

  const token = authStore.accessToken();
  let authReq = req;

  if (token) {
    authReq = addToken(req, token);
  }

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && token) {
        return handleTokenRefresh(authReq, next, authService, authStore, router);
      }
      return throwError(() => error);
    })
  );
};

function addToken(req: HttpRequest<unknown>, token: string): HttpRequest<unknown> {
  return req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  });
}

function handleTokenRefresh(
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
  authService: AuthService,
  authStore: AuthStore,
  router: Router
): Observable<HttpEvent<unknown>> {
  if (!isRefreshing) {
    isRefreshing = true;
    refreshSubject.next(null);

    return authService.refreshToken(authStore.refreshToken()!).pipe(
      switchMap(response => {
        isRefreshing = false;
        refreshSubject.next(response.accessToken);
        return next(addToken(req, response.accessToken));
      }),
      catchError(err => {
        isRefreshing = false;
        refreshSubject.next(null);
        authStore.logout();
        router.navigate(['/login'], {
          queryParams: { sessionExpired: 'true' }
        });
        return throwError(() => err);
      })
    );
  }

  // Wait for refresh in progress, then retry
  return refreshSubject.pipe(
    filter(token => token !== null),
    take(1),
    switchMap(token => next(addToken(req, token!)))
  );
}
