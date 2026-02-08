import { Component, OnInit, OnDestroy, signal, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DomSanitizer, SafeStyle } from '@angular/platform-browser';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { ProjectService } from '../../core/services/project.service';
import { HobbyService } from '../../core/services/hobby.service';
import { ProjectResponse } from '../../core/models/project.model';
import { HobbyResponse } from '../../core/models/hobby.model';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { getImageUrl } from '../../shared/utils/image.utils';

@Component({
  selector: 'app-projects',
  standalone: true,
  imports: [CommonModule, RouterLink, HeaderComponent],
  templateUrl: './projects.component.html',
  styleUrl: './projects.component.scss'
})
export class ProjectsComponent implements OnInit, OnDestroy {
  private readonly projectService = inject(ProjectService);
  private readonly hobbyService = inject(HobbyService);
  private readonly sanitizer = inject(DomSanitizer);
  private readonly destroy$ = new Subject<void>();
  private readonly searchSubject = new Subject<string>();

  // Ember particles for background effect
  protected readonly embers = Array.from({ length: 25 }, (_, i) => ({
    id: i,
    left: Math.random() * 100,
    delay: Math.random() * 8,
    duration: 8 + Math.random() * 6,
    size: 3 + Math.random() * 4
  }));

  // Data signals
  protected readonly projects = signal<ProjectResponse[]>([]);
  protected readonly hobbies = signal<HobbyResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly isLoadingHobbies = signal(true);

  // Pagination
  protected readonly totalCount = signal(0);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(12);

  // Filters
  protected readonly searchQuery = signal('');
  protected readonly selectedHobbyId = signal<number | null>(null);
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
      this.loadProjects();
    });

    this.loadHobbies();
    this.loadProjects();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadHobbies() {
    this.isLoadingHobbies.set(true);
    this.hobbyService.getHobbies()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (hobbies) => {
          this.hobbies.set(hobbies);
          this.isLoadingHobbies.set(false);
        },
        error: () => this.isLoadingHobbies.set(false)
      });
  }

  private loadProjects() {
    this.isLoading.set(true);
    this.projectService.getProjects({
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      search: this.searchQuery() || undefined,
      hobbyId: this.selectedHobbyId() ?? undefined,
      orderBy: this.sortOption()
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
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

  protected selectHobby(hobbyId: number | null): void {
    this.selectedHobbyId.set(hobbyId);
    this.currentPage.set(1);
    this.loadProjects();
  }

  protected onSortChange(event: Event): void {
    const value = (event.target as HTMLSelectElement).value;
    this.sortOption.set(value);
    this.currentPage.set(1);
    this.loadProjects();
  }

  protected goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadProjects();
    window.scrollTo({ top: 300, behavior: 'smooth' });
  }

  protected getImageUrl = getImageUrl;

  protected getSafeBackgroundImage(path: string | undefined | null): SafeStyle | null {
    const url = getImageUrl(path);
    if (!url) return null;
    return this.sanitizer.bypassSecurityTrustStyle(`url(${url})`);
  }
}
