import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { HobbyService } from '../../../../core/services/hobby.service';
import { HobbyResponse } from '../../../../core/models/hobby.model';

@Component({
  selector: 'app-hobby-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './hobby-list.component.html',
  styleUrl: './hobby-list.component.scss'
})
export class HobbyListComponent implements OnInit, OnDestroy {
  private readonly hobbyService = inject(HobbyService);
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
  }

  protected cancelAdd() {
    this.isAdding.set(false);
    this.newHobbyName = '';
    this.newHobbyDescription = '';
  }

  protected addHobby() {
    if (!this.newHobbyName.trim()) return;

    this.hobbyService.createHobby({
      name: this.newHobbyName.trim(),
      description: this.newHobbyDescription.trim() || undefined
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.cancelAdd();
        this.loadHobbies();
      }
    });
  }

  protected startEdit(hobby: HobbyResponse) {
    this.editingId.set(hobby.id);
    this.editHobbyName = hobby.name;
    this.editHobbyDescription = hobby.description || '';
  }

  protected cancelEdit() {
    this.editingId.set(null);
    this.editHobbyName = '';
    this.editHobbyDescription = '';
  }

  protected saveEdit(id: number) {
    if (!this.editHobbyName.trim()) return;

    this.hobbyService.updateHobby(id, {
      name: this.editHobbyName.trim(),
      description: this.editHobbyDescription.trim() || undefined
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.cancelEdit();
        this.loadHobbies();
      }
    });
  }

  protected deleteHobby(hobby: HobbyResponse) {
    if (confirm(`Da li ste sigurni da zelite obrisati hobi "${hobby.name}"?`)) {
      this.hobbyService.deleteHobby(hobby.id).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => this.loadHobbies()
      });
    }
  }
}
