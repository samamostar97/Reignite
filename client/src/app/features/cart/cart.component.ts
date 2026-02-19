import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../core/services/cart.service';
import { CouponService } from '../../core/services/coupon.service';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { EmberBackgroundComponent } from '../../shared/components/ember-background/ember-background.component';
import { getImageUrl } from '../../shared/utils/image.utils';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, HeaderComponent, EmberBackgroundComponent],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss'
})
export class CartComponent {
  protected readonly cartService = inject(CartService);
  private readonly couponService = inject(CouponService);
  protected readonly getImageUrl = getImageUrl;

  protected couponCode = '';
  protected readonly couponLoading = signal(false);
  protected readonly couponError = signal<string | null>(null);

  protected updateQuantity(productId: number, event: Event): void {
    const value = parseInt((event.target as HTMLInputElement).value, 10);
    if (value >= 1) {
      this.cartService.updateQuantity(productId, value);
    }
  }

  protected increment(productId: number, currentQty: number): void {
    this.cartService.updateQuantity(productId, currentQty + 1);
  }

  protected decrement(productId: number, currentQty: number): void {
    if (currentQty > 1) {
      this.cartService.updateQuantity(productId, currentQty - 1);
    }
  }

  protected applyCoupon(): void {
    if (!this.couponCode.trim()) return;

    this.couponLoading.set(true);
    this.couponError.set(null);

    this.couponService.validateCoupon({
      code: this.couponCode.trim(),
      orderTotal: this.cartService.total()
    }).subscribe({
      next: (coupon) => {
        this.cartService.applyCoupon(coupon);
        this.couponLoading.set(false);
        this.couponCode = '';
      },
      error: (err) => {
        this.couponLoading.set(false);
        this.couponError.set(err.error?.error || 'Neispravni kupon.');
      }
    });
  }

  protected removeCoupon(): void {
    this.cartService.removeCoupon();
    this.couponError.set(null);
  }

  protected formatPrice(price: number): string {
    return price.toFixed(2);
  }
}
