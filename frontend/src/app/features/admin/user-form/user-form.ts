import { Component, EventEmitter, inject, Input, OnChanges, OnInit, Output, signal, SimpleChanges } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserService } from '../../../core/services/user';
import { DepartmentService } from '../../../core/services/department';
import { Department } from '../../../core/models/department.model';
import { JobTitle } from '../../../core/models/job-title.enum';
import { User } from '../../../core/models/user.model';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './user-form.html',
  styleUrls: ['./user-form.scss'],
})
export class UserForm implements OnInit, OnChanges {
  private userService = inject(UserService);
  private departmentService = inject(DepartmentService);
  private fb = inject(FormBuilder);

  @Input() editUser: User | null = null;
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  departments = signal<Department[]>([]);
  loading = signal(false);
  errorMessage = signal('');

  jobTitleOptions = [
    { value: JobTitle.Teacher, label: 'Teacher' },
    { value: JobTitle.Student, label: 'Student' },
    { value: JobTitle.Manager, label: 'Manager' },
    { value: JobTitle.Employee, label: 'Employee' },
    { value: JobTitle.Client, label: 'Client' },
  ];

  roleOptions = [
    { value: 'Admin', label: 'Admin' },
    { value: 'Evaluator', label: 'Evaluator' },
    { value: 'Evaluatee', label: 'Evaluatee' },
    { value: 'Reviewer', label: 'Reviewer' },
  ];

  form = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.minLength(3)]],
    username: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    departmentId: [0, [Validators.required, Validators.min(1)]],
    jobTitle: [0, [Validators.required, Validators.min(1)]],
    role: ['', Validators.required],
  });

  get isEdit(): boolean {
    return !!this.editUser;
  }

  ngOnInit(): void {
    this.loadDepartments();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['editUser']) {
      if (this.editUser) {
        this.form.patchValue({
          fullName: this.editUser.fullName || '',
          email: this.editUser.email,
          departmentId: this.editUser.departmentId,
          jobTitle: this.editUser.jobTitle,
          role: this.editUser.role || this.editUser.roles?.[0] || '',
          username: this.editUser.username || '',
        });
        this.form.controls.password.clearValidators();
        this.form.controls.password.updateValueAndValidity();
      } else {
        this.form.reset();
        this.form.controls.password.setValidators([Validators.required, Validators.minLength(6)]);
        this.form.controls.password.updateValueAndValidity();
      }
    }
  }

  loadDepartments() {
    this.departmentService.getDepartments(1, 100).subscribe({
      next: (res) => this.departments.set(res.items ?? []),
    });
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set('');
    const dto = this.form.getRawValue();

    if (this.isEdit && this.editUser) {
      this.userService.update(this.editUser.id, {
        email: dto.email,
        fullName: dto.fullName,
        departmentId: dto.departmentId,
        jobTitle: dto.jobTitle,
        roles: [dto.role],
      }).subscribe({
        next: () => {
          this.loading.set(false);
          this.saved.emit();
        },
        error: (err) => {
          this.loading.set(false);
          this.errorMessage.set(err?.error?.message || 'Failed to update user.');
        },
      });
    } else {
      this.userService.create(dto).subscribe({
        next: () => {
          this.loading.set(false);
          this.saved.emit();
        },
        error: (err) => {
          this.loading.set(false);
          this.errorMessage.set(err?.error?.message || 'Failed to create user.');
        },
      });
    }
  }

  onCancel() {
    this.cancelled.emit();
  }
}