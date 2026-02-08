import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CartService } from '../../core/services/cart.service';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { EmberBackgroundComponent } from '../../shared/components/ember-background/ember-background.component';
import { getImageUrl } from '../../shared/utils/image.utils';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink, HeaderComponent, EmberBackgroundComponent],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss'
})
export class CartComponent {
  protected readonly cartService = inject(CartService);
  protected readonly getImageUrl = getImageUrl;

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

  protected formatPrice(price: number): string {
    return price.toFixed(2);
  }
}
