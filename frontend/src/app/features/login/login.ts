import { Component, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/services/auth';
import { Router, RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './login.html',
  styleUrls: ['./login.scss'],
})
export class Login {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  loading = signal(false);
  errorMessage = signal('');
  showPassword = signal(false);

  form = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  togglePassword() {
    this.showPassword.update(v => !v);
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading.set(true);
    this.errorMessage.set('');

    this.authService.login(this.form.getRawValue()).subscribe({
      next: (res) => {
        this.authService.saveToken(res.token);
        // Decode JWT to extract user info for navbar/sidebar
        const userInfo = this.decodeToken(res.token);
        if (userInfo) {
          localStorage.setItem('user', JSON.stringify(userInfo));
        }
        this.loading.set(false);
        this.router.navigate(['/admin']);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        const defaultMessage = 'Wrong email or password. Please try again.';
        if (err.status === 400 || err.status === 401 || err.status === 500) {
          this.errorMessage.set(defaultMessage);
        } else if (err.status === 0) {
          this.errorMessage.set('Unable to reach the authentication server. Please check your connection.');
        } else {
          this.errorMessage.set(
            err.error?.message || err.error?.Message || defaultMessage
          );
        }
      },
    });
  }

  private decodeToken(token: string): { fullName: string; email: string; role: string; id: number } | null {
    try {
      const parts = token.split('.');
      if (parts.length !== 3) return null;
      const payload = JSON.parse(atob(parts[1]));
      return {
        fullName: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
                  || payload.name
                  || payload.unique_name
                  || payload.sub
                  || 'User',
        email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']
               || payload.email
               || payload.emailaddress
               || '',
        role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
              || payload.role
              || (Array.isArray(payload.roles) ? payload.roles.join(', ') : payload.roles?.toString() || '')
              || '',
        id: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid']
            || payload.sid
            || payload.nameidentifier
            || payload.sub
            || Number(payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'])
            || 0,
      };
    } catch {
      return null;
    }
  }
}
