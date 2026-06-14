import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { UserService } from '../../../core/services/user';
import { DepartmentService } from '../../../core/services/department';

import { Department } from '../../../core/models/department.model';
import { JobTitle } from '../../../core/models/job-title.enum';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './user-form.html',
  styleUrl: './user-form.scss',
})
export class UserForm implements OnInit {
  private userService = inject(UserService);
  private departmentService = inject(DepartmentService);
  private fb = inject(FormBuilder);

  departments = signal<Department[]>([]);

  loading = signal(false);

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

    departmentId: [0, Validators.required],

    jobTitle: [0, Validators.required],

    role: ['', Validators.required],
  });

  ngOnInit(): void {
    this.loadDepartments();
  }

  loadDepartments() {
    this.departmentService.getDepartments().subscribe({
      next: (res) => {
        this.departments.set(res.items ?? res);
      },

      error: (err) => {
        console.log(err.error);
        console.log(err.error.errors);
      },
    });
  }

  onSubmit() {
    if (this.form.invalid) {
      return;
    }

    this.loading.set(true);

    const dto = this.form.getRawValue();

    this.userService.create(dto).subscribe({
      next: () => {
        alert('User created successfully');
        console.log(this.form.value);
        this.form.reset();

        this.loading.set(false);
      },

      error: (err) => {
        console.error(err);
        alert(JSON.stringify(err.error, null, 2));
        this.loading.set(false);
      },
    });
  }
}
