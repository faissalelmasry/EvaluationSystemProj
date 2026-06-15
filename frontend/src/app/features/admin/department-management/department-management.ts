import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs';
import { Department } from '../../../core/models/department.model';
import { DepartmentService } from '../../../core/services/department';
import { DepartmentForm } from '../department-form/department-form';

@Component({
  selector: 'app-department-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DepartmentForm],
  templateUrl: './department-management.html',
  styleUrls: ['./department-management.css'],
})
export class DepartmentManagement implements OnInit {
  private departmentService = inject(DepartmentService);

  departments = signal<Department[]>([]);
  loading = signal(false);
  totalCount = signal(0);
  pageNumber = signal(1);
  pageSize = signal(10);
  showModal = signal(false);
  selectedDepartment = signal<Department | null>(null);
  actionLoadingId = signal<number | null>(null);

  searchControl = new FormControl('', { nonNullable: true });

  get pageCount() {
    return Math.max(1, Math.ceil(this.totalCount() / this.pageSize()));
  }

  get showingTo(): number {
    return Math.min(this.pageNumber() * this.pageSize(), this.totalCount());
  }

  ngOnInit(): void {
    this.loadDepartments();
    this.listenToSearch();
  }

  loadDepartments() {
    this.loading.set(true);
    this.departmentService
      .getDepartments(this.pageNumber(), this.pageSize(), this.searchControl.value)
      .subscribe({
        next: (res) => {
          this.departments.set(res.items);
          this.totalCount.set(res.totalCount);
          this.loading.set(false);
        },
        error: () => this.loading.set(false),
      });
  }

  listenToSearch() {
    this.searchControl.valueChanges
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        switchMap((search) => {
          this.pageNumber.set(1);
          return this.departmentService.getDepartments(1, this.pageSize(), search);
        })
      )
      .subscribe({
        next: (res) => {
          this.departments.set(res.items);
          this.totalCount.set(res.totalCount);
        },
      });
  }

  nextPage() {
    if (this.pageNumber() < this.pageCount) {
      this.pageNumber.update(v => v + 1);
      this.loadDepartments();
    }
  }

  previousPage() {
    if (this.pageNumber() > 1) {
      this.pageNumber.update(v => v - 1);
      this.loadDepartments();
    }
  }

  openCreateModal() {
    this.selectedDepartment.set(null);
    this.showModal.set(true);
  }

  openEditModal(dep: Department) {
    this.selectedDepartment.set(dep);
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.selectedDepartment.set(null);
  }

  onSaved() {
    this.closeModal();
    this.loadDepartments();
  }

  deleteDepartment(dep: Department) {
    if (!confirm(`Delete department "${dep.name}"? This cannot be undone.`)) return;
    this.actionLoadingId.set(dep.id);
    this.departmentService.delete(dep.id).subscribe({
      next: () => {
        this.actionLoadingId.set(null);
        this.loadDepartments();
      },
      error: () => this.actionLoadingId.set(null),
    });
  }
}