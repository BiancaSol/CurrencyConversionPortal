import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { ApiError } from '../models/api-error';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {

  constructor(private router: Router) {}

  /**
   * Extract user-friendly error message from HTTP error response
   */
  getErrorMessage(error: HttpErrorResponse): string {
    if (error.status === 0) {
      return 'Unable to connect to the server. Please check your connection.';
    }

    if (error.error && typeof error.error === 'object') {
      const apiError = error.error as ApiError;
      if (apiError.details) {
        return apiError.details;
      }
      if (apiError.error) {
        return this.getFriendlyMessage(apiError.error, error.status);
      }
    }

    if (error.status === 401 && error.error && typeof error.error === 'string') {

      return error.error;
    }

    switch (error.status) {
      case 400:
        return 'Invalid request. Please check your input.';
      case 401:
      case 302:
        return 'Please log in to continue.';
      case 403:
        return 'You do not have permission to perform this action.';
      case 404:
        return 'The requested resource was not found.';
      case 500:
        return 'A server error occurred. Please try again later.';
      case 503:
        return 'The service is temporarily unavailable. Please try again later.';
      default:
        return 'An unexpected error occurred. Please try again.';
    }
  }

  /**
   * Check if error requires login redirect
   */
  isAuthError(error: HttpErrorResponse): boolean {
    return error.status === 401 || error.status === 403 || error.status === 302;
  }

  /**
   * Check if error is retryable (network or service issues)
   */
  isRetryable(error: HttpErrorResponse): boolean {
    return error.status === 0 || error.status === 503 || error.status >= 500;
  }

  /**
   * Handle authentication errors by redirecting to login
   */
  handleAuthError(error: HttpErrorResponse): void {
    if (this.isAuthError(error) || (error.status === 405 && error.url && error.url.includes('/auth/login'))) {
      this.router.navigate(['/login']);
    }
  }

  /**
   * Convert API error messages to user-friendly messages
   */
  private getFriendlyMessage(apiErrorMessage: string, statusCode: number): string {
    const message = apiErrorMessage.toLowerCase();

    if (message.includes('validation')) {
      return 'Please check your input and try again.';
    }
    if (message.includes('business logic')) {
      return 'This operation is not allowed.';
    }
    if (message.includes('external service')) {
      return 'Currency service is temporarily unavailable. Please try again later.';
    }
    // For authentication errors, if the message is specific (like "Invalid username or password"), 
    // preserve it instead of converting to generic message
    if (message.includes('unauthorized') && !message.includes('invalid') && !message.includes('password') && !message.includes('username')) {
      return 'Please log in to continue.';
    }

    return apiErrorMessage.length < 100 ? apiErrorMessage : this.getErrorMessage({ status: statusCode } as HttpErrorResponse);
  }
}
