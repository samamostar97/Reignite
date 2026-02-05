import { Injectable, signal } from '@angular/core';
import { Notification, NotificationConfig, NotificationType } from '../models/notification.model';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private readonly _notifications = signal<Notification[]>([]);
  readonly notifications = this._notifications.asReadonly();

  private generateId(): string {
    return `${Date.now()}-${Math.random().toString(36).substring(2, 9)}`;
  }

  private show(type: NotificationType, config: NotificationConfig): void {
    const notification: Notification = {
      id: this.generateId(),
      type,
      title: config.title,
      message: config.message,
      duration: config.duration ?? 4000,
      dismissible: config.dismissible ?? true
    };

    this._notifications.update(notifications => [...notifications, notification]);

    if (notification.duration && notification.duration > 0) {
      setTimeout(() => this.dismiss(notification.id), notification.duration);
    }
  }

  success(config: NotificationConfig): void {
    this.show('success', config);
  }

  error(config: NotificationConfig): void {
    this.show('error', { ...config, duration: config.duration ?? 5000 });
  }

  warning(config: NotificationConfig): void {
    this.show('warning', config);
  }

  info(config: NotificationConfig): void {
    this.show('info', config);
  }

  dismiss(id: string): void {
    this._notifications.update(notifications =>
      notifications.filter(n => n.id !== id)
    );
  }

  dismissAll(): void {
    this._notifications.set([]);
  }
}
