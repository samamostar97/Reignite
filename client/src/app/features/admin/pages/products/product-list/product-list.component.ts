import { Component, inject, signal, computed, OnInit, OnDestroy, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { ProductService } from '../../../../../core/services/product.service';
import { ProductResponse } from '../../../../../core/models/product.model';
import { environment } from '../../../../../../environments/environment';
import { ConfirmDialogService } from '../../../../../shared/services/confirm-dialog.service';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProductListComponent implements OnInit, OnDestroy {
  private readonly productService = inject(ProductService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly destroy$ = new Subject<void>();
  private readonly searchSubject = new Subject<string>();

  protected readonly products = signal<ProductResponse[]>([]);
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
      this.loadProducts();
    });

    this.loadProducts();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadProducts() {
    this.isLoading.set(true);
    this.productService.getProducts({
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      search: this.searchQuery() || undefined
    }).subscribe({
      next: (result) => {
        this.products.set(result.items);
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
    this.loadProducts();
  }

  protected getImageUrl(path: string | undefined): string {
    if (!path) return '';
    // If it's already an absolute URL (Unsplash, etc.), return as-is
    if (path.startsWith('http://') || path.startsWith('https://')) {
      return path;
    }
    return `${environment.baseUrl}${path}`;
  }

  protected async deleteProduct(product: ProductResponse) {
    const confirmed = await this.confirmDialog.open({
      title: 'Obriši proizvod',
      message: `Da li ste sigurni da želite obrisati proizvod "${product.name}"?`,
      confirmText: 'Obriši',
      cancelText: 'Otkaži',
      confirmButtonClass: 'danger'
    });

    if (confirmed) {
      this.confirmDialog.setLoading(true);
      this.productService.deleteProduct(product.id).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => {
          this.confirmDialog.close();
          this.loadProducts();
        },
        error: (err) => {
          this.confirmDialog.setError(err.error?.error || 'Greška pri brisanju proizvoda.');
        }
      });
    }
  }
}
