import { Component, Input, inject, signal, HostListener, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router, NavigationEnd } from '@angular/router';
import { Subject, filter, takeUntil } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { WishlistStateService } from '../../../core/services/wishlist.service';
import { CartService } from '../../../core/services/cart.service';
import { getInitials } from '../../utils/image.utils';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit, OnDestroy {
  private readonly authService = inject(AuthService);
  protected readonly wishlistService = inject(WishlistStateService);
  protected readonly cartService = inject(CartService);
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
    this.wishlistService.loadWishlist();
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

  protected getUserInitials(): string {
    const name = this.userFullName();
    if (!name) return '?';
    const parts = name.split(' ');
    return getInitials(parts[0] || '', parts[1] || '');
  }
}
