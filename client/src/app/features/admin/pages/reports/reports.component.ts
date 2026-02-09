import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReportService } from '../../../../core/services/report.service';
import {
  DashboardReportResponse,
  SalesChartDataPoint,
  UserGrowthDataPoint
} from '../../../../core/models/report.model';
import { getImageUrl, getInitials } from '../../../../shared/utils/image.utils';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, DecimalPipe, FormsModule],
  templateUrl: './reports.component.html',
  styleUrl: './reports.component.scss'
})
export class ReportsComponent implements OnInit {
  private readonly reportService = inject(ReportService);

  protected readonly report = signal<DashboardReportResponse | null>(null);
  protected readonly isLoading = signal(true);
  protected readonly error = signal<string | null>(null);

  // PDF Export
  protected exportStartDate = '';
  protected exportEndDate = '';
  protected readonly isExporting = signal(false);

  // Chart dimensions
  protected readonly chartWidth = 800;
  protected readonly chartHeight = 300;
  protected readonly chartPadding = 40;

  ngOnInit(): void {
    this.loadReport();
    // Default date range: last 30 days
    const end = new Date();
    const start = new Date();
    start.setDate(start.getDate() - 30);
    this.exportStartDate = start.toISOString().split('T')[0];
    this.exportEndDate = end.toISOString().split('T')[0];
  }

  protected loadReport(): void {
    this.isLoading.set(true);
    this.error.set(null);

    this.reportService.getDashboardReport().subscribe({
      next: (data) => {
        this.report.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Greška pri učitavanju izvještaja');
        this.isLoading.set(false);
      }
    });
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

  protected exportOrdersPdf(): void {
    if (!this.exportStartDate || !this.exportEndDate) return;
    this.isExporting.set(true);
    this.reportService.exportOrdersReport(this.exportStartDate, this.exportEndDate).subscribe({
      next: (blob) => {
        this.downloadBlob(blob, `Reignite_Narudzbe_${this.exportStartDate}_${this.exportEndDate}.pdf`);
        this.isExporting.set(false);
      },
      error: () => {
        this.error.set('Greška pri generiranju PDF izvještaja.');
        this.isExporting.set(false);
      }
    });
  }

  protected exportRevenuePdf(): void {
    if (!this.exportStartDate || !this.exportEndDate) return;
    this.isExporting.set(true);
    this.reportService.exportRevenueReport(this.exportStartDate, this.exportEndDate).subscribe({
      next: (blob) => {
        this.downloadBlob(blob, `Reignite_Prihodi_${this.exportStartDate}_${this.exportEndDate}.pdf`);
        this.isExporting.set(false);
      },
      error: () => {
        this.error.set('Greška pri generiranju PDF izvještaja.');
        this.isExporting.set(false);
      }
    });
  }

  private downloadBlob(blob: Blob, filename: string): void {
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();
    window.URL.revokeObjectURL(url);
  }
}
