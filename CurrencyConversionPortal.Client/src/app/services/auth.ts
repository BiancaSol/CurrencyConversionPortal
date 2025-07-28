import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private baseUrl = environment.apiUrl + '/auth'; // or adjust to match your API

  constructor(private http: HttpClient) {}
  async login(username: string, password: string): Promise<boolean> {
    try {
      await firstValueFrom(this.http.post(this.baseUrl + '/login', { username, password }, { withCredentials: true }));
      return true;
    } catch {
      return false;
    }
  }

  async register(username: string, password: string): Promise<{ success: boolean; message?: string }> {
    try {
      await firstValueFrom(this.http.post(this.baseUrl + '/register', { username, password }));
      return { success: true };
    } catch (error: any) {
      const message = error?.error?.message || error?.error || 'Registration failed';
      return { success: false, message };
    }
  }
}