import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { MessageService } from 'primeng/api';
import { extractApiErrorMessage } from '../models/api-error.model';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const messageService = inject(MessageService, { optional: true });

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let summary = 'Error';
      let detail = 'An unexpected error occurred.';

      if (error.status === 0 || error.error instanceof ProgressEvent) {
        // Network / client-side connection error
        summary = 'Network Error';
        detail = 'Unable to connect to the server. Please check your network connection or API server.';
      } else {
        // Server-side error
        const serverDetail = extractApiErrorMessage(error);

        switch (error.status) {
          case 400:
            summary = 'Invalid Request';
            detail = serverDetail;
            break;
          case 401:
            // Handled by AuthInterceptor / LoginComponent — don't show global duplicate toast
            return throwError(() => error);
          case 403:
            summary = 'Access Forbidden';
            detail = serverDetail || 'You do not have permission to perform this action.';
            break;
          case 404:
            summary = 'Not Found';
            detail = serverDetail || 'The requested resource was not found.';
            break;
          case 409:
            summary = 'Conflict';
            detail = serverDetail || 'A conflict occurred with the current state of the resource.';
            break;
          case 422:
            summary = 'Validation Error';
            detail = serverDetail;
            break;
          case 500:
          case 502:
          case 503:
            summary = 'Server Error';
            detail = serverDetail || 'A backend server error occurred. Please try again later.';
            break;
          default:
            detail = serverDetail || `An unexpected error occurred (HTTP ${error.status}).`;
            break;
        }
      }

      if (messageService) {
        messageService.add({
          severity: 'error',
          summary,
          detail,
          life: 5000
        });
      }

      return throwError(() => error);
    })
  );
};
