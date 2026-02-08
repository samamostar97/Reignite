import { Component, inject, signal, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileService } from '../../../core/services/profile.service';
import { OrderResponse, OrderStatus } from '../../../core/models/order.model';
import { getImageUrl } from '../../../shared/utils/image.utils';

@Component({
  selector: 'app-orders-tab',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './orders-tab.component.html',
  styleUrl: './orders-tab.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class OrdersTabComponent implements OnInit {
  private readonly profileService = inject(ProfileService);

  protected readonly isLoading = signal(true);
  protected readonly orders = signal<OrderResponse[]>([]);
  protected readonly totalCount = signal(0);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = 5;
  protected readonly expandedOrderId = signal<number | null>(null);

  ngOnInit() {
    this.loadOrders();
  }

  private loadOrders() {
    this.isLoading.set(true);
    this.profileService.getOrders(this.currentPage(), this.pageSize).subscribe({
      next: (result) => {
        this.orders.set(result.items);
        this.totalCount.set(result.totalCount);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  protected toggleOrder(orderId: number) {
    this.expandedOrderId.set(this.expandedOrderId() === orderId ? null : orderId);
  }

  protected nextPage() {
    if (this.currentPage() * this.pageSize < this.totalCount()) {
      this.currentPage.set(this.currentPage() + 1);
      this.loadOrders();
    }
  }

  protected prevPage() {
    if (this.currentPage() > 1) {
      this.currentPage.set(this.currentPage() - 1);
      this.loadOrders();
    }
  }

  protected get totalPages(): number {
    return Math.ceil(this.totalCount() / this.pageSize);
  }

  protected getStatusClass(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Processing: return 'status-processing';
      case OrderStatus.OnDelivery: return 'status-delivery';
      case OrderStatus.Delivered: return 'status-delivered';
      case OrderStatus.Cancelled: return 'status-cancelled';
      default: return '';
    }
  }

  protected getProductImage(url?: string): string {
    return url ? getImageUrl(url) : '';
  }
}
