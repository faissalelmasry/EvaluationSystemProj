import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from '../../core/services/user';
import { User } from '../../core/models/user.model';
import { HttpErrorResponse } from '@angular/common/http';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './profile.html',
  styleUrls: ['./profile.css'],
})
export class Profile implements OnInit {
  private readonly userService = inject(UserService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  user = signal<User | null>(null);
  userFromToken = signal<{ fullName: string; email: string; role: string; id: number } | null>(null);
  loading = signal(false);
  errorMessage = signal('');

  ngOnInit(): void {
    this.loadUserFromToken();
    this.loadUserDetails();
  }

  private loadUserFromToken(): void {
    const userData = localStorage.getItem('user');
    if (userData) {
      try {
        const parsed = JSON.parse(userData);
        this.userFromToken.set(parsed);
      } catch {
        this.errorMessage.set('Failed to load user session data.');
      }
    } else {
      this.errorMessage.set('No user session found. Please log in again.');
    }
  }

  private loadUserDetails(): void {
    const userData = this.userFromToken();
    const userId = userData?.id;

    if (!userId) {
      this.loading.set(false);
      return;
    }

    this.loading.set(true);
    this.userService.getById(userId).subscribe({
      next: (user) => {
        this.user.set(user);
        this.loading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        const defaultMessage = 'Failed to load user details.';
        if (err.status === 401) {
          this.errorMessage.set('Session expired. Please log in again.');
        } else if (err.status === 404) {
          this.errorMessage.set('User not found.');
        } else {
          this.errorMessage.set(
            err.error?.message || err.error?.Message || defaultMessage
          );
        }
      },
    });
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

  getJobTitleLabel(jobTitle: number): string {
    const titles: Record<number, string> = {
      1: 'Teacher',
      2: 'Student',
      3: 'Manager',
      4: 'Employee',
      5: 'Client',
    };
    return titles[jobTitle] || 'Unknown';
  }

  getUserRole(user: User): string {
    if (user.role) return user.role;
    if (user.roles?.length) return user.roles.join(', ');
    return this.userFromToken()?.role || '';
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
        },
      });
    }
  }
}