import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ErrorService } from '../services/error.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private errorService: ErrorService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        console.log('Interceptor caught error:', {
          status: error.status,
          url: error.url,
          originalUrl: req.url,
          message: error.message
        });

        // Check if this is an authentication error
        const isAuthError = this.errorService.isAuthError(error);
        
        // Check if this is a 405 error on a login URL (result of 302 redirect)
        const is405OnLogin = error.status === 405 && error.url && 
          (error.url.includes('/auth/login') || error.url.includes('ReturnUrl='));
        
        if (isAuthError || is405OnLogin) {
          console.log('Handling authentication error - redirecting to login');
          // Create a synthetic auth error for consistent handling
          const authError = new HttpErrorResponse({
            status: 401,
            statusText: 'Unauthorized',
            url: req.url || error.url || undefined
          });
          this.errorService.handleAuthError(authError);
          
          // Return a more user-friendly error instead of the 405
          return throwError(() => authError);
        }
        
        // Re-throw the original error for non-auth errors
        return throwError(() => error);
      })
    );
  }
}