export type NotificationType = 'success' | 'error' | 'warning' | 'info';

export interface Notification {
  id: string;
  type: NotificationType;
  title: string;
  message: string;
  duration?: number;
  dismissible?: boolean;
}

export interface NotificationConfig {
  title: string;
  message: string;
  duration?: number;
  dismissible?: boolean;
}
