import { Component, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthService } from '../../core/services/auth';
import { Router, RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './forgot-password.html',
  styleUrls: ['./forgot-password.css'],
})
export class ForgotPassword {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  loading = signal(false);
  successMessage = signal('');
  errorMessage = signal('');
  emailSent = signal(false);

  form = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
  });

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const dto = { email: this.form.getRawValue().email };

    this.authService.forgotPassword(dto).subscribe({
      next: (res) => {
        this.loading.set(false);
        this.emailSent.set(true);
        this.successMessage.set(res.message || 'Password reset link sent to your email. Please check your inbox.');
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        if (err.status === 404) {
          this.errorMessage.set('No account found with this email address.');
        } else if (err.status === 0) {
          this.errorMessage.set('Unable to reach the server. Please check your connection.');
        } else {
          this.errorMessage.set(err.error?.message || err.error?.Message || 'An error occurred. Please try again.');
        }
      },
    });
  }
}