import { Component, OnInit, OnDestroy, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DomSanitizer, SafeStyle } from '@angular/platform-browser';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { ProductService } from '../../core/services/product.service';
import { CategoryService } from '../../core/services/category.service';
import { AuthService } from '../../core/services/auth.service';
import { WishlistStateService } from '../../core/services/wishlist.service';
import { ProductResponse } from '../../core/models/product.model';
import { ProductCategoryResponse } from '../../core/models/category.model';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { EmberBackgroundComponent } from '../../shared/components/ember-background/ember-background.component';
import { getImageUrl } from '../../shared/utils/image.utils';

@Component({
  selector: 'app-shop',
  standalone: true,
  imports: [CommonModule, RouterLink, HeaderComponent, EmberBackgroundComponent],
  templateUrl: './shop.component.html',
  styleUrl: './shop.component.scss'
})
export class ShopComponent implements OnInit, OnDestroy {
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly sanitizer = inject(DomSanitizer);
  protected readonly authService = inject(AuthService);
  protected readonly wishlistService = inject(WishlistStateService);
  private readonly destroy$ = new Subject<void>();
  private readonly searchSubject = new Subject<string>();

  // Data signals
  protected readonly products = signal<ProductResponse[]>([]);
  protected readonly categories = signal<ProductCategoryResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly isLoadingCategories = signal(true);

  // Pagination
  protected readonly totalCount = signal(0);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(12);

  // Filters
  protected readonly searchQuery = signal('');
  protected readonly selectedCategoryId = signal<number | null>(null);
  protected readonly sortOption = signal<string>('createdatdesc');

  // Computed
  protected readonly totalPages = computed(() =>
    Math.ceil(this.totalCount() / this.pageSize())
  );

  ngOnInit() {
    // Setup search debounce
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(query => {
      this.searchQuery.set(query);
      this.currentPage.set(1);
      this.loadProducts();
    });

    this.loadCategories();
    this.loadProducts();
    this.wishlistService.loadWishlist();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCategories() {
    this.isLoadingCategories.set(true);
    this.categoryService.getCategories({ pageSize: 100 })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.categories.set(result.items);
          this.isLoadingCategories.set(false);
        },
        error: () => this.isLoadingCategories.set(false)
      });
  }

  private loadProducts() {
    this.isLoading.set(true);
    this.productService.getProducts({
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      search: this.searchQuery() || undefined,
      productCategoryId: this.selectedCategoryId() ?? undefined,
      orderBy: this.sortOption()
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.products.set(result.items);
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

  protected selectCategory(categoryId: number | null): void {
    this.selectedCategoryId.set(categoryId);
    this.currentPage.set(1);
    this.loadProducts();
  }

  protected onSortChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.sortOption.set(value);
    this.currentPage.set(1);
    this.loadProducts();
  }

  protected goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadProducts();
    window.scrollTo({ top: 300, behavior: 'smooth' });
  }

  protected getImageUrl = getImageUrl;

  protected getSafeBackgroundImage(path: string | undefined | null): SafeStyle | null {
    const url = getImageUrl(path);
    if (!url) return null;
    return this.sanitizer.bypassSecurityTrustStyle(`url(${url})`);
  }

  protected formatPrice(price: number): string {
    return price.toFixed(2);
  }

  protected toggleWishlist(event: Event, productId: number): void {
    event.preventDefault();
    event.stopPropagation();
    if (!this.authService.isAuthenticated()) return;
    this.wishlistService.toggle(productId);
  }
}
