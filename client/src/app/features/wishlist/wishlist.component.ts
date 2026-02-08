import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { EmberBackgroundComponent } from '../../shared/components/ember-background/ember-background.component';
import { WishlistStateService } from '../../core/services/wishlist.service';
import { CartService } from '../../core/services/cart.service';
import { getImageUrl } from '../../shared/utils/image.utils';

@Component({
  selector: 'app-wishlist',
  standalone: true,
  imports: [CommonModule, RouterLink, HeaderComponent, EmberBackgroundComponent],
  templateUrl: './wishlist.component.html',
  styleUrl: './wishlist.component.scss'
})
export class WishlistComponent {
  protected readonly wishlistService = inject(WishlistStateService);
  protected readonly cartService = inject(CartService);
  protected readonly getImageUrl = getImageUrl;

  protected removeItem(productId: number): void {
    this.wishlistService.removeItem(productId);
  }

  protected addToCart(productId: number, name: string, price: number, imageUrl: string | null): void {
    this.cartService.addItem({
      id: productId,
      name,
      price,
      productImageUrl: imageUrl ?? undefined
    });
  }

  protected formatPrice(price: number): string {
    return price.toFixed(2);
  }
}
