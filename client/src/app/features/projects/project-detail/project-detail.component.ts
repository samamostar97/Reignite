import { Component, OnInit, OnDestroy, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DomSanitizer, SafeStyle } from '@angular/platform-browser';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ProjectService } from '../../../core/services/project.service';
import { ProjectReviewService } from '../../../core/services/project-review.service';
import { AuthService } from '../../../core/services/auth.service';
import { ProjectResponse, ProjectReviewResponse } from '../../../core/models/project.model';
import { HeaderComponent } from '../../../shared/components/header/header.component';
import { EmberBackgroundComponent } from '../../../shared/components/ember-background/ember-background.component';
import { getImageUrl, getInitials } from '../../../shared/utils/image.utils';

@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, HeaderComponent, EmberBackgroundComponent],
  templateUrl: './project-detail.component.html',
  styleUrl: './project-detail.component.scss'
})
export class ProjectDetailComponent implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly projectService = inject(ProjectService);
  private readonly reviewService = inject(ProjectReviewService);
  private readonly authService = inject(AuthService);
  private readonly sanitizer = inject(DomSanitizer);
  private readonly destroy$ = new Subject<void>();

  protected readonly isAuthenticated = this.authService.isAuthenticated;

  protected readonly project = signal<ProjectResponse | null>(null);
  protected readonly reviews = signal<ProjectReviewResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly hasReviewed = signal(false);
  protected readonly isSubmittingReview = signal(false);
  protected readonly reviewError = signal<string | null>(null);

  // Review form
  protected reviewRating = 0;
  protected reviewComment = '';
  protected readonly hoverRating = signal(0);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id && /^\d+$/.test(id)) {
      this.loadProject(parseInt(id, 10));
    } else {
      this.router.navigate(['/projects']);
    }
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadProject(id: number) {
    this.isLoading.set(true);
    this.error.set(null);

    this.projectService.getProjectById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (project) => {
          this.project.set(project);
          this.reviews.set(project.reviews || []);
          this.isLoading.set(false);
          if (this.isAuthenticated()) {
            this.checkHasReviewed(id);
          }
        },
        error: () => {
          this.error.set('Projekat nije pronađen');
          this.isLoading.set(false);
        }
      });
  }

  private checkHasReviewed(projectId: number): void {
    this.reviewService.hasUserReviewed(projectId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => this.hasReviewed.set(result),
        error: () => {}
      });
  }

  protected setRating(rating: number): void {
    this.reviewRating = rating;
  }

  protected submitReview(): void {
    const p = this.project();
    if (!p || this.reviewRating < 1) return;

    this.isSubmittingReview.set(true);
    this.reviewError.set(null);

    this.reviewService.createReview({
      projectId: p.id,
      rating: this.reviewRating,
      comment: this.reviewComment || undefined
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (newReview) => {
          this.reviews.set([newReview, ...this.reviews()]);
          this.hasReviewed.set(true);
          this.reviewRating = 0;
          this.reviewComment = '';
          this.isSubmittingReview.set(false);
          // Update project review count/rating
          const updated = { ...p, reviewCount: p.reviewCount + 1 };
          this.project.set(updated);
        },
        error: (err) => {
          this.reviewError.set(err?.error?.message || 'Greška pri slanju recenzije.');
          this.isSubmittingReview.set(false);
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
}
