import { Component, OnInit } from '@angular/core';
import { NgClass, NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [NgClass],
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.css']
})
export class SidebarComponent implements OnInit {
  activeRoute: string = 'dashboard';
  userRole!: string | null;

  constructor(private router: Router, private authService: AuthService) {}

  ngOnInit(): void {
    const userData = localStorage.getItem('user');
    if (userData) {
      const user = JSON.parse(userData);
      this.userRole=this.authService.getUserRole();
    }
    this.setActiveRoute();
  }

  navigate(route: string): void {
    this.activeRoute = route;
    this.router.navigate([`/${route}`]);
  }

  logout(): void {
    if (confirm('Are you sure you want to logout?')) {
      this.authService.logout().subscribe({
        next: () => {
          localStorage.removeItem('user');
          localStorage.removeItem('token');
          this.router.navigate(['/login']);
        },
        error: (err) => {
          console.error('Logout error:', err);
          localStorage.removeItem('user');
          localStorage.removeItem('token');
          this.router.navigate(['/login']);
        }
      });
    }
  }

  private setActiveRoute(): void {
    const currentUrl = this.router.url;
    if (currentUrl.includes('/dashboard')) this.activeRoute = 'dashboard';
    else if (currentUrl.includes('/reviews')) this.activeRoute = 'reviews';
    else if (currentUrl.includes('/history')) this.activeRoute = 'history'; 
    else if (currentUrl.includes('/reports')) this.activeRoute = 'reports';
    else if (currentUrl.includes('/admin')) this.activeRoute = 'admin';
    else if (currentUrl.includes('/departments')) this.activeRoute = 'departments';
    else if (currentUrl.includes('/assignments')) this.activeRoute = 'assignments';
    else if (currentUrl.includes('/pending')) this.activeRoute = 'pending';
    else if (currentUrl.includes('/dashboard')) this.activeRoute = 'dashboard';
    else this.activeRoute = 'notfound';
  }
}