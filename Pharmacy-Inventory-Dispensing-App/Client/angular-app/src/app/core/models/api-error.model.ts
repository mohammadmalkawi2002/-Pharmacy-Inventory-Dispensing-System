import { HttpErrorResponse } from '@angular/common/http';

/**
 * Standard backend error response contract.
 */
export interface ApiErrorResponse {
  success: boolean;
  message: string;
  errors?: Record<string, string[]>;
  traceId?: string;
}

/**
 * Extracts a user-friendly error message from an API error response.
 * Handles both the custom ApiErrorResponse shape and standard ProblemDetails.
 */
export function extractApiErrorMessage(error: unknown): string {
  if (!error) return 'An unexpected error occurred.';

  const httpError = error as HttpErrorResponse;
  const body = httpError.error ?? error;

  if (typeof body === 'string') {
    return body;
  }

  if (typeof body === 'object' && body !== null) {
    // 1. Check detailed validation or domain errors dictionary
    if ('errors' in body && typeof body.errors === 'object' && body.errors !== null) {
      const messages: string[] = [];
      for (const key of Object.keys(body.errors)) {
        const val = (body.errors as Record<string, unknown>)[key];
        if (Array.isArray(val)) {
          messages.push(...val.filter(v => typeof v === 'string'));
        } else if (typeof val === 'string') {
          messages.push(val);
        }
      }
      if (messages.length > 0) {
        return messages.join(' ');
      }
    }

    // 2. Check top-level message
    if ('message' in body && typeof body.message === 'string' && body.message.trim().length > 0) {
      return body.message;
    }

    // 3. Check ProblemDetails detail or title
    if ('detail' in body && typeof body.detail === 'string' && body.detail.trim().length > 0) {
      return body.detail;
    }
    if ('title' in body && typeof body.title === 'string' && body.title.trim().length > 0) {
      return body.title;
    }
  }

  if (httpError.message) {
    return httpError.message;
  }

  return 'An unexpected error occurred. Please try again.';
}
