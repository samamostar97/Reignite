import { Injectable, inject, signal, computed } from '@angular/core';
import { OrderService } from './order.service';
import { OrderResponse } from '../models/order.model';

@Injectable({
  providedIn: 'root'
})
export class OrderNotificationService {
  private readonly orderService = inject(OrderService);
  private readonly LAST_SEEN_KEY = 'admin_last_seen_order_id';

  private readonly _notifications = signal<OrderResponse[]>([]);
  private readonly _isLoading = signal(false);
  private readonly _lastSeenOrderId = signal<number>(this.getStoredLastSeenId());
  private readonly _showCallout = signal(false);

  readonly notifications = this._notifications.asReadonly();
  readonly isLoading = this._isLoading.asReadonly();
  readonly showCallout = this._showCallout.asReadonly();

  readonly unreadCount = computed(() => {
    const lastSeenId = this._lastSeenOrderId();
    return this._notifications().filter(order => order.id > lastSeenId).length;
  });

  readonly hasUnread = computed(() => this.unreadCount() > 0);

  loadNotifications(): void {
    this._isLoading.set(true);
    this.orderService.getOrders({
      pageNumber: 1,
      pageSize: 5,
      orderBy: 'datedesc'
    }).subscribe({
      next: (result) => {
        this._notifications.set(result.items);
        this._isLoading.set(false);
      },
      error: () => {
        this._isLoading.set(false);
      }
    });
  }

  markAllAsRead(): void {
    const notifications = this._notifications();
    if (notifications.length > 0) {
      const latestId = Math.max(...notifications.map(n => n.id));
      this._lastSeenOrderId.set(latestId);
      localStorage.setItem(this.LAST_SEEN_KEY, latestId.toString());
    }
  }

  triggerNewOrderCallout(): void {
    this._showCallout.set(true);
    this.loadNotifications();
    setTimeout(() => this._showCallout.set(false), 4000);
  }

  hideCallout(): void {
    this._showCallout.set(false);
  }

  private getStoredLastSeenId(): number {
    const stored = localStorage.getItem(this.LAST_SEEN_KEY);
    return stored ? parseInt(stored, 10) : 0;
  }
}
