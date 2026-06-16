import { Component, EventEmitter, inject, Input, OnChanges, Output, signal, SimpleChanges } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { DepartmentService } from '../../../core/services/department';
import { Department } from '../../../core/models/department.model';

@Component({
  selector: 'app-department-form',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './department-form.html',
  styleUrls: ['./department-form.css'],
})
export class DepartmentForm implements OnChanges {
  private departmentService = inject(DepartmentService);
  private fb = inject(FormBuilder);

  @Input() department: Department | null = null;
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  loading = signal(false);
  errorMessage = signal('');

  form = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    description: [''],
  });

  get isEdit(): boolean {
    return !!this.department;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['department']) {
      if (this.department) {
        this.form.patchValue({
          name: this.department.name,
          description: this.department.description ?? '',
        });
      } else {
        this.form.reset();
      }
    }
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set('');
    const dto = this.form.getRawValue();

    const action$: Observable<any> = this.department
      ? this.departmentService.update(this.department.id, dto)
      : this.departmentService.create(dto);

    action$.subscribe({
      next: () => {
        this.loading.set(false);
        this.saved.emit();
      },
      error: (err: any) => {
        this.loading.set(false);
        this.errorMessage.set(err?.error?.message || 'Operation failed. Please try again.');
      },
    });
  }

  onCancel() {
    this.cancelled.emit();
  }
}