import { Component, inject, OnInit, signal } from '@angular/core';
import { UserService } from '../../../core/services/user';
import { User } from '../../../core/models/user.model';
import { DepartmentService } from '../../../core/services/department';
import { Department } from '../../../core/models/department.model';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs';
import { CommonModule } from '@angular/common';
import { UserForm } from '../user-form/user-form';
import { RouterLink } from "@angular/router";

@Component({
  selector: 'app-user-management',
  imports: [CommonModule, ReactiveFormsModule, UserForm, RouterLink],
  templateUrl: './user-management.html',
  styleUrl: './user-management.css',
})
export class UserManagement implements OnInit {
  private userService = inject(UserService);
  private departmentService = inject(DepartmentService);

  users = signal<User[]>([]);
  loading = signal(false);
  totalCount = signal(0);
  pageNumber = signal(1);
  pageSize = signal(5);
  departmentMap = signal<Record<number, string>>({});

  searchControl = new FormControl('', {
    nonNullable: true,
  });

  get pageCount() {
    return Math.max(1, Math.ceil(this.totalCount() / this.pageSize()));
  }

  changePageSize(event: Event) {
    const selectedSize = Number((event.target as HTMLSelectElement).value);
    if (selectedSize && selectedSize !== this.pageSize()) {
      this.pageSize.set(selectedSize);
      this.pageNumber.set(1);
      this.getUsers();
    }
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.pageCount) {
      this.pageNumber.set(page);
      this.getUsers();
    }
  }

  ngOnInit() {
    this.getDepartments();
    this.getUsers();
    this.listenToSearch();
  }

  getDepartments() {
    this.departmentService.getDepartments().subscribe({
      next: (result) => {
        const departments = result.items ?? result;
        const map = departments.reduce((acc: Record<number, string>, department: Department) => {
          acc[department.id] = department.name;
          return acc;
        }, {});

        this.departmentMap.set(map);
      },
      error: (err) => {
        console.error(err);
      },
    });
  }

  listenToSearch() {
    this.searchControl.valueChanges
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        switchMap((search) => {
          this.loading.set(true);
          this.pageNumber.set(1);
          return this.userService.getUsers(this.pageNumber(), this.pageSize(), search);
        }),
      )
      .subscribe({
        next: (res) => {
          this.totalCount.set(res.totalCount);
          this.users.set(res.items);

          this.loading.set(false);
        },

        error: () => {
          this.loading.set(false);
        },
      });
  }

  getUsers() {
    this.loading.set(true);

    this.userService
      .getUsers(this.pageNumber(), this.pageSize(), this.searchControl.value)
      .subscribe({
        next: (res) => {
          this.totalCount.set(res.totalCount ?? (res.items?.length ?? 0));
          this.users.set(res.items ?? res);

          this.loading.set(false);
        },

        error: (err) => {
          console.error(err);

          this.loading.set(false);
        },
      });
  }

  nextPage() {
    this.pageNumber.update((value) => value + 1);

    this.getUsers();
  }

  previousPage() {
    if (this.pageNumber() > 1) {
      this.pageNumber.update((value) => value - 1);

      this.getUsers();
    }
  }
  showCreateModal = signal(false);

  openCreateModal() {
    this.showCreateModal.set(true);
  }

  closeCreateModal() {
    this.showCreateModal.set(false);
  }
}
