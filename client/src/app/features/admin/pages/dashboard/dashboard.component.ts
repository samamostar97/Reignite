import { Component, inject, signal, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductService } from '../../../../core/services/product.service';
import { CategoryService } from '../../../../core/services/category.service';
import { ProjectService } from '../../../../core/services/project.service';
import { UserService } from '../../../../core/services/user.service';
import { ActivityService } from '../../../../core/services/activity.service';
import { OrderService } from '../../../../core/services/order.service';
import { OrderNotificationService } from '../../../../core/services/order-notification.service';
import { ActivityResponse, ActivityType } from '../../../../core/models/activity.model';
import { CreateOrderRequest } from '../../../../core/models/order.model';
import { CreateProjectRequest } from '../../../../core/models/project.model';
import { getImageUrl, getInitials } from '../../../../shared/utils/image.utils';

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
  private readonly orderService = inject(OrderService);
  private readonly orderNotificationService = inject(OrderNotificationService);

  protected readonly ActivityType = ActivityType;
  protected readonly activities = signal<ActivityResponse[]>([]);
  protected readonly isLoadingActivity = signal(true);
  protected readonly selectedActivityType = signal<ActivityType | null>(null);

  // Dev tools state
  protected readonly isCreatingOrder = signal(false);
  protected readonly isCreatingProject = signal(false);
  protected readonly devMessage = signal<{ type: 'success' | 'error'; text: string } | null>(null);
  protected readonly showLoadingOverlay = signal(false);

  // Valid IDs from seed data (to avoid FK violations)
  private readonly VALID_USER_IDS = [2, 3, 4]; // Skip admin (1)
  private readonly VALID_PRODUCT_IDS = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15];
  private readonly VALID_HOBBY_IDS = [1, 2, 3, 4, 5, 6];
  private readonly PROJECT_TITLES = [
    'Drvena kutija za nakit', 'Kožna torbica', 'Keramička vaza',
    'Ručno rezbarena figura', 'Metalna skulptura', 'Drvena polica',
    'Kožni remen', 'Keramička zdjela', 'Drveni okvir za sliku'
  ];

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

  protected getImageUrl = getImageUrl;

  protected getInitialsFromName(name: string): string {
    const parts = name.split(' ');
    return getInitials(parts[0] || '', parts[1] || '');
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

  // Dev tools methods
  protected createTestOrder(): void {
    if (this.isCreatingOrder()) return;

    this.isCreatingOrder.set(true);
    this.showLoadingOverlay.set(true);
    this.devMessage.set(null);

    const request: CreateOrderRequest = this.generateRandomOrder();

    this.orderService.createTestOrder(request).subscribe({
      next: (order) => {
        this.isCreatingOrder.set(false);
        this.devMessage.set({ type: 'success', text: `Narudžba #${order.id} kreirana!` });
        this.loadStats();
        // Trigger the notification callout on the topbar
        this.orderNotificationService.triggerNewOrderCallout();
        // Hide overlay after delay so user can see the success message
        setTimeout(() => this.showLoadingOverlay.set(false), 1500);
        setTimeout(() => this.devMessage.set(null), 4000);
      },
      error: () => {
        this.isCreatingOrder.set(false);
        this.showLoadingOverlay.set(false);
        this.devMessage.set({ type: 'error', text: 'Greška pri kreiranju narudžbe' });
        setTimeout(() => this.devMessage.set(null), 3000);
      }
    });
  }

  protected createTestProject(): void {
    if (this.isCreatingProject()) return;

    this.isCreatingProject.set(true);
    this.showLoadingOverlay.set(true);
    this.devMessage.set(null);

    const request: CreateProjectRequest = this.generateRandomProject();

    this.projectService.createProject(request).subscribe({
      next: (project) => {
        this.isCreatingProject.set(false);
        this.devMessage.set({ type: 'success', text: `Projekat "${project.title}" kreiran!` });
        this.loadStats();
        this.loadActivity();
        // Hide overlay after delay so user can see the success message
        setTimeout(() => this.showLoadingOverlay.set(false), 1500);
        setTimeout(() => this.devMessage.set(null), 4000);
      },
      error: () => {
        this.isCreatingProject.set(false);
        this.showLoadingOverlay.set(false);
        this.devMessage.set({ type: 'error', text: 'Greška pri kreiranju projekta' });
        setTimeout(() => this.devMessage.set(null), 3000);
      }
    });
  }

  private generateRandomOrder(): CreateOrderRequest {
    const userId = this.randomFrom(this.VALID_USER_IDS);
    const itemCount = Math.floor(Math.random() * 3) + 1; // 1-3 items
    const usedProducts = new Set<number>();
    const items = [];

    for (let i = 0; i < itemCount; i++) {
      let productId: number;
      do {
        productId = this.randomFrom(this.VALID_PRODUCT_IDS);
      } while (usedProducts.has(productId));
      usedProducts.add(productId);

      items.push({
        productId,
        quantity: Math.floor(Math.random() * 3) + 1 // 1-3 quantity
      });
    }

    return { userId, items };
  }

  private generateRandomProject(): CreateProjectRequest {
    const userId = this.randomFrom(this.VALID_USER_IDS);
    const hobbyId = this.randomFrom(this.VALID_HOBBY_IDS);
    const baseTitle = this.randomFrom(this.PROJECT_TITLES);
    const title = `${baseTitle} #${Date.now().toString(36)}`; // Unique suffix

    return {
      title,
      userId,
      hobbyId,
      description: 'Test projekat kreiran putem admin panela za testiranje.',
      hoursSpent: Math.floor(Math.random() * 20) + 1,
      productId: Math.random() > 0.5 ? this.randomFrom(this.VALID_PRODUCT_IDS) : undefined
    };
  }

  private randomFrom<T>(arr: T[]): T {
    return arr[Math.floor(Math.random() * arr.length)];
  }
}
