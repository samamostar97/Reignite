import { Component, OnInit, OnDestroy, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { DomSanitizer, SafeStyle } from '@angular/platform-browser';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ProductService } from '../../../core/services/product.service';
import { ProductReviewService } from '../../../core/services/product-review.service';
import { AuthService } from '../../../core/services/auth.service';
import { WishlistStateService } from '../../../core/services/wishlist.service';
import { CartService } from '../../../core/services/cart.service';
import { ProductResponse } from '../../../core/models/product.model';
import { ProductReviewResponse } from '../../../core/models/product-review.model';
import { HeaderComponent } from '../../../shared/components/header/header.component';
import { EmberBackgroundComponent } from '../../../shared/components/ember-background/ember-background.component';
import { getImageUrl, getInitials } from '../../../shared/utils/image.utils';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, HeaderComponent, EmberBackgroundComponent],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly productService = inject(ProductService);
  private readonly reviewService = inject(ProductReviewService);
  private readonly authService = inject(AuthService);
  protected readonly wishlistService = inject(WishlistStateService);
  protected readonly cartService = inject(CartService);
  private readonly sanitizer = inject(DomSanitizer);
  private readonly destroy$ = new Subject<void>();

  protected readonly isAuthenticated = this.authService.isAuthenticated;

  protected readonly product = signal<ProductResponse | null>(null);
  protected readonly reviews = signal<ProductReviewResponse[]>([]);
  protected readonly relatedProducts = signal<ProductResponse[]>([]);
  protected readonly averageRating = signal<number>(0);
  protected readonly reviewCount = signal<number>(0);

  protected readonly isLoading = signal(true);
  protected readonly isLoadingReviews = signal(true);
  protected readonly isLoadingRelated = signal(true);
  protected readonly error = signal<string | null>(null);

  ngOnInit() {
    this.wishlistService.loadWishlist();
    this.route.paramMap
      .pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        const id = params.get('id');
        if (id && /^\d+$/.test(id)) {
          this.loadProduct(parseInt(id, 10));
          window.scrollTo({ top: 0, behavior: 'smooth' });
        } else {
          this.router.navigate(['/shop']);
        }
      });
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadProduct(id: number) {
    this.isLoading.set(true);
    this.error.set(null);

    this.productService.getProductById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (product) => {
          this.product.set(product);
          this.isLoading.set(false);
          this.loadReviews(id);
          this.loadRelatedProducts(product.productCategoryId, id);
        },
        error: () => {
          this.error.set('Proizvod nije pronađen');
          this.isLoading.set(false);
        }
      });
  }

  private loadReviews(productId: number) {
    this.isLoadingReviews.set(true);

    this.reviewService.getByProductId(productId, 1, 20)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.reviews.set(result.items);
          this.reviewCount.set(result.totalCount);
          this.calculateAverageRating(result.items);
          this.isLoadingReviews.set(false);
        },
        error: () => {
          this.isLoadingReviews.set(false);
        }
      });
  }

  private calculateAverageRating(reviews: ProductReviewResponse[]) {
    if (reviews.length === 0) {
      this.averageRating.set(0);
      return;
    }
    const sum = reviews.reduce((acc, r) => acc + r.rating, 0);
    this.averageRating.set(sum / reviews.length);
  }

  private loadRelatedProducts(categoryId: number, excludeId: number) {
    this.isLoadingRelated.set(true);

    this.productService.getProducts({
      productCategoryId: categoryId,
      pageSize: 5
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          // Filter out the current product
          const related = result.items.filter(p => p.id !== excludeId).slice(0, 4);
          this.relatedProducts.set(related);
          this.isLoadingRelated.set(false);
        },
        error: () => {
          this.isLoadingRelated.set(false);
        }
      });
  }

  protected getImageUrl = getImageUrl;

  protected getSafeBackgroundImage(path: string | undefined | null): SafeStyle | null {
    const url = getImageUrl(path);
    if (!url) return null;
    return this.sanitizer.bypassSecurityTrustStyle(`url(${url})`);
  }

  protected getInitialsFromName(name: string): string {
    const parts = name.split(' ');
    return getInitials(parts[0] || '', parts[1] || '');
  }

  protected formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('hr-HR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  protected formatPrice(price: number): string {
    return price.toFixed(2);
  }

  protected addToCart(): void {
    const p = this.product();
    if (!p) return;
    this.cartService.addItem(p);
  }
}
