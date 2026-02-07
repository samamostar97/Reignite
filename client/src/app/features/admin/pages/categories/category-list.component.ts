import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { CategoryService } from '../../../../core/services/category.service';
import { ProductCategoryResponse } from '../../../../core/models/category.model';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.scss'
})
export class CategoryListComponent implements OnInit, OnDestroy {
  private readonly categoryService = inject(CategoryService);
  private readonly destroy$ = new Subject<void>();
  private readonly searchSubject = new Subject<string>();

  protected readonly categories = signal<ProductCategoryResponse[]>([]);
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

  protected newCategoryName = '';
  protected editCategoryName = '';

  ngOnInit() {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(query => {
      this.searchQuery.set(query);
      this.currentPage.set(1);
      this.loadCategories();
    });

    this.loadCategories();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCategories() {
    this.isLoading.set(true);
    this.categoryService.getCategories({
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      search: this.searchQuery() || undefined
    }).subscribe({
      next: (result) => {
        this.categories.set(result.items);
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
    this.loadCategories();
  }

  protected showAddForm() {
    this.isAdding.set(true);
    this.newCategoryName = '';
    this.errorMessage.set(null);
  }

  protected cancelAdd() {
    this.isAdding.set(false);
    this.newCategoryName = '';
    this.errorMessage.set(null);
  }

  protected addCategory() {
    if (!this.newCategoryName.trim()) return;

    this.errorMessage.set(null);
    this.categoryService.createCategory({ name: this.newCategoryName.trim() }).subscribe({
      next: () => {
        this.cancelAdd();
        this.loadCategories();
      },
      error: (err) => {
        this.errorMessage.set(err.error?.error || 'Greška pri dodavanju kategorije.');
      }
    });
  }

  protected startEdit(category: ProductCategoryResponse) {
    this.editingId.set(category.id);
    this.editCategoryName = category.name;
    this.errorMessage.set(null);
  }

  protected cancelEdit() {
    this.editingId.set(null);
    this.editCategoryName = '';
    this.errorMessage.set(null);
  }

  protected saveEdit(id: number) {
    if (!this.editCategoryName.trim()) return;

    this.errorMessage.set(null);
    this.categoryService.updateCategory(id, { name: this.editCategoryName.trim() }).subscribe({
      next: () => {
        this.cancelEdit();
        this.loadCategories();
      },
      error: (err) => {
        this.errorMessage.set(err.error?.error || 'Greška pri ažuriranju kategorije.');
      }
    });
  }

  protected deleteCategory(category: ProductCategoryResponse) {
    if (confirm(`Da li ste sigurni da želite obrisati kategoriju "${category.name}"?`)) {
      this.categoryService.deleteCategory(category.id).subscribe({
        next: () => this.loadCategories(),
        error: (err) => {
          this.errorMessage.set(err.error?.error || 'Greška pri brisanju kategorije.');
        }
      });
    }
  }
}
