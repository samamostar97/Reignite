import { Injectable, signal, computed, inject, effect } from '@angular/core';
import { CouponResponse } from '../models/coupon.model';
import { AuthService } from './auth.service';

export interface CartItem {
  productId: number;
  productName: string;
  productImageUrl?: string;
  unitPrice: number;
  quantity: number;
}

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly authService = inject(AuthService);

  private readonly _items = signal<CartItem[]>([]);
  private readonly _appliedCoupon = signal<CouponResponse | null>(null);
  private _loadedForUserId: number | null = null;

  constructor() {
    this.ensureCorrectUser();
    effect(() => {
      const userId = this.authService.currentUser()?.id ?? null;
      if (this._loadedForUserId !== userId) {
        this._loadedForUserId = userId;
        this._items.set(this.loadFromStorage());
        this._appliedCoupon.set(null);
      }
    });
  }

  readonly items = this._items.asReadonly();
  readonly count = computed(() => this._items().reduce((sum, item) => sum + item.quantity, 0));
  readonly total = computed(() => this._items().reduce((sum, item) => sum + item.unitPrice * item.quantity, 0));
  readonly isEmpty = computed(() => this._items().length === 0);

  readonly appliedCoupon = this._appliedCoupon.asReadonly();

  readonly discountAmount = computed(() => {
    const coupon = this._appliedCoupon();
    if (!coupon) return 0;
    const subtotal = this.total();
    if (coupon.discountType === 'Percentage') {
      return Math.min(subtotal, subtotal * coupon.discountValue / 100);
    }
    return Math.min(subtotal, coupon.discountValue);
  });

  readonly discountedTotal = computed(() => {
    const result = this.total() - this.discountAmount();
    return result < 0 ? 0 : result;
  });

  private get cartKey(): string {
    const userId = this.authService.currentUser()?.id;
    return userId ? `reignite_cart_${userId}` : 'reignite_cart_guest';
  }

  private ensureCorrectUser(): void {
    const currentUserId = this.authService.currentUser()?.id ?? null;
    if (this._loadedForUserId !== currentUserId) {
      this._loadedForUserId = currentUserId;
      this._items.set(this.loadFromStorage());
      this._appliedCoupon.set(null);
    }
  }

  addItem(product: { id: number; name: string; productImageUrl?: string; price: number }, quantity = 1): void {
    this.ensureCorrectUser();
    const items = [...this._items()];
    const existing = items.find(i => i.productId === product.id);

    if (existing) {
      existing.quantity += quantity;
    } else {
      items.push({
        productId: product.id,
        productName: product.name,
        productImageUrl: product.productImageUrl,
        unitPrice: product.price,
        quantity
      });
    }

    this._items.set(items);
    this.saveToStorage(items);
  }

  removeItem(productId: number): void {
    this.ensureCorrectUser();
    const items = this._items().filter(i => i.productId !== productId);
    this._items.set(items);
    this.saveToStorage(items);
  }

  updateQuantity(productId: number, quantity: number): void {
    if (quantity < 1) {
      this.removeItem(productId);
      return;
    }
    this.ensureCorrectUser();
    const items = this._items().map(i =>
      i.productId === productId ? { ...i, quantity } : i
    );
    this._items.set(items);
    this.saveToStorage(items);
  }

  applyCoupon(coupon: CouponResponse): void {
    this._appliedCoupon.set(coupon);
  }

  removeCoupon(): void {
    this._appliedCoupon.set(null);
  }

  clear(): void {
    this._items.set([]);
    this._appliedCoupon.set(null);
    localStorage.removeItem(this.cartKey);
  }

  private loadFromStorage(): CartItem[] {
    try {
      const data = localStorage.getItem(this.cartKey);
      return data ? JSON.parse(data) : [];
    } catch {
      return [];
    }
  }

  private saveToStorage(items: CartItem[]): void {
    localStorage.setItem(this.cartKey, JSON.stringify(items));
  }
}
