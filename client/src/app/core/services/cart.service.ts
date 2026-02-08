import { Injectable, signal, computed } from '@angular/core';

export interface CartItem {
  productId: number;
  productName: string;
  productImageUrl?: string;
  unitPrice: number;
  quantity: number;
}

const CART_KEY = 'reignite_cart';

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly _items = signal<CartItem[]>(this.loadFromStorage());

  readonly items = this._items.asReadonly();
  readonly count = computed(() => this._items().reduce((sum, item) => sum + item.quantity, 0));
  readonly total = computed(() => this._items().reduce((sum, item) => sum + item.unitPrice * item.quantity, 0));
  readonly isEmpty = computed(() => this._items().length === 0);

  addItem(product: { id: number; name: string; productImageUrl?: string; price: number }, quantity = 1): void {
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
    const items = this._items().filter(i => i.productId !== productId);
    this._items.set(items);
    this.saveToStorage(items);
  }

  updateQuantity(productId: number, quantity: number): void {
    if (quantity < 1) {
      this.removeItem(productId);
      return;
    }
    const items = this._items().map(i =>
      i.productId === productId ? { ...i, quantity } : i
    );
    this._items.set(items);
    this.saveToStorage(items);
  }

  clear(): void {
    this._items.set([]);
    localStorage.removeItem(CART_KEY);
  }

  private loadFromStorage(): CartItem[] {
    try {
      const data = localStorage.getItem(CART_KEY);
      return data ? JSON.parse(data) : [];
    } catch {
      return [];
    }
  }

  private saveToStorage(items: CartItem[]): void {
    localStorage.setItem(CART_KEY, JSON.stringify(items));
  }
}
