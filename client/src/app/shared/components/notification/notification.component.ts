import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../../core/services/notification.service';
import { Notification } from '../../../core/models/notification.model';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification.component.html',
  styleUrl: './notification.component.scss'
})
export class NotificationComponent {
  private readonly notificationService = inject(NotificationService);

  protected readonly notifications = this.notificationService.notifications;

  protected dismiss(id: string): void {
    this.notificationService.dismiss(id);
  }

  protected handleAction(notification: Notification): void {
    if (notification.action) {
      notification.action.callback();
      this.dismiss(notification.id);
    }
  }

  protected getIcon(type: Notification['type']): string {
    switch (type) {
      case 'success':
        return '✓';
      case 'error':
        return '✕';
      case 'warning':
        return '⚠';
      case 'info':
        return 'ℹ';
    }
  }

  protected trackById(_: number, notification: Notification): string {
    return notification.id;
  }
}
