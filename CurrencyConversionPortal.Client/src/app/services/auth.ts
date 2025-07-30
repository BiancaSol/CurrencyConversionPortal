import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';
import { ErrorService } from './error.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = environment.apiUrl + '/auth'; 

  constructor(
    private http: HttpClient,
    private errorService: ErrorService
  ) {}
  
  async login(username: string, password: string): Promise<{ success: boolean; error?: string }> {
    try {
      await firstValueFrom(this.http.post(this.baseUrl + '/login', { username, password }, { withCredentials: true }));
      return { success: true };
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        return { success: false, error: this.errorService.getErrorMessage(error) };
      }
      return { success: false, error: 'Login failed. Please try again.' };
    }
  }

  async register(username: string, password: string): Promise<{ success: boolean; error?: string }> {
    try {
      await firstValueFrom(this.http.post(this.baseUrl + '/register', { username, password }));
      return { success: true };
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        return { success: false, error: this.errorService.getErrorMessage(error) };
      }
      return { success: false, error: 'Registration failed. Please try again.' };
    }
  }

  async logout(): Promise<void> {
    try {
      await firstValueFrom(this.http.post(this.baseUrl + '/logout', {}, { withCredentials: true }));
    } catch (error) {
      console.error('Logout error:', error);
    }
  }
}