import { Injectable, inject, signal, computed } from '@angular/core';
import { ProfileService } from './profile.service';
import { AuthService } from './auth.service';
import { WishlistItemResponse } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class WishlistStateService {
  private readonly profileService = inject(ProfileService);
  private readonly authService = inject(AuthService);

  private readonly _items = signal<WishlistItemResponse[]>([]);
  private _loadedForUserId: number | null = null;

  readonly items = this._items.asReadonly();
  readonly count = computed(() => this._items().length);

  isInWishlist(productId: number): boolean {
    return this._items().some(item => item.productId === productId);
  }

  loadWishlist(): void {
    if (!this.authService.isAuthenticated()) return;

    const currentUserId = this.authService.currentUser()?.id ?? null;

    // If already loaded for this user, skip
    if (this._loadedForUserId === currentUserId && currentUserId !== null) return;

    this._loadedForUserId = currentUserId;

    this.profileService.getWishlist().subscribe({
      next: (wishlist) => {
        this._items.set(wishlist.items || []);
      },
      error: () => {
        this._items.set([]);
      }
    });
  }

  addItem(productId: number): void {
    this.profileService.addToWishlist(productId).subscribe({
      next: (item) => {
        this._items.set([...this._items(), item]);
      }
    });
  }

  removeItem(productId: number): void {
    this.profileService.removeFromWishlist(productId).subscribe({
      next: () => {
        this._items.set(this._items().filter(i => i.productId !== productId));
      }
    });
  }

  toggle(productId: number): void {
    if (this.isInWishlist(productId)) {
      this.removeItem(productId);
    } else {
      this.addItem(productId);
    }
  }

  reset(): void {
    this._items.set([]);
    this._loadedForUserId = null;
  }
}
