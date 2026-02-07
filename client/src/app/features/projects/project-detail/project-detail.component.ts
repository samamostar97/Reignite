import { Component, OnInit, OnDestroy, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { DomSanitizer, SafeStyle } from '@angular/platform-browser';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ProjectService } from '../../../core/services/project.service';
import { AuthService } from '../../../core/services/auth.service';
import { ProjectResponse } from '../../../core/models/project.model';
import { HeaderComponent } from '../../../shared/components/header/header.component';
import { getImageUrl, getInitials } from '../../../shared/utils/image.utils';

@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, HeaderComponent],
  templateUrl: './project-detail.component.html',
  styleUrl: './project-detail.component.scss'
})
export class ProjectDetailComponent implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly projectService = inject(ProjectService);
  private readonly authService = inject(AuthService);
  private readonly sanitizer = inject(DomSanitizer);
  private readonly destroy$ = new Subject<void>();

  protected readonly isAuthenticated = this.authService.isAuthenticated;

  protected readonly project = signal<ProjectResponse | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly error = signal<string | null>(null);

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
          this.isLoading.set(false);
        },
        error: () => {
          this.error.set('Projekat nije pronađen');
          this.isLoading.set(false);
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
