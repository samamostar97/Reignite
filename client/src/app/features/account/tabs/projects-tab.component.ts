import { Component, inject, signal, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProjectService } from '../../../core/services/project.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { ConfirmDialogService } from '../../../shared/services/confirm-dialog.service';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ProjectResponse } from '../../../core/models/project.model';
import { getImageUrl } from '../../../shared/utils/image.utils';

@Component({
  selector: 'app-projects-tab',
  standalone: true,
  imports: [CommonModule, RouterLink, ConfirmDialogComponent],
  templateUrl: './projects-tab.component.html',
  styleUrl: './projects-tab.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProjectsTabComponent implements OnInit {
  private readonly projectService = inject(ProjectService);
  private readonly authService = inject(AuthService);
  private readonly notification = inject(NotificationService);
  private readonly confirmDialog = inject(ConfirmDialogService);

  protected readonly isLoading = signal(true);
  protected readonly projects = signal<ProjectResponse[]>([]);
  protected readonly totalCount = signal(0);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = 6;

  protected readonly getImageUrl = getImageUrl;

  ngOnInit() {
    this.loadProjects();
  }

  private loadProjects() {
    const userId = this.authService.currentUser()?.id;
    if (!userId) return;

    this.isLoading.set(true);
    this.projectService.getProjects({
      userId,
      pageNumber: this.currentPage(),
      pageSize: this.pageSize
    }).subscribe({
      next: (result) => {
        this.projects.set(result.items);
        this.totalCount.set(result.totalCount);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  protected async deleteProject(project: ProjectResponse): Promise<void> {
    const confirmed = await this.confirmDialog.open({
      title: 'Brisanje projekta',
      message: `Da li ste sigurni da želite obrisati projekat "${project.title}"?`,
      confirmText: 'Obriši',
      cancelText: 'Otkaži'
    });

    if (!confirmed) return;

    this.projectService.deleteProject(project.id).subscribe({
      next: () => {
        this.notification.success({ title: 'Uspjeh', message: 'Projekat je obrisan.' });
        this.loadProjects();
      },
      error: () => {
        this.notification.error({ title: 'Greška', message: 'Greška prilikom brisanja projekta.' });
      }
    });
  }

  protected nextPage() {
    if (this.currentPage() * this.pageSize < this.totalCount()) {
      this.currentPage.set(this.currentPage() + 1);
      this.loadProjects();
    }
  }

  protected prevPage() {
    if (this.currentPage() > 1) {
      this.currentPage.set(this.currentPage() - 1);
      this.loadProjects();
    }
  }

  protected get totalPages(): number {
    return Math.ceil(this.totalCount() / this.pageSize);
  }

  protected formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toLocaleDateString('hr-HR', { day: '2-digit', month: '2-digit', year: 'numeric' });
  }
}
