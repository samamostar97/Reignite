import { Component, inject, signal, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductService } from '../../../../core/services/product.service';
import { CategoryService } from '../../../../core/services/category.service';
import { ProjectService } from '../../../../core/services/project.service';
import { UserService } from '../../../../core/services/user.service';
import { ActivityService } from '../../../../core/services/activity.service';
import { ActivityResponse, ActivityType } from '../../../../core/models/activity.model';
import { environment } from '../../../../../environments/environment';

interface StatCard {
  label: string;
  value: string | number;
  icon: string;
  route: string;
  color: string;
  secondaryColor: string;
  rgb: string;
  loading?: boolean;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardComponent implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly projectService = inject(ProjectService);
  private readonly userService = inject(UserService);
  private readonly activityService = inject(ActivityService);

  protected readonly ActivityType = ActivityType;
  protected readonly activities = signal<ActivityResponse[]>([]);
  protected readonly isLoadingActivity = signal(true);
  protected readonly selectedActivityType = signal<ActivityType | null>(null);

  protected readonly stats = signal<StatCard[]>([
    {
      label: 'Proizvodi',
      value: '...',
      icon: 'cube',
      route: '/admin/products',
      color: '#ff6b35',
      secondaryColor: '#f7931e',
      rgb: '255, 107, 53',
      loading: true
    },
    {
      label: 'Kategorije',
      value: '...',
      icon: 'tag',
      route: '/admin/categories',
      color: '#f7931e',
      secondaryColor: '#ff6b35',
      rgb: '247, 147, 30',
      loading: true
    },
    {
      label: 'Projekti',
      value: '...',
      icon: 'photo',
      route: '/admin/projects',
      color: '#6366f1',
      secondaryColor: '#8b5cf6',
      rgb: '99, 102, 241',
      loading: true
    },
    {
      label: 'Korisnici',
      value: '...',
      icon: 'users',
      route: '/admin/users',
      color: '#5a3a2a',
      secondaryColor: '#2c1810',
      rgb: '90, 58, 42',
      loading: true
    },
  ]);

  ngOnInit() {
    this.loadStats();
    this.loadActivity();
    // Preload products for faster navigation to products page
    this.productService.preloadProducts();
  }

  private loadActivity() {
    this.isLoadingActivity.set(true);
    const filter: { pageNumber: number; pageSize: number; type?: ActivityType } = {
      pageNumber: 1,
      pageSize: 8
    };
    const selectedType = this.selectedActivityType();
    if (selectedType !== null) {
      filter.type = selectedType;
    }
    this.activityService.getActivities(filter).subscribe({
      next: (result) => {
        this.activities.set(result.items);
        this.isLoadingActivity.set(false);
      },
      error: () => {
        this.isLoadingActivity.set(false);
      }
    });
  }

  protected filterActivity(type: ActivityType | null) {
    if (this.selectedActivityType() === type) return;
    this.selectedActivityType.set(type);
    this.loadActivity();
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

  protected formatTimeAgo(dateString: string): string {
    const date = new Date(dateString);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return 'Upravo';
    if (diffMins < 60) return `Prije ${diffMins} min`;
    if (diffHours < 24) return `Prije ${diffHours}h`;
    if (diffDays < 7) return `Prije ${diffDays} dana`;
    return date.toLocaleDateString('bs-BA');
  }

  protected getActivityIcon(type: ActivityType): string {
    switch (type) {
      case ActivityType.ProductReview: return 'star';
      case ActivityType.ProjectReview: return 'chat';
      case ActivityType.NewProject: return 'photo';
      default: return 'activity';
    }
  }

  protected getActivityColor(type: ActivityType): string {
    switch (type) {
      case ActivityType.ProductReview: return '#f7931e';
      case ActivityType.ProjectReview: return '#6366f1';
      case ActivityType.NewProject: return '#10b981';
      default: return '#ff6b35';
    }
  }

  private loadStats() {
    // Load products count
    this.productService.getProducts({ pageSize: 1 }).subscribe({
      next: (result) => {
        this.updateStat(0, result.totalCount);
      },
      error: () => this.updateStat(0, 'Greška')
    });

    // Load categories count
    this.categoryService.getCategories({ pageSize: 1 }).subscribe({
      next: (result) => {
        this.updateStat(1, result.totalCount);
      },
      error: () => this.updateStat(1, 'Greška')
    });

    // Load projects count
    this.projectService.getProjects({ pageSize: 1 }).subscribe({
      next: (result) => {
        this.updateStat(2, result.totalCount);
      },
      error: () => this.updateStat(2, 'Greška')
    });

    // Load users count
    this.userService.getUsers({ pageSize: 1 }).subscribe({
      next: (result) => {
        this.updateStat(3, result.totalCount);
      },
      error: () => this.updateStat(3, 'Greška')
    });
  }

  private updateStat(index: number, value: string | number) {
    const currentStats = this.stats();
    const updated = [...currentStats];
    updated[index] = { ...updated[index], value, loading: false };
    this.stats.set(updated);
  }
}
