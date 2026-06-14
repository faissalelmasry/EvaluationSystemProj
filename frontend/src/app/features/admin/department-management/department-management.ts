import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

import {
  debounceTime,
  distinctUntilChanged,
  switchMap,
} from 'rxjs';

import { Department } from '../../../core/models/department.model';
import { DepartmentService } from '../../../core/services/department';

import { DepartmentForm } from '../department-form/department-form';

@Component({
  selector: 'app-department-management',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DepartmentForm,
  ],
  templateUrl: './department-management.html',
  styleUrl: './department-management.css',
})
export class DepartmentManagement implements OnInit {

  private departmentService = inject(DepartmentService);

  departments = signal<Department[]>([]);

  loading = signal(false);

  totalCount = signal(0);

  pageNumber = signal(1);

  pageSize = signal(10);

  showCreateModal = signal(false);

  selectedDepartment = signal<Department | null>(null);

  searchControl = new FormControl('', {
    nonNullable: true,
  });

  ngOnInit(): void {

    this.loadDepartments();

    this.listenToSearch();
  }

  loadDepartments() {

    this.loading.set(true);

    this.departmentService
      .getDepartments(
        this.pageNumber(),
        this.pageSize(),
        this.searchControl.value
      )
      .subscribe({

        next: (res) => {

          this.departments.set(res.items);

          this.totalCount.set(res.totalCount);

          this.loading.set(false);
        },

        error: (err) => {

          console.error(err);

          this.loading.set(false);
        }
      });
  }

  listenToSearch() {

    this.searchControl.valueChanges
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        switchMap((search) =>
          this.departmentService.getDepartments(
            this.pageNumber(),
            this.pageSize(),
            search
          )
        )
      )
      .subscribe({

        next: (res) => {

          this.departments.set(res.items);

          this.totalCount.set(res.totalCount);
        },

        error: (err) => {

          console.error(err);
        }
      });
  }

  nextPage() {

    this.pageNumber.update(value => value + 1);

    this.loadDepartments();
  }

  previousPage() {

    if (this.pageNumber() > 1) {

      this.pageNumber.update(value => value - 1);

      this.loadDepartments();
    }
  }

  openCreateModal() {

    this.selectedDepartment.set(null);

    this.showCreateModal.set(true);
  }

  closeCreateModal() {

    this.showCreateModal.set(false);
  }

  editDepartment(department: Department) {

    this.selectedDepartment.set(department);

    this.showCreateModal.set(true);
  }

  deleteDepartment(id: number) {

    const confirmed = confirm(
      'Are you sure you want to delete this department?'
    );

    if (!confirmed) {
      return;
    }

    this.departmentService
      .delete(id)
      .subscribe({

        next: () => {

          alert('Department deleted successfully');

          this.loadDepartments();
        },

        error: (err) => {

          console.error(err);
        }
      });
  }
}