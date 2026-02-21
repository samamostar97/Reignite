import { Component, OnInit, OnDestroy, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CommunityService } from '../../core/services/community.service';
import { HobbyService } from '../../core/services/hobby.service';
import { AuthService } from '../../core/services/auth.service';
import { CommunityUserResponse } from '../../core/models/community.model';
import { HobbyResponse } from '../../core/models/hobby.model';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { EmberBackgroundComponent } from '../../shared/components/ember-background/ember-background.component';
import { getImageUrl, getInitials } from '../../shared/utils/image.utils';

@Component({
  selector: 'app-community',
  standalone: true,
  imports: [CommonModule, RouterLink, HeaderComponent, EmberBackgroundComponent],
  templateUrl: './community.component.html',
  styleUrl: './community.component.scss'
})
export class CommunityComponent implements OnInit, OnDestroy {
  private readonly communityService = inject(CommunityService);
  private readonly hobbyService = inject(HobbyService);
  private readonly authService = inject(AuthService);
  private readonly destroy$ = new Subject<void>();

  // Data signals
  protected readonly hobbies = signal<HobbyResponse[]>([]);
  protected readonly users = signal<CommunityUserResponse[]>([]);
  protected readonly isLoading = signal(true);

  // Pagination
  protected readonly totalCount = signal(0);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(12);

  // Filter
  protected readonly selectedHobbyId = signal<number | null>(null);

  // Computed
  protected readonly totalPages = computed(() =>
    Math.ceil(this.totalCount() / this.pageSize())
  );

  ngOnInit() {
    this.loadHobbies();
    this.loadUsers();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadHobbies() {
    this.hobbyService.getHobbies()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (hobbies) => this.hobbies.set(hobbies),
        error: () => {}
      });
  }

  private loadUsers() {
    this.isLoading.set(true);
    this.communityService.getUsers(
      this.currentPage(),
      this.pageSize(),
      this.selectedHobbyId() ?? undefined
    )
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.users.set(result.items);
          this.totalCount.set(result.totalCount);
          this.isLoading.set(false);
        },
        error: () => this.isLoading.set(false)
      });
  }

  protected selectHobby(hobbyId: number | null): void {
    this.selectedHobbyId.set(hobbyId);
    this.currentPage.set(1);
    this.loadUsers();
  }

  protected goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadUsers();
    window.scrollTo({ top: 300, behavior: 'smooth' });
  }

  protected getImageUrl = getImageUrl;

  protected getInitials = getInitials;

  protected formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('bs-Latn-BA', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }
}
