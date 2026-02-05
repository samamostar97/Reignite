import { Component, inject, input, computed, signal, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.scss'
})
export class TopbarComponent {
  private readonly authService = inject(AuthService);
  private readonly elementRef = inject(ElementRef);

  pageTitle = input<string>('Dashboard');

  protected readonly userFullName = this.authService.userFullName;
  protected readonly currentUser = this.authService.currentUser;
  protected readonly isDropdownOpen = signal(false);

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

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: Event): void {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.isDropdownOpen.set(false);
    }
  }

  @HostListener('document:keydown', ['$event'])
  onKeydown(event: KeyboardEvent): void {
    if (event.key === 'Escape') {
      this.isDropdownOpen.set(false);
    }
  }

  protected toggleDropdown(): void {
    this.isDropdownOpen.update(v => !v);
  }

  protected logout(): void {
    this.isDropdownOpen.set(false);
    this.authService.logout();
  }
}
