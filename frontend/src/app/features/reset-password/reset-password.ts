import { Component, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/services/auth';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './reset-password.html',
  styleUrls: ['./reset-password.css'],
})
export class ResetPassword {
  private readonly authService = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  loading = signal(false);
  successMessage = signal('');
  errorMessage = signal('');
  showPassword = signal(false);
  showConfirmPassword = signal(false);

  email = signal('');
  resetToken = signal('');

  constructor() {
    // Extract query params: email and token
    this.route.queryParams.subscribe(params => {
      this.email.set(params['email'] || '');
      this.resetToken.set(params['token'] || '');
    });
  }

  form = new FormGroup({
    newPassword: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(6)],
    }),
    confirmPassword: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  togglePassword() {
    this.showPassword.update(v => !v);
  }

  toggleConfirmPassword() {
    this.showConfirmPassword.update(v => !v);
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { newPassword, confirmPassword } = this.form.getRawValue();

    if (newPassword !== confirmPassword) {
      this.errorMessage.set('Passwords do not match.');
      return;
    }

    if (!this.email() || !this.resetToken()) {
      this.errorMessage.set('Invalid reset link. Missing email or token.');
      return;
    }

    this.loading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    this.authService.resetPassword({
      email: this.email(),
      token: this.resetToken(),
      newPassword,
    }).subscribe({
      next: (res) => {
        this.loading.set(false);
        this.successMessage.set(res.message || 'Password has been reset successfully.');
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 2000);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        if (err.status === 400) {
          this.errorMessage.set('Invalid or expired reset token. Please request a new one.');
        } else if (err.status === 0) {
          this.errorMessage.set('Unable to reach the server. Please check your connection.');
        } else {
          this.errorMessage.set(err.error?.message || err.error?.Message || 'An error occurred. Please try again.');
        }
      },
    });
  }
}