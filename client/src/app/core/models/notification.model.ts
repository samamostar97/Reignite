export type NotificationType = 'success' | 'error' | 'warning' | 'info';

export interface NotificationAction {
  label: string;
  callback: () => void;
}

export interface Notification {
  id: string;
  type: NotificationType;
  title: string;
  message: string;
  duration?: number;
  dismissible?: boolean;
  action?: NotificationAction;
}

export interface NotificationConfig {
  title: string;
  message: string;
  duration?: number;
  dismissible?: boolean;
  action?: NotificationAction;
}
