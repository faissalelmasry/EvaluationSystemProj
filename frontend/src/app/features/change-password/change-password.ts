import { Component, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/services/auth';
import { Router, RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-change-password',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './change-password.html',
  styleUrls: ['./change-password.css'],
})
export class ChangePassword {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  loading = signal(false);
  successMessage = signal('');
  errorMessage = signal('');
  showCurrentPassword = signal(false);
  showNewPassword = signal(false);
  showConfirmPassword = signal(false);

  form = new FormGroup({
    currentPassword: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    newPassword: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.minLength(6)],
    }),
    confirmPassword: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  toggleCurrentPassword() {
    this.showCurrentPassword.update(v => !v);
  }

  toggleNewPassword() {
    this.showNewPassword.update(v => !v);
  }

  toggleConfirmPassword() {
    this.showConfirmPassword.update(v => !v);
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { currentPassword, newPassword, confirmPassword } = this.form.getRawValue();

    if (newPassword !== confirmPassword) {
      this.errorMessage.set('New passwords do not match.');
      return;
    }

    if (currentPassword === newPassword) {
      this.errorMessage.set('New password must be different from current password.');
      return;
    }

    this.loading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    this.authService.changePassword({ currentPassword, newPassword }).subscribe({
      next: (res) => {
        this.loading.set(false);
        this.successMessage.set(res.message || 'Password changed successfully.');
        this.form.reset();
        setTimeout(() => {
          this.router.navigate(['/admin']);
        }, 2000);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        if (err.status === 400) {
          this.errorMessage.set(err.error?.message || err.error?.Message || 'Current password is incorrect.');
        } else if (err.status === 401) {
          this.errorMessage.set('Session expired. Please login again.');
        } else if (err.status === 0) {
          this.errorMessage.set('Unable to reach the server. Please check your connection.');
        } else {
          this.errorMessage.set(err.error?.message || err.error?.Message || 'An error occurred. Please try again.');
        }
      },
    });
  }
}