import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { HobbyService } from '../../../../core/services/hobby.service';
import { HobbyResponse } from '../../../../core/models/hobby.model';
import { ConfirmDialogService } from '../../../../shared/services/confirm-dialog.service';

@Component({
  selector: 'app-hobby-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './hobby-list.component.html',
  styleUrl: './hobby-list.component.scss'
})
export class HobbyListComponent implements OnInit, OnDestroy {
  private readonly hobbyService = inject(HobbyService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly destroy$ = new Subject<void>();
  private readonly searchSubject = new Subject<string>();

  protected readonly hobbies = signal<HobbyResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly isAdding = signal(false);
  protected readonly editingId = signal<number | null>(null);
  protected readonly totalCount = signal(0);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly searchQuery = signal('');
  protected readonly errorMessage = signal<string | null>(null);

  protected readonly totalPages = computed(() =>
    Math.ceil(this.totalCount() / this.pageSize())
  );

  protected newHobbyName = '';
  protected newHobbyDescription = '';
  protected editHobbyName = '';
  protected editHobbyDescription = '';

  ngOnInit() {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(query => {
      this.searchQuery.set(query);
      this.currentPage.set(1);
      this.loadHobbies();
    });

    this.loadHobbies();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadHobbies() {
    this.isLoading.set(true);
    this.hobbyService.getHobbiesPaged({
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      search: this.searchQuery() || undefined
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: (result) => {
        this.hobbies.set(result.items);
        this.totalCount.set(result.totalCount);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  protected onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchSubject.next(value);
  }

  protected goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadHobbies();
  }

  protected showAddForm() {
    this.isAdding.set(true);
    this.newHobbyName = '';
    this.newHobbyDescription = '';
    this.errorMessage.set(null);
  }

  protected cancelAdd() {
    this.isAdding.set(false);
    this.newHobbyName = '';
    this.newHobbyDescription = '';
    this.errorMessage.set(null);
  }

  protected addHobby() {
    const name = this.newHobbyName.trim();
    const description = this.newHobbyDescription.trim();

    if (!name) {
      this.errorMessage.set('Naziv hobija je obavezan.');
      return;
    }
    if (name.length < 2 || name.length > 100) {
      this.errorMessage.set('Naziv mora imati između 2 i 100 znakova.');
      return;
    }
    if (description && description.length > 500) {
      this.errorMessage.set('Opis može imati najviše 500 znakova.');
      return;
    }

    this.errorMessage.set(null);
    this.hobbyService.createHobby({
      name,
      description: description || undefined
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.cancelAdd();
        this.loadHobbies();
      },
      error: (err) => {
        this.errorMessage.set(err.error?.error || 'Greška pri dodavanju hobija.');
      }
    });
  }

  protected startEdit(hobby: HobbyResponse) {
    this.editingId.set(hobby.id);
    this.editHobbyName = hobby.name;
    this.editHobbyDescription = hobby.description || '';
    this.errorMessage.set(null);
  }

  protected cancelEdit() {
    this.editingId.set(null);
    this.editHobbyName = '';
    this.editHobbyDescription = '';
    this.errorMessage.set(null);
  }

  protected saveEdit(id: number) {
    const name = this.editHobbyName.trim();
    const description = this.editHobbyDescription.trim();

    if (!name) {
      this.errorMessage.set('Naziv hobija je obavezan.');
      return;
    }
    if (name.length < 2 || name.length > 100) {
      this.errorMessage.set('Naziv mora imati između 2 i 100 znakova.');
      return;
    }
    if (description && description.length > 500) {
      this.errorMessage.set('Opis može imati najviše 500 znakova.');
      return;
    }

    this.errorMessage.set(null);
    this.hobbyService.updateHobby(id, {
      name,
      description: description || undefined
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.cancelEdit();
        this.loadHobbies();
      },
      error: (err) => {
        this.errorMessage.set(err.error?.error || 'Greška pri ažuriranju hobija.');
      }
    });
  }

  protected async deleteHobby(hobby: HobbyResponse) {
    const confirmed = await this.confirmDialog.open({
      title: 'Obriši hobi',
      message: `Da li ste sigurni da želite obrisati hobi "${hobby.name}"?`,
      confirmText: 'Obriši',
      cancelText: 'Otkaži',
      confirmButtonClass: 'danger'
    });

    if (confirmed) {
      this.confirmDialog.setLoading(true);
      this.hobbyService.deleteHobby(hobby.id).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => {
          this.confirmDialog.close();
          this.loadHobbies();
        },
        error: (err) => {
          this.confirmDialog.setError(err.error?.error || 'Greška pri brisanju hobija.');
        }
      });
    }
  }
}
