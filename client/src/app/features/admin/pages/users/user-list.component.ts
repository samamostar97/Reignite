import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { UserService } from '../../../../core/services/user.service';
import { UserResponse, UserRole } from '../../../../core/models/user.model';
import { ConfirmDialogService } from '../../../../shared/services/confirm-dialog.service';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss'
})
export class UserListComponent implements OnInit, OnDestroy {
  private readonly userService = inject(UserService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly destroy$ = new Subject<void>();
  private readonly searchSubject = new Subject<string>();

  protected readonly users = signal<UserResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly totalCount = signal(0);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly searchQuery = signal('');
  protected readonly errorMessage = signal<string | null>(null);

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
        // Admins are now filtered on the backend
        this.users.set(result.items);
        this.totalCount.set(result.totalCount);
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

  protected getImageUrl(path: string | null): string {
    if (!path) return '';
    if (path.startsWith('http://') || path.startsWith('https://')) {
      return path;
    }
    return `${environment.baseUrl}${path}`;
  }

  protected formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('bs-BA', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  protected async deleteUser(user: UserResponse) {
    const confirmed = await this.confirmDialog.open({
      title: 'Obriši korisnika',
      message: `Da li ste sigurni da želite obrisati korisnika "${user.firstName} ${user.lastName}"?`,
      confirmText: 'Obriši',
      cancelText: 'Otkaži',
      confirmButtonClass: 'danger'
    });

    if (confirmed) {
      this.confirmDialog.setLoading(true);
      this.userService.deleteUser(user.id).pipe(
        takeUntil(this.destroy$)
      ).subscribe({
        next: () => {
          this.confirmDialog.close();
          this.loadUsers();
        },
        error: (err) => {
          this.confirmDialog.setError(err.error?.error || 'Greška pri brisanju korisnika.');
        }
      });
    }
  }
}
