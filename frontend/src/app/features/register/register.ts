import { Component, inject, signal } from '@angular/core';
import { AuthService } from '../../core/services/auth';
import { Router, RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { DepartmentService } from '../../core/services/department';
import { Department } from '../../core/models/department.model';
import { JobTitle } from '../../core/models/job-title.enum';

function passwordMatchValidator(group: AbstractControl): ValidationErrors | null {
  const pass = group.get('password')?.value;
  const confirm = group.get('confirmPassword')?.value;
  return pass === confirm ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink],
  templateUrl: './register.html',
  styleUrls: ['./register.scss'],
})
export class Register {
  private readonly authService = inject(AuthService);
  private readonly departmentService = inject(DepartmentService);
  private readonly router = inject(Router);

  loading = signal(false);
  errorMessage = signal('');
  successMessage = signal('');
  departments = signal<Department[]>([]);
  showPassword = signal(false);

  jobTitleOptions = [
    { value: JobTitle.Teacher, label: 'Teacher' },
    { value: JobTitle.Student, label: 'Student' },
    { value: JobTitle.Manager, label: 'Manager' },
    { value: JobTitle.Employee, label: 'Employee' },
    { value: JobTitle.Client, label: 'Client' },
  ];

  form = new FormGroup(
    {
      fullName: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.minLength(3)],
      }),
      email: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.email],
      }),
      password: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required, Validators.minLength(6)],
      }),
      confirmPassword: new FormControl('', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      departmentId: new FormControl(0, {
        nonNullable: true,
        validators: [Validators.required, Validators.min(1)],
      }),
      jobTitle: new FormControl<JobTitle>(JobTitle.Employee, {
        nonNullable: true,
        validators: [Validators.required],
      }),
    },
    { validators: passwordMatchValidator }
  );

  constructor() {
    this.loadDepartments();
  }

  loadDepartments() {
    this.departmentService.getDepartmentsPublic(1, 100).subscribe({
      next: (res) => this.departments.set(res.items),
      error: () => {},
    });
  }

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

    const { confirmPassword, ...dto } = this.form.getRawValue();

    this.authService.register(dto).subscribe({
      next: (res) => {
        this.loading.set(false);
        this.successMessage.set(res.message || 'Registration successful! You can now sign in.');
        setTimeout(() => this.router.navigate(['/login']), 2500);
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(
          err?.error?.message || err?.error?.Message || 'Registration failed. Please try again.'
        );
      },
    });
  }
}
