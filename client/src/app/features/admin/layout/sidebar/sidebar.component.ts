import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
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
export class SidebarComponent {
  private readonly authService = inject(AuthService);
  private readonly sidebarService = inject(SidebarService);

  protected readonly isCollapsed = this.sidebarService.isCollapsed;

  // Grouped navigation items
  protected readonly mainNavItems: NavItem[] = [
    { label: 'Dashboard', route: '/admin/dashboard', icon: 'chart-bar' },
  ];

  protected readonly contentNavItems: NavItem[] = [
    { label: 'Proizvodi', route: '/admin/products', icon: 'cube' },
    { label: 'Kategorije', route: '/admin/categories', icon: 'tag' },
    { label: 'Projekti', route: '/admin/projects', icon: 'photo' },
  ];

  protected readonly managementNavItems: NavItem[] = [
    { label: 'Korisnici', route: '/admin/users', icon: 'users' },
    { label: 'Narudžbe', route: '/admin/orders', icon: 'shopping-bag' },
  ];

  protected toggleCollapse(): void {
    this.sidebarService.toggle();
  }

  protected logout(): void {
    this.authService.logout();
  }
}
