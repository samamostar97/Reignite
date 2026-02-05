import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { UserService } from '../../../../core/services/user.service';
import { UserResponse, UserRole } from '../../../../core/models/user.model';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss'
})
export class UserListComponent implements OnInit, OnDestroy {
  private readonly userService = inject(UserService);
  private readonly destroy$ = new Subject<void>();
  private readonly searchSubject = new Subject<string>();

  protected readonly users = signal<UserResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly totalCount = signal(0);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly searchQuery = signal('');

  protected readonly totalPages = computed(() =>
    Math.ceil(this.totalCount() / this.pageSize())
  );

  ngOnInit() {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(query => {
      this.searchQuery.set(query);
      this.currentPage.set(1);
      this.loadUsers();
    });

    this.loadUsers();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadUsers() {
    this.isLoading.set(true);
    this.userService.getUsers({
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      search: this.searchQuery() || undefined
    }).subscribe({
      next: (result) => {
        // Filter out admin users - only show regular users
        const nonAdminUsers = result.items.filter(u => u.role !== UserRole.Admin);
        this.users.set(nonAdminUsers);
        // Adjust total count to exclude admins
        const adminCount = result.items.length - nonAdminUsers.length;
        this.totalCount.set(result.totalCount - adminCount);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  protected onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchSubject.next(value);
  }

  protected goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadUsers();
  }

  protected getRoleName(role: UserRole): string {
    return role === UserRole.Admin ? 'Admin' : 'Korisnik';
  }

  protected getRoleClass(role: UserRole): string {
    return role === UserRole.Admin ? 'role-admin' : 'role-user';
  }

  protected getInitials(user: UserResponse): string {
    return `${user.firstName.charAt(0)}${user.lastName.charAt(0)}`.toUpperCase();
  }

  protected formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('bs-BA', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  protected deleteUser(user: UserResponse) {
    if (confirm(`Da li ste sigurni da želite obrisati korisnika "${user.firstName} ${user.lastName}"?`)) {
      this.userService.deleteUser(user.id).subscribe({
        next: () => {
          this.loadUsers();
        }
      });
    }
  }
}
