import { Component, Input, inject, signal, HostListener, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router, NavigationEnd } from '@angular/router';
import { Subject, filter, takeUntil } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit, OnDestroy {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly destroy$ = new Subject<void>();

  @Input() visible = true;
  @Input() activePath: string | null = null;

  protected readonly isAuthenticated = this.authService.isAuthenticated;
  protected readonly isAdmin = this.authService.isAdmin;
  protected readonly userFullName = this.authService.userFullName;
  protected readonly dropdownOpen = signal(false);
  protected readonly mobileMenuOpen = signal(false);

  ngOnInit(): void {
    // Close mobile menu on route navigation
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      this.mobileMenuOpen.set(false);
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    if (!target.closest('.user-menu')) {
      this.dropdownOpen.set(false);
    }
    // Close mobile menu when clicking outside
    if (!target.closest('.main-header')) {
      this.mobileMenuOpen.set(false);
    }
  }

  protected toggleDropdown(event: Event) {
    event.stopPropagation();
    this.dropdownOpen.update(v => !v);
  }

  protected toggleMobileMenu(event: Event) {
    event.stopPropagation();
    this.mobileMenuOpen.update(v => !v);
  }

  protected logout() {
    this.dropdownOpen.set(false);
    this.mobileMenuOpen.set(false);
    this.authService.logout();
  }

  protected getInitials(): string {
    const name = this.userFullName();
    if (!name) return '?';
    return name
      .split(' ')
      .map(n => n.charAt(0))
      .join('')
      .toUpperCase()
      .slice(0, 2);
  }
}
