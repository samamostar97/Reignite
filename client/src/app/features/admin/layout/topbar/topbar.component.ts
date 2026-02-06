import { Component, inject, input, computed, signal, HostListener, ElementRef, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { OrderNotificationService } from '../../../../core/services/order-notification.service';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TopbarComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly elementRef = inject(ElementRef);
  private readonly router = inject(Router);
  protected readonly notificationService = inject(OrderNotificationService);

  pageTitle = input<string>('Dashboard');

  protected readonly userFullName = this.authService.userFullName;
  protected readonly currentUser = this.authService.currentUser;
  protected readonly isDropdownOpen = signal(false);
  protected readonly isNotificationsOpen = signal(false);

  protected readonly userRole = computed(() => {
    const role = this.currentUser()?.role;
    if (role === 'Admin' || role === 1) return 'Administrator';
    if (role === 'AppUser' || role === 0) return 'Korisnik';
    return role?.toString() ?? '';
  });

  protected readonly userInitials = computed(() => {
    const user = this.currentUser();
    const first = user?.firstName?.charAt(0) || '';
    const last = user?.lastName?.charAt(0) || '';
    return (first + last).toUpperCase();
  });

  ngOnInit(): void {
    this.notificationService.loadNotifications();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event): void {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.isDropdownOpen.set(false);
      this.isNotificationsOpen.set(false);
    }
  }

  @HostListener('document:keydown', ['$event'])
  onKeydown(event: KeyboardEvent): void {
    if (event.key === 'Escape') {
      this.isDropdownOpen.set(false);
      this.isNotificationsOpen.set(false);
    }
  }

  protected toggleDropdown(): void {
    this.isNotificationsOpen.set(false);
    this.isDropdownOpen.update(v => !v);
  }

  protected toggleNotifications(): void {
    this.isDropdownOpen.set(false);
    const wasOpen = this.isNotificationsOpen();
    this.isNotificationsOpen.update(v => !v);

    if (!wasOpen) {
      this.notificationService.markAllAsRead();
    }
  }

  protected viewAllOrders(): void {
    this.isNotificationsOpen.set(false);
    this.router.navigate(['/admin/orders']);
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

  protected formatCurrency(amount: number): string {
    return amount.toFixed(2) + ' KM';
  }

  protected logout(): void {
    this.isDropdownOpen.set(false);
    this.authService.logout();
  }
}
