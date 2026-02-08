import { Component, inject, signal, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductService } from '../../../../core/services/product.service';
import { CategoryService } from '../../../../core/services/category.service';
import { ProjectService } from '../../../../core/services/project.service';
import { UserService } from '../../../../core/services/user.service';
import { ActivityService } from '../../../../core/services/activity.service';
import { ReportService } from '../../../../core/services/report.service';
import { ActivityResponse, ActivityType } from '../../../../core/models/activity.model';
import {
  DashboardReportResponse,
  SalesChartDataPoint,
  TopProductResponse,
  RecentOrderResponse,
  RatingOverviewResponse,
  UserGrowthDataPoint
} from '../../../../core/models/report.model';
import { getImageUrl, getInitials } from '../../../../shared/utils/image.utils';

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
  private readonly reportService = inject(ReportService);

  protected readonly ActivityType = ActivityType;

  // Report data
  protected readonly report = signal<DashboardReportResponse | null>(null);
  protected readonly isLoadingReport = signal(true);

  // Stat counts (from individual services)
  protected readonly productCount = signal<number | null>(null);
  protected readonly categoryCount = signal<number | null>(null);
  protected readonly projectCount = signal<number | null>(null);
  protected readonly userCount = signal<number | null>(null);

  // Activity
  protected readonly activities = signal<ActivityResponse[]>([]);
  protected readonly isLoadingActivity = signal(true);
  protected readonly selectedActivityType = signal<ActivityType | null>(null);

  // Chart dimensions
  protected readonly chartWidth = 720;
  protected readonly chartHeight = 240;
  protected readonly chartPadding = 40;

  ngOnInit() {
    this.loadReport();
    this.loadStats();
    this.loadActivity();
    this.productService.preloadProducts();
  }

  private loadReport() {
    this.isLoadingReport.set(true);
    this.reportService.getDashboardReport().subscribe({
      next: (data) => {
        this.report.set(data);
        this.isLoadingReport.set(false);
      },
      error: () => {
        this.isLoadingReport.set(false);
      }
    });
  }

  private loadStats() {
    this.productService.getProducts({ pageSize: 1 }).subscribe({
      next: (result) => this.productCount.set(result.totalCount),
      error: () => this.productCount.set(0)
    });
    this.categoryService.getCategories({ pageSize: 1 }).subscribe({
      next: (result) => this.categoryCount.set(result.totalCount),
      error: () => this.categoryCount.set(0)
    });
    this.projectService.getProjects({ pageSize: 1 }).subscribe({
      next: (result) => this.projectCount.set(result.totalCount),
      error: () => this.projectCount.set(0)
    });
    this.userService.getUsers({ pageSize: 1 }).subscribe({
      next: (result) => this.userCount.set(result.totalCount),
      error: () => this.userCount.set(0)
    });
  }

  private loadActivity() {
    this.isLoadingActivity.set(true);
    const filter: { pageNumber: number; pageSize: number; type?: ActivityType } = {
      pageNumber: 1,
      pageSize: 6
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

  protected formatCurrency(value: number): string {
    return new Intl.NumberFormat('bs-BA', {
      style: 'currency',
      currency: 'BAM',
      minimumFractionDigits: 2
    }).format(value);
  }

  protected formatCompactCurrency(value: number): string {
    if (value >= 1000) {
      return `${(value / 1000).toFixed(1).replace('.0', '')}k KM`;
    }
    return `${Math.round(value)} KM`;
  }

  protected formatShortDate(dateString: string): string {
    const date = new Date(dateString);
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    return `${day}.${month}`;
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

  protected getActivityColor(type: ActivityType): string {
    switch (type) {
      case ActivityType.ProductReview: return '#f7931e';
      case ActivityType.ProjectReview: return '#6366f1';
      case ActivityType.NewProject: return '#10b981';
      default: return '#ff6b35';
    }
  }

  // Sales chart helpers
  protected getSalesChartPath(): string {
    const data = this.report()?.salesChart;
    if (!data || data.length === 0) return '';

    const maxRevenue = Math.max(...data.map(d => d.revenue), 1);
    const width = this.chartWidth - this.chartPadding * 2;
    const height = this.chartHeight - this.chartPadding * 2;

    const points = data.map((d, i) => {
      const x = this.chartPadding + (i / (data.length - 1)) * width;
      const y = this.chartPadding + height - (d.revenue / maxRevenue) * height;
      return `${x},${y}`;
    });

    return `M ${points.join(' L ')}`;
  }

  protected getSalesChartArea(): string {
    const data = this.report()?.salesChart;
    if (!data || data.length === 0) return '';

    const maxRevenue = Math.max(...data.map(d => d.revenue), 1);
    const width = this.chartWidth - this.chartPadding * 2;
    const height = this.chartHeight - this.chartPadding * 2;

    const points = data.map((d, i) => {
      const x = this.chartPadding + (i / (data.length - 1)) * width;
      const y = this.chartPadding + height - (d.revenue / maxRevenue) * height;
      return `${x},${y}`;
    });

    const startX = this.chartPadding;
    const endX = this.chartPadding + width;
    const bottomY = this.chartPadding + height;

    return `M ${startX},${bottomY} L ${points.join(' L ')} L ${endX},${bottomY} Z`;
  }

  protected getChartPoints(): { x: number; y: number; data: SalesChartDataPoint }[] {
    const data = this.report()?.salesChart;
    if (!data || data.length === 0) return [];

    const maxRevenue = Math.max(...data.map(d => d.revenue), 1);
    const width = this.chartWidth - this.chartPadding * 2;
    const height = this.chartHeight - this.chartPadding * 2;

    return data.map((d, i) => ({
      x: this.chartPadding + (i / (data.length - 1)) * width,
      y: this.chartPadding + height - (d.revenue / maxRevenue) * height,
      data: d
    }));
  }

  protected getMaxRevenue(): number {
    const data = this.report()?.salesChart;
    if (!data || data.length === 0) return 0;
    return Math.max(...data.map(d => d.revenue));
  }

  // Top products chart
  protected getMaxProductRevenue(): number {
    const products = this.report()?.topProducts;
    if (!products || products.length === 0) return 0;
    return Math.max(...products.map(p => p.totalRevenue));
  }

  protected getProductBarWidth(revenue: number): number {
    const max = this.getMaxProductRevenue();
    if (max === 0) return 0;
    return (revenue / max) * 100;
  }

  // Rating helpers
  protected getRatingBarWidth(count: number, total: number): number {
    if (total === 0) return 0;
    return (count / total) * 100;
  }

  protected getTotalProductReviews(): number {
    const dist = this.report()?.ratingOverview?.productRatingDistribution;
    if (!dist) return 0;
    return dist.oneStar + dist.twoStar + dist.threeStar + dist.fourStar + dist.fiveStar;
  }

  // Order status helpers
  protected getStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'processing': return 'status-processing';
      case 'delivered': return 'status-delivered';
      case 'cancelled': return 'status-cancelled';
      default: return 'status-processing';
    }
  }

  protected getStatusLabel(status: string): string {
    switch (status.toLowerCase()) {
      case 'processing': return 'U obradi';
      case 'delivered': return 'Isporučeno';
      case 'cancelled': return 'Otkazano';
      default: return status;
    }
  }

  // User growth chart
  protected getUserGrowthPath(): string {
    const data = this.report()?.userGrowth?.growthChart;
    if (!data || data.length === 0) return '';

    const maxUsers = Math.max(...data.map(d => d.totalUsers), 1);
    const width = this.chartWidth - this.chartPadding * 2;
    const height = this.chartHeight - this.chartPadding * 2;

    const points = data.map((d, i) => {
      const x = this.chartPadding + (i / (data.length - 1)) * width;
      const y = this.chartPadding + height - (d.totalUsers / maxUsers) * height;
      return `${x},${y}`;
    });

    return `M ${points.join(' L ')}`;
  }

  protected getUserGrowthArea(): string {
    const data = this.report()?.userGrowth?.growthChart;
    if (!data || data.length === 0) return '';

    const maxUsers = Math.max(...data.map(d => d.totalUsers), 1);
    const width = this.chartWidth - this.chartPadding * 2;
    const height = this.chartHeight - this.chartPadding * 2;

    const points = data.map((d, i) => {
      const x = this.chartPadding + (i / (data.length - 1)) * width;
      const y = this.chartPadding + height - (d.totalUsers / maxUsers) * height;
      return `${x},${y}`;
    });

    const startX = this.chartPadding;
    const endX = this.chartPadding + width;
    const bottomY = this.chartPadding + height;

    return `M ${startX},${bottomY} L ${points.join(' L ')} L ${endX},${bottomY} Z`;
  }

  protected getUserGrowthPoints(): { x: number; y: number; data: UserGrowthDataPoint }[] {
    const data = this.report()?.userGrowth?.growthChart;
    if (!data || data.length === 0) return [];

    const maxUsers = Math.max(...data.map(d => d.totalUsers), 1);
    const width = this.chartWidth - this.chartPadding * 2;
    const height = this.chartHeight - this.chartPadding * 2;

    return data.map((d, i) => ({
      x: this.chartPadding + (i / (data.length - 1)) * width,
      y: this.chartPadding + height - (d.totalUsers / maxUsers) * height,
      data: d
    }));
  }
}
