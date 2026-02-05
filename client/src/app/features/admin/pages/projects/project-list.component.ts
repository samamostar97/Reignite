import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProjectService } from '../../../../core/services/project.service';
import { ProjectResponse } from '../../../../core/models/project.model';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-project-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './project-list.component.html',
  styleUrl: './project-list.component.scss'
})
export class ProjectListComponent implements OnInit {
  private readonly projectService = inject(ProjectService);

  protected readonly projects = signal<ProjectResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly totalCount = signal(0);
  protected readonly searchQuery = signal('');

  protected readonly filteredProjects = computed(() => {
    const query = this.searchQuery().toLowerCase().trim();
    const allProjects = this.projects();

    if (!query) {
      return allProjects;
    }

    return allProjects.filter(project =>
      project.title.toLowerCase().includes(query) ||
      project.userName?.toLowerCase().includes(query) ||
      project.description?.toLowerCase().includes(query)
    );
  });

  ngOnInit() {
    this.loadProjects();
  }

  private loadProjects() {
    this.isLoading.set(true);
    this.projectService.getProjects({ pageSize: 50 }).subscribe({
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
    this.searchQuery.set(value);
  }

  protected getImageUrl(path: string | undefined): string {
    if (!path) return '';
    return `${environment.baseUrl}${path}`;
  }

  protected deleteProject(project: ProjectResponse) {
    if (confirm(`Da li ste sigurni da želite obrisati projekat "${project.title}"?`)) {
      this.projectService.deleteProject(project.id).subscribe({
        next: () => this.loadProjects()
      });
    }
  }
}
