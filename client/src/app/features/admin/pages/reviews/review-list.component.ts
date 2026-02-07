import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { ProductReviewService } from '../../../../core/services/product-review.service';
import { ProductReviewResponse } from '../../../../core/models/product-review.model';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-review-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './review-list.component.html',
  styleUrl: './review-list.component.scss'
})
export class ReviewListComponent implements OnInit, OnDestroy {
  private readonly reviewService = inject(ProductReviewService);
  private readonly destroy$ = new Subject<void>();
  private readonly searchSubject = new Subject<string>();

  protected readonly reviews = signal<ProductReviewResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly totalCount = signal(0);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly searchQuery = signal('');
  protected readonly ratingFilter = signal<number | null>(null);
  protected readonly errorMessage = signal<string | null>(null);

  // Delete confirmation
  protected readonly reviewToDelete = signal<ProductReviewResponse | null>(null);
  protected readonly isDeleting = signal(false);

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
      this.loadReviews();
    });

    this.loadReviews();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadReviews() {
    this.isLoading.set(true);
    const rating = this.ratingFilter();
    this.reviewService.getReviews({
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      minRating: rating ?? undefined,
      maxRating: rating ?? undefined,
      orderBy: 'createdatdesc'
    }).subscribe({
      next: (result) => {
        this.reviews.set(result.items);
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

  protected onRatingFilter(rating: number | null): void {
    this.ratingFilter.set(rating);
    this.currentPage.set(1);
    this.loadReviews();
  }

  protected goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadReviews();
  }

  protected getImageUrl(path: string | undefined | null): string {
    if (!path) return '';
    if (path.startsWith('http://') || path.startsWith('https://')) {
      return path;
    }
    return `${environment.baseUrl}${path}`;
  }

  protected getInitials(name: string): string {
    return name
      .split(' ')
      .map(n => n.charAt(0))
      .join('')
      .toUpperCase()
      .slice(0, 2);
  }

  protected formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('bs-BA', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  protected confirmDelete(review: ProductReviewResponse): void {
    this.reviewToDelete.set(review);
  }

  protected cancelDelete(): void {
    this.reviewToDelete.set(null);
  }

  protected deleteReview(): void {
    const review = this.reviewToDelete();
    if (!review) return;

    this.isDeleting.set(true);
    this.errorMessage.set(null);
    this.reviewService.deleteReview(review.id).subscribe({
      next: () => {
        this.reviewToDelete.set(null);
        this.isDeleting.set(false);
        this.loadReviews();
      },
      error: (err) => {
        this.isDeleting.set(false);
        this.reviewToDelete.set(null);
        this.errorMessage.set(err.error?.error || 'Greška pri brisanju recenzije.');
      }
    });
  }
}
