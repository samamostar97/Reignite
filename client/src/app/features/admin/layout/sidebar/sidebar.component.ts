import { Component, inject, ChangeDetectionStrategy, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, Router, NavigationEnd } from '@angular/router';
import { Subject, filter, takeUntil } from 'rxjs';
import { AuthService } from '../../../../core/services/auth.service';
import { SidebarService } from '../../services/sidebar.service';

interface NavItem {
  label: string;
  route: string;
  icon: string;
  placeholder?: boolean;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarComponent implements OnInit, OnDestroy {
  private readonly authService = inject(AuthService);
  private readonly sidebarService = inject(SidebarService);
  private readonly router = inject(Router);
  private readonly destroy$ = new Subject<void>();

  protected readonly isCollapsed = this.sidebarService.isCollapsed;
  protected readonly isMobileOpen = this.sidebarService.isMobileOpen;

  // Grouped navigation items
  protected readonly mainNavItems: NavItem[] = [
    { label: 'Dashboard', route: '/admin/dashboard', icon: 'chart-bar' },
  ];

  protected readonly contentNavItems: NavItem[] = [
    { label: 'Proizvodi', route: '/admin/products', icon: 'cube' },
    { label: 'Kategorije', route: '/admin/categories', icon: 'tag' },
    { label: 'Dobavljači', route: '/admin/suppliers', icon: 'truck' },
    { label: 'Hobiji', route: '/admin/hobbies', icon: 'sparkles' },
    { label: 'Projekti', route: '/admin/projects', icon: 'photo' },
    { label: 'Recenzije', route: '/admin/reviews', icon: 'star' },
  ];

  protected readonly managementNavItems: NavItem[] = [
    { label: 'Korisnici', route: '/admin/users', icon: 'users' },
    { label: 'Narudžbe', route: '/admin/orders', icon: 'shopping-bag' },
    { label: 'Kuponi', route: '/admin/coupons', icon: 'ticket' },
  ];

  protected readonly reportsNavItems: NavItem[] = [
    { label: 'Izvještaji', route: '/admin/reports', icon: 'document-chart' },
  ];

  ngOnInit(): void {
    // Close mobile sidebar on route navigation
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.sidebarService.closeMobile();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  protected toggleCollapse(): void {
    this.sidebarService.toggle();
  }

  protected closeMobile(): void {
    this.sidebarService.closeMobile();
  }

  protected logout(): void {
    this.authService.logout();
  }
}
