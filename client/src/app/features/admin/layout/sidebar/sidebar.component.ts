import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

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
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  private readonly authService = inject(AuthService);

  protected readonly isCollapsed = signal(false);

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
    { label: 'Korisnici', route: '/admin/users', icon: 'users', placeholder: true },
    { label: 'Narudžbe', route: '/admin/orders', icon: 'clipboard', placeholder: true },
  ];

  protected toggleCollapse(): void {
    this.isCollapsed.update(v => !v);
  }

  protected logout(): void {
    this.authService.logout();
  }
}
