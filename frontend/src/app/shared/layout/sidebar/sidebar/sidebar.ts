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
  userRole: string = '';

  constructor(private router: Router, private authService: AuthService) {}

  ngOnInit(): void {
    const userData = localStorage.getItem('user');
    if (userData) {
      const user = JSON.parse(userData);
      this.userRole = user.role || (Array.isArray(user.roles) ? user.roles.join(', ') : user.roles?.toString() || '');
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
          localStorage.removeItem('jwt_token');
          this.router.navigate(['/login']);
        },
        error: (err) => {
          console.error('Logout error:', err);
          localStorage.removeItem('user');
          localStorage.removeItem('jwt_token');
          this.router.navigate(['/login']);
        }
      });
    }
  }

  private setActiveRoute(): void {
    const currentUrl = this.router.url;
    if (currentUrl.includes('/dashboard')) this.activeRoute = 'dashboard';
    else if (currentUrl.includes('/evaluations')) this.activeRoute = 'evaluations';
    else if (currentUrl.includes('/form')) this.activeRoute = 'form';
    else if (currentUrl.includes('/reports')) this.activeRoute = 'reports';
    else if (currentUrl.includes('/admin')) this.activeRoute = 'admin';
    else if (currentUrl.includes('/departments')) this.activeRoute = 'departments';
    else if (currentUrl.includes('/assignments')) this.activeRoute = 'assignments';
    else if (currentUrl.includes('/pending')) this.activeRoute = 'pending';
    else this.activeRoute = 'dashboard';
  }
}
