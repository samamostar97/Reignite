import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { OrderService } from '../../../../core/services/order.service';
import { OrderResponse, OrderStatus } from '../../../../core/models/order.model';
import { getImageUrl, getInitials } from '../../../../shared/utils/image.utils';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './order-list.component.html',
  styleUrl: './order-list.component.scss'
})
export class OrderListComponent implements OnInit, OnDestroy {
  private readonly orderService = inject(OrderService);
  private readonly destroy$ = new Subject<void>();
  private readonly searchSubject = new Subject<string>();

  protected readonly orders = signal<OrderResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly totalCount = signal(0);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly searchQuery = signal('');
  protected readonly statusFilter = signal<OrderStatus | null>(null);

  // Modal state
  protected readonly selectedOrder = signal<OrderResponse | null>(null);
  protected readonly orderDetail = signal<OrderResponse | null>(null);
  protected readonly isLoadingDetail = signal(false);

  protected readonly totalPages = computed(() =>
    Math.ceil(this.totalCount() / this.pageSize())
  );

  protected readonly OrderStatus = OrderStatus;

  ngOnInit() {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(query => {
      this.searchQuery.set(query);
      this.currentPage.set(1);
      this.loadOrders();
    });

    this.loadOrders();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadOrders() {
    this.isLoading.set(true);
    this.orderService.getOrders({
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      search: this.searchQuery() || undefined,
      status: this.statusFilter() ?? undefined
    }).subscribe({
      next: (result) => {
        this.orders.set(result.items);
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

  protected onStatusFilter(status: OrderStatus | null): void {
    this.statusFilter.set(status);
    this.currentPage.set(1);
    this.loadOrders();
  }

  protected goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadOrders();
  }

  protected getImageUrl = getImageUrl;

  protected getInitialsFromName(name: string): string {
    const parts = name.split(' ');
    return getInitials(parts[0] || '', parts[1] || '');
  }

  protected getStatusClass(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Processing:
        return 'status-processing';
      case OrderStatus.OnDelivery:
        return 'status-delivery';
      case OrderStatus.Delivered:
        return 'status-delivered';
      case OrderStatus.Cancelled:
        return 'status-cancelled';
      default:
        return '';
    }
  }

  protected getStatusName(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Processing:
        return 'U obradi';
      case OrderStatus.OnDelivery:
        return 'Dostava';
      case OrderStatus.Delivered:
        return 'Dostavljeno';
      case OrderStatus.Cancelled:
        return 'Otkazano';
      default:
        return 'Nepoznato';
    }
  }

  protected updateOrderStatus(orderId: number, newStatus: OrderStatus): void {
    this.orderService.updateOrder(orderId, { status: newStatus }).subscribe({
      next: (updatedOrder) => {
        // Update the order in the list
        const orders = this.orders();
        const index = orders.findIndex(o => o.id === orderId);
        if (index !== -1) {
          orders[index] = updatedOrder;
          this.orders.set([...orders]);
        }
        // Update detail view if open
        if (this.orderDetail()?.id === orderId) {
          this.orderDetail.set(updatedOrder);
        }
      },
      error: (error) => {
        console.error('Greška prilikom ažuriranja statusa:', error);
      }
    });
  }

  protected getAvailableStatuses(currentStatus: OrderStatus): OrderStatus[] {
    switch (currentStatus) {
      case OrderStatus.Processing:
        return [OrderStatus.OnDelivery, OrderStatus.Cancelled];
      case OrderStatus.OnDelivery:
        return [OrderStatus.Delivered, OrderStatus.Cancelled];
      case OrderStatus.Delivered:
        return []; // Delivered orders cannot change status
      case OrderStatus.Cancelled:
        return []; // Cancelled orders cannot change status
      default:
        return [];
    }
  }

  protected formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('bs-BA', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  protected formatCurrency(amount: number): string {
    return amount.toFixed(2) + ' KM';
  }

  protected viewOrder(order: OrderResponse): void {
    this.selectedOrder.set(order);
    this.isLoadingDetail.set(true);
    this.orderDetail.set(null);

    this.orderService.getOrderById(order.id).subscribe({
      next: (detail) => {
        this.orderDetail.set(detail);
        this.isLoadingDetail.set(false);
      },
      error: () => {
        this.isLoadingDetail.set(false);
        this.closeModal();
      }
    });
  }

  protected closeModal(): void {
    this.selectedOrder.set(null);
    this.orderDetail.set(null);
  }
}
