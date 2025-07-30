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
        const isAuthError = this.errorService.isAuthError(error);
        
        const is405OnLogin = error.status === 405 && error.url && 
          (error.url.includes('/auth/login') || error.url.includes('ReturnUrl='));
        
        if (isAuthError || is405OnLogin) {
          const authError = new HttpErrorResponse({
            status: 401,
            statusText: 'Unauthorized',
            url: req.url || error.url || undefined
          });
          this.errorService.handleAuthError(authError);
          
          return throwError(() => authError);
        }
        
        return throwError(() => error);
      })
    );
  }
}