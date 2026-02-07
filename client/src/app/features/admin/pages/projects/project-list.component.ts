import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { ProjectService } from '../../../../core/services/project.service';
import { ProjectResponse } from '../../../../core/models/project.model';
import { environment } from '../../../../../environments/environment';
import { ConfirmDialogService } from '../../../../shared/services/confirm-dialog.service';

@Component({
  selector: 'app-project-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './project-list.component.html',
  styleUrl: './project-list.component.scss'
})
export class ProjectListComponent implements OnInit, OnDestroy {
  private readonly projectService = inject(ProjectService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly destroy$ = new Subject<void>();
  private readonly searchSubject = new Subject<string>();

  protected readonly projects = signal<ProjectResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly totalCount = signal(0);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly searchQuery = signal('');
  protected readonly errorMessage = signal<string | null>(null);

  // Modal state
  protected readonly selectedProject = signal<ProjectResponse | null>(null);
  protected readonly projectDetail = signal<ProjectResponse | null>(null);
  protected readonly isLoadingDetail = signal(false);

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
      this.loadProjects();
    });

    this.loadProjects();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadProjects() {
    this.isLoading.set(true);
    this.projectService.getProjects({
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      search: this.searchQuery() || undefined
    }).subscribe({
      next: (result) => {
        this.projects.set(result.items);
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
    this.loadProjects();
  }

  protected getImageUrl(path: string | undefined): string {
    if (!path) return '';
    // If it's already an absolute URL (Unsplash, etc.), return as-is
    if (path.startsWith('http://') || path.startsWith('https://')) {
      return path;
    }
    return `${environment.baseUrl}${path}`;
  }

  protected async deleteProject(project: ProjectResponse) {
    const confirmed = await this.confirmDialog.open({
      title: 'Obriši projekat',
      message: `Da li ste sigurni da želite obrisati projekat "${project.title}"?`,
      confirmText: 'Obriši',
      cancelText: 'Otkaži',
      confirmButtonClass: 'danger'
    });

    if (confirmed) {
      this.confirmDialog.setLoading(true);
      this.projectService.deleteProject(project.id).pipe(
        takeUntil(this.destroy$)
      ).subscribe({
        next: () => {
          this.confirmDialog.close();
          this.loadProjects();
        },
        error: (err) => {
          this.confirmDialog.setError(err.error?.error || 'Greška pri brisanju projekta.');
        }
      });
    }
  }

  protected viewProject(project: ProjectResponse): void {
    this.selectedProject.set(project);
    this.isLoadingDetail.set(true);
    this.projectDetail.set(null);

    this.projectService.getProjectById(project.id).subscribe({
      next: (detail) => {
        this.projectDetail.set(detail);
        this.isLoadingDetail.set(false);
      },
      error: () => {
        this.isLoadingDetail.set(false);
        this.closeModal();
      }
    });
  }

  protected closeModal(): void {
    this.selectedProject.set(null);
    this.projectDetail.set(null);
  }

  protected getInitials(name: string): string {
    return name
      .split(' ')
      .map(n => n.charAt(0))
      .join('')
      .toUpperCase()
      .slice(0, 2);
  }
}
