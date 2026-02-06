import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { ReportService } from '../../../../core/services/report.service';
import {
  DashboardReportResponse,
  SalesChartDataPoint,
  UserGrowthDataPoint
} from '../../../../core/models/report.model';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, DecimalPipe],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.scss'
})
export class ReportsComponent implements OnInit {
  private readonly reportService = inject(ReportService);

  protected readonly report = signal<DashboardReportResponse | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly error = signal<string | null>(null);

  // Chart dimensions
  protected readonly chartWidth = 800;
  protected readonly chartHeight = 300;
  protected readonly chartPadding = 40;

  ngOnInit(): void {
    this.loadReport();
  }

  protected loadReport(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.reportService.getDashboardReport().subscribe({
      next: (data) => {
        this.report.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.error.set('Greška pri učitavanju izvještaja');
        this.isLoading.set(false);
        console.error(err);
      }
    });
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

  protected formatCurrency(value: number): string {
    return new Intl.NumberFormat('bs-BA', {
      style: 'currency',
      currency: 'BAM',
      minimumFractionDigits: 2
    }).format(value);
  }

  protected formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('bs-BA', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  protected formatShortDate(dateString: string): string {
    const date = new Date(dateString);
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    return `${day}.${month}`;
  }

  protected formatChartCurrency(value: number): string {
    if (value >= 1000) {
      return `${(value / 1000).toFixed(1).replace('.0', '')}k KM`;
    }
    return `${Math.round(value)} KM`;
  }

  // SVG Chart helpers
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

  protected getRatingBarWidth(count: number, total: number): number {
    if (total === 0) return 0;
    return (count / total) * 100;
  }

  protected getTotalProductReviews(): number {
    const dist = this.report()?.ratingOverview?.productRatingDistribution;
    if (!dist) return 0;
    return dist.oneStar + dist.twoStar + dist.threeStar + dist.fourStar + dist.fiveStar;
  }

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

  // User Growth Chart helpers
  protected getMaxTotalUsers(): number {
    const data = this.report()?.userGrowth?.growthChart;
    if (!data || data.length === 0) return 0;
    return Math.max(...data.map(d => d.totalUsers));
  }

  protected getMaxNewUsers(): number {
    const data = this.report()?.userGrowth?.growthChart;
    if (!data || data.length === 0) return 1;
    return Math.max(...data.map(d => d.newUsers), 1);
  }

  protected getUserGrowthChartPath(): string {
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

  protected getUserGrowthChartArea(): string {
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
