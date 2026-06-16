import { Component, inject, OnInit, signal } from '@angular/core';
import { UserService } from '../../../core/services/user';
import { User } from '../../../core/models/user.model';
import { DepartmentService } from '../../../core/services/department';
import { Department } from '../../../core/models/department.model';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs';
import { CommonModule } from '@angular/common';
import { UserForm } from '../user-form/user-form';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, UserForm],
  templateUrl: './user-management.html',
  styleUrls: ['./user-management.css'],
})
export class UserManagement implements OnInit {
  private userService = inject(UserService);
  private departmentService = inject(DepartmentService);

  users = signal<User[]>([]);
  loading = signal(false);
  totalCount = signal(0);
  pageNumber = signal(1);
  pageSize = signal(10);
  departmentMap = signal<Record<number, string>>({});
  showCreateModal = signal(false);
  selectedUser = signal<User | null>(null);
  actionLoadingId = signal<number | null>(null);

  searchControl = new FormControl('', { nonNullable: true });

  roleOptions = ['Admin', 'Evaluator', 'Evaluatee', 'Reviewer'];

  get pageCount() {
    return Math.max(1, Math.ceil(this.totalCount() / this.pageSize()));
  }

  ngOnInit() {
    this.getDepartments();
    this.getUsers();
    this.listenToSearch();
  }

  getDepartments() {
    this.departmentService.getDepartments(1, 100).subscribe({
      next: (result) => {
        const departments = result.items ?? result;
        const map = departments.reduce((acc: Record<number, string>, dep: Department) => {
          acc[dep.id] = dep.name;
          return acc;
        }, {});
        this.departmentMap.set(map);
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
          return this.userService.getUsers(1, this.pageSize(), search);
        })
      )
      .subscribe({
        next: (res) => {
          this.totalCount.set(res.totalCount);
          this.users.set(res.items);
          this.loading.set(false);
        },
        error: () => this.loading.set(false),
      });
  }

  getUsers() {
    this.loading.set(true);
    this.userService
      .getUsers(this.pageNumber(), this.pageSize(), this.searchControl.value)
      .subscribe({
        next: (res) => {
          this.totalCount.set(res.totalCount ?? res.items?.length ?? 0);
          this.users.set(res.items ?? []);
          this.loading.set(false);
        },
        error: () => this.loading.set(false),
      });
  }

  changePageSize(event: Event) {
    const size = Number((event.target as HTMLSelectElement).value);
    if (size && size !== this.pageSize()) {
      this.pageSize.set(size);
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

  nextPage() {
    if (this.pageNumber() < this.pageCount) {
      this.pageNumber.update(v => v + 1);
      this.getUsers();
    }
  }

  previousPage() {
    if (this.pageNumber() > 1) {
      this.pageNumber.update(v => v - 1);
      this.getUsers();
    }
  }

  openCreateModal() {
    this.selectedUser.set(null);
    this.showCreateModal.set(true);
  }

  openEditModal(user: User) {
    this.selectedUser.set(user);
    this.showCreateModal.set(true);
  }

  closeCreateModal() {
    this.showCreateModal.set(false);
    this.selectedUser.set(null);
  }

  onUserSaved() {
    this.closeCreateModal();
    this.getUsers();
  }

  toggleActive(user: User) {
    this.actionLoadingId.set(user.id);
    const action$ = user.isActive
      ? this.userService.deactivate(user.id)
      : this.userService.activate(user.id);

    action$.subscribe({
      next: () => {
        this.actionLoadingId.set(null);
        this.getUsers();
      },
      error: () => this.actionLoadingId.set(null),
    });
  }

  assignRole(user: User, event: Event) {
    const role = (event.target as HTMLSelectElement).value;
    if (!role) return;
    this.actionLoadingId.set(user.id);
    this.userService.assignRole(user.id, role).subscribe({
      next: () => {
        this.actionLoadingId.set(null);
        this.getUsers();
      },
      error: () => this.actionLoadingId.set(null),
    });
  }

  deleteUser(user: User) {
    if (!confirm(`Delete user "${user.fullName || user.email}"? This cannot be undone.`)) return;
    this.userService.delete(user.id).subscribe({
      next: () => this.getUsers(),
    });
  }

  getUserRole(user: User): string {
    if (user.role) return user.role;
    if (user.roles?.length) return user.roles.join(', ');
    return '';
  }
  get showingTo(): number {
  return Math.min(this.pageNumber() * this.pageSize(), this.totalCount());
}
}