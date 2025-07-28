import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-navbar',
  imports: [RouterModule, CommonModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar implements OnInit {
  isAuthenticated = false;
  currentRoute = '';

  constructor(
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit() {
    // Check if user is authenticated by checking current route
    this.router.events.subscribe(() => {
      this.currentRoute = this.router.url;
      this.isAuthenticated = this.currentRoute !== '/login' && this.currentRoute !== '/register';
    });
  }

  async logout() {
    try {
      await this.authService.logout();
      this.isAuthenticated = false;
      this.router.navigate(['/login']);
    } catch (error) {
      console.error('Logout error:', error);
      // Navigate to login anyway
      this.router.navigate(['/login']);
    }
  }

  isActive(route: string): boolean {
    return this.currentRoute === route;
  }
}
