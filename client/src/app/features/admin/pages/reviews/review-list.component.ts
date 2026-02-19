import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { ProductReviewService } from '../../../../core/services/product-review.service';
import { ProjectReviewService } from '../../../../core/services/project-review.service';
import { ProductReviewResponse } from '../../../../core/models/product-review.model';
import { ProjectReviewResponse } from '../../../../core/models/project.model';
import { getImageUrl, getInitials } from '../../../../shared/utils/image.utils';

type ReviewTab = 'product' | 'project';

interface ReviewItem {
  id: number;
  userId: number;
  userName: string;
  userProfileImageUrl?: string;
  targetName: string;
  rating: number;
  comment?: string;
  createdAt: string;
}

@Component({
  selector: 'app-review-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './review-list.component.html',
  styleUrl: './review-list.component.scss'
})
export class ReviewListComponent implements OnInit, OnDestroy {
  private readonly productReviewService = inject(ProductReviewService);
  private readonly projectReviewService = inject(ProjectReviewService);
  private readonly destroy$ = new Subject<void>();
  private readonly searchSubject = new Subject<string>();

  protected readonly activeTab = signal<ReviewTab>('product');
  protected readonly reviews = signal<ReviewItem[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly totalCount = signal(0);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly searchQuery = signal('');
  protected readonly ratingFilter = signal<number | null>(null);
  protected readonly errorMessage = signal<string | null>(null);

  // Delete confirmation
  protected readonly reviewToDelete = signal<ReviewItem | null>(null);
  protected readonly isDeleting = signal(false);

  protected readonly totalPages = computed(() =>
    Math.ceil(this.totalCount() / this.pageSize())
  );

  protected readonly targetLabel = computed(() =>
    this.activeTab() === 'product' ? 'Proizvod' : 'Projekat'
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

  protected switchTab(tab: ReviewTab): void {
    if (this.activeTab() === tab) return;
    this.activeTab.set(tab);
    this.currentPage.set(1);
    this.ratingFilter.set(null);
    this.errorMessage.set(null);
    this.loadReviews();
  }

  private loadReviews() {
    this.isLoading.set(true);
    const rating = this.ratingFilter();

    if (this.activeTab() === 'product') {
      this.productReviewService.getReviews({
        pageNumber: this.currentPage(),
        pageSize: this.pageSize(),
        minRating: rating ?? undefined,
        maxRating: rating ?? undefined,
        orderBy: 'createdatdesc'
      }).subscribe({
        next: (result) => {
          this.reviews.set(result.items.map(r => this.mapProductReview(r)));
          this.totalCount.set(result.totalCount);
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      });
    } else {
      this.projectReviewService.getReviews({
        pageNumber: this.currentPage(),
        pageSize: this.pageSize(),
        minRating: rating ?? undefined,
        maxRating: rating ?? undefined,
        orderBy: 'createdatdesc'
      }).subscribe({
        next: (result) => {
          this.reviews.set(result.items.map(r => this.mapProjectReview(r)));
          this.totalCount.set(result.totalCount);
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      });
    }
  }

  private mapProductReview(r: ProductReviewResponse): ReviewItem {
    return {
      id: r.id,
      userId: r.userId,
      userName: r.userName,
      userProfileImageUrl: r.userProfileImageUrl,
      targetName: r.productName,
      rating: r.rating,
      comment: r.comment,
      createdAt: r.createdAt
    };
  }

  private mapProjectReview(r: ProjectReviewResponse): ReviewItem {
    return {
      id: r.id,
      userId: r.userId,
      userName: r.userName,
      userProfileImageUrl: r.userProfileImageUrl,
      targetName: r.projectName,
      rating: r.rating,
      comment: r.comment,
      createdAt: r.createdAt
    };
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

  protected getImageUrl = getImageUrl;

  protected getInitialsFromName(name: string): string {
    const parts = name.split(' ');
    return getInitials(parts[0] || '', parts[1] || '');
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

  protected confirmDelete(review: ReviewItem): void {
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

    const deleteObs = this.activeTab() === 'product'
      ? this.productReviewService.deleteReview(review.id)
      : this.projectReviewService.deleteReview(review.id);

    deleteObs.subscribe({
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
