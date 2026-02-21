import { Component, OnInit, OnDestroy, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { CommunityService } from '../../../core/services/community.service';
import { PublicUserProfileResponse } from '../../../core/models/community.model';
import { HeaderComponent } from '../../../shared/components/header/header.component';
import { EmberBackgroundComponent } from '../../../shared/components/ember-background/ember-background.component';
import { getImageUrl, getInitials } from '../../../shared/utils/image.utils';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, RouterLink, HeaderComponent, EmberBackgroundComponent],
  templateUrl: './user-profile.component.html',
  styleUrl: './user-profile.component.scss'
})
export class UserProfileComponent implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly communityService = inject(CommunityService);
  private readonly destroy$ = new Subject<void>();

  protected readonly profile = signal<PublicUserProfileResponse | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly error = signal<string | null>(null);

  ngOnInit(): void {
    const userId = this.route.snapshot.paramMap.get('id');
    if (userId && /^\d+$/.test(userId)) {
      this.loadProfile(parseInt(userId, 10));
    } else {
      this.router.navigate(['/community']);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadProfile(userId: number): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.communityService.getUserProfile(userId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (profile) => {
          this.profile.set(profile);
          this.isLoading.set(false);
        },
        error: () => {
          this.error.set('Korisnik nije pronađen.');
          this.isLoading.set(false);
        }
      });
  }

  protected getSkillLevelLabel(level: number): string {
    return ['Početnik', 'Srednji', 'Napredni', 'Ekspert'][level] ?? 'Nepoznato';
  }

  protected formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    return `${day}.${month}.${year}`;
  }

  protected getInitials = getInitials;

  protected getImageUrl = getImageUrl;
}
