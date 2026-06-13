import { Component, inject, OnInit, signal } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { UserService } from '../../../core/services/user';
import { DepartmentService } from '../../../core/services/department';

import { Department } from '../../../core/models/department.model';

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

  form = this.fb.nonNullable.group({
    fullName: [
      '',
      [Validators.required, Validators.minLength(3)]
    ],

    email: [
      '',
      [Validators.required, Validators.email]
    ],

    password: [
      '',
      [Validators.required, Validators.minLength(6)]
    ],

    departmentId: [
      0,
      Validators.required
    ],

    jobTitle: [
      0,
      Validators.required
    ],

    role: [
      '',
      Validators.required
    ]
  });

  ngOnInit(): void {
    this.loadDepartments();
  }

  loadDepartments() {

    this.departmentService.getDepartments()
      .subscribe({

        next: (res) => {

          this.departments.set(res.items ?? res);

        },

        error: (err) => {

          console.error(err);

        }

      });

  }

  onSubmit() {

    if (this.form.invalid) {
      return;
    }

    this.loading.set(true);

    const dto = this.form.getRawValue();

    this.userService.create(dto)
      .subscribe({

        next: () => {

          alert('User created successfully');

          this.form.reset();

          this.loading.set(false);

        },

        error: (err) => {

          console.error(err);

          this.loading.set(false);

        }

      });

  }

}