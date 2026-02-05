import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs';
import { SidebarComponent } from './sidebar/sidebar.component';
import { TopbarComponent } from './topbar/topbar.component';
import { SidebarService } from '../services/sidebar.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, SidebarComponent, TopbarComponent],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss'
})
export class AdminLayoutComponent {
  private readonly router = inject(Router);
  private readonly sidebarService = inject(SidebarService);

  protected readonly pageTitle = signal('Dashboard');
  protected readonly isSidebarCollapsed = this.sidebarService.isCollapsed;

  private readonly pageTitles: Record<string, string> = {
    '/admin/dashboard': 'Dashboard',
    '/admin/products': 'Proizvodi',
    '/admin/products/new': 'Novi Proizvod',
    '/admin/categories': 'Kategorije',
    '/admin/projects': 'Projekti',
    '/admin/users': 'Korisnici',
    '/admin/orders': 'Narudžbe',
  };

  constructor() {
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe((event) => {
      const url = (event as NavigationEnd).urlAfterRedirects;
      // Check for edit routes
      if (url.includes('/products/') && url.includes('/edit')) {
        this.pageTitle.set('Uredi Proizvod');
      } else {
        this.pageTitle.set(this.pageTitles[url] || 'Admin');
      }
    });

    // Set initial title
    const currentUrl = this.router.url;
    this.pageTitle.set(this.pageTitles[currentUrl] || 'Dashboard');
  }
}
