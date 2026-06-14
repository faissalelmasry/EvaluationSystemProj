import { Component, inject, Input, OnInit, signal } from '@angular/core';
import { DepartmentService } from '../../../core/services/department';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Department } from '../../../core/models/department.model';

@Component({
  selector: 'app-department-form',
  imports: [ReactiveFormsModule],
  templateUrl: './department-form.html',
  styleUrl: './department-form.css',
})
export class DepartmentForm implements OnInit {
  private departmentService = inject(DepartmentService);
  private fb = inject(FormBuilder);
  @Input()
  department: Department | null = null;
  loading = signal(false);
  form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    description: [''],
  });
  ngOnInit() {
    if (this.department) {
      this.form.patchValue({
        name: this.department.name,

        description: this.department.description ?? '',
      });
    }
  }

  onSubmit() {
    if (this.form.invalid) {
      return;
    }

    this.loading.set(true);

    const dto = this.form.getRawValue();

    if (this.department) {
      this.departmentService.update(this.department.id, dto).subscribe({
        next: () => {
          alert('Department Updated Successfully');

          this.loading.set(false);
        },

        error: (err) => {
          console.error(err);
          alert(JSON.stringify(err.error, null, 2));
          this.loading.set(false);
        },
      });
    } else {
      this.departmentService.create(dto).subscribe({
        next: () => {
          alert('Department Created Successfully');

          this.form.reset();

          this.loading.set(false);
        },

        error: (err) => {
          console.error(err);

          this.loading.set(false);
        },
      });
    }
  }
}
