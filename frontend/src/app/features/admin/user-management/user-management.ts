import { Component, inject, OnInit, signal } from '@angular/core';
import { UserService } from '../../../core/services/user';
import { User } from '../../../core/models/user.model';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-management',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-management.html',
  styleUrl: './user-management.scss',
})
export class UserManagement implements OnInit {
  private userService = inject(UserService);

  users = signal<User[]>([]);
  loading = signal(false);
  totalCount = signal(0);
  pageNumber = signal(1);
  pageSize = signal(10);

  searchControl = new FormControl('', {
    nonNullable: true,
  });

  ngOnInit() {
    this.getUsers();
    this.listenToSearch();
  }
  listenToSearch() {
    this.searchControl.valueChanges
      .pipe(
        debounceTime(500),
        distinctUntilChanged(),
        switchMap((search) => {
          this.loading.set(true);
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
