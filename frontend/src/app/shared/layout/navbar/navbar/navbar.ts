import { Component, OnInit } from '@angular/core';
import { NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth';

@Component({
  selector: 'app-top-bar',
  standalone: true,
  imports: [NgIf],
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.scss']
})
export class TopBarComponent implements OnInit {
  pageTitle: string = 'Dashboard';
  showProfileMenu: boolean = false;
  userName: string = '';
  userEmail: string = '';
  userRole: string = '';
  userInitials: string = '';

  constructor(private router: Router, private authService: AuthService) {}

  ngOnInit(): void {
    this.loadUserData();
    this.updatePageTitle();
  }

  private loadUserData(): void {
    const userData = localStorage.getItem('user');
    if (userData) {
      const user = JSON.parse(userData);
      this.userName = user.fullName || user.username || user.name || 'User';
      this.userEmail = user.email || '';
      this.userRole = user.role || (Array.isArray(user.roles) ? user.roles.join(', ') : user.roles?.toString() || '');
      this.userInitials = this.getInitials(this.userName);
    }
  }

  getInitials(name: string): string {
    if (!name) return '?';
    return name
      .split(' ')
      .map(n => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2);
  }

  toggleProfileMenu(): void {
    this.showProfileMenu = !this.showProfileMenu;
  }

  closeDropdown(): void {
    this.showProfileMenu = false;
  }

  openProfile(): void {
    this.closeDropdown();
    this.router.navigate(['/profile']);
  }

  openChangePassword(): void {
    this.closeDropdown();
    this.router.navigate(['/change-password']);
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

  private updatePageTitle(): void {
    const currentUrl = this.router.url;
    if (currentUrl.includes('dashboard')) this.pageTitle = 'Dashboard';
    else if (currentUrl.includes('evaluations')) this.pageTitle = 'My Evaluations';
    else if (currentUrl.includes('form')) this.pageTitle = 'Evaluation Form';
    else if (currentUrl.includes('reports')) this.pageTitle = 'Reports';
    else if (currentUrl.includes('admin')) this.pageTitle = 'Admin Panel';
    else if (currentUrl.includes('settings')) this.pageTitle = 'Settings';
    else if (currentUrl.includes('profile')) this.pageTitle = 'My Profile';
    else this.pageTitle = 'Evaluation System';
  }
}