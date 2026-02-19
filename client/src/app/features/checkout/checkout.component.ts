import { Component, OnInit, OnDestroy, inject, signal, ElementRef, ViewChild, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CartService } from '../../core/services/cart.service';
import { ProfileService } from '../../core/services/profile.service';
import { PaymentService } from '../../core/services/payment.service';
import { UserAddressResponse, CreateUserAddressRequest } from '../../core/models/user.model';
import { HeaderComponent } from '../../shared/components/header/header.component';
import { EmberBackgroundComponent } from '../../shared/components/ember-background/ember-background.component';
import { getImageUrl } from '../../shared/utils/image.utils';
import { loadStripe, Stripe, StripeCardElement } from '@stripe/stripe-js';

type CheckoutStep = 'review' | 'address' | 'payment' | 'confirmation';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule, HeaderComponent, EmberBackgroundComponent],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss'
})
export class CheckoutComponent implements OnInit, OnDestroy, AfterViewChecked {
  private readonly router = inject(Router);
  protected readonly cartService = inject(CartService);
  private readonly profileService = inject(ProfileService);
  private readonly paymentService = inject(PaymentService);

  protected readonly currentStep = signal<CheckoutStep>('review');
  protected readonly address = signal<UserAddressResponse | null>(null);
  protected readonly isLoadingAddress = signal(true);
  protected readonly isSubmitting = signal(false);
  protected readonly orderSuccess = signal(false);
  protected readonly orderId = signal<number | null>(null);
  protected readonly error = signal<string | null>(null);

  // Stripe
  private stripe: Stripe | null = null;
  private cardElement: StripeCardElement | null = null;
  private clientSecret: string | null = null;
  private paymentIntentId: string | null = null;
  protected readonly isLoadingStripe = signal(true);
  protected readonly cardError = signal<string | null>(null);
  protected readonly cardComplete = signal(false);
  private cardMounted = false;
  private needsCardMount = false;

  // Address form for new/edit
  protected readonly isEditingAddress = signal(false);
  protected addressForm: CreateUserAddressRequest = {
    street: '',
    city: '',
    postalCode: '',
    country: 'Bosna i Hercegovina'
  };

  protected readonly getImageUrl = getImageUrl;

  protected readonly steps: { key: CheckoutStep; label: string }[] = [
    { key: 'review', label: 'Pregled' },
    { key: 'address', label: 'Adresa' },
    { key: 'payment', label: 'Plaćanje' },
    { key: 'confirmation', label: 'Potvrda' }
  ];

  ngOnInit(): void {
    if (this.cartService.isEmpty()) {
      this.router.navigate(['/cart']);
      return;
    }
    this.loadAddress();
    this.initStripe();
  }

  ngOnDestroy(): void {
    if (this.cardElement) {
      this.cardElement.destroy();
    }
  }

  ngAfterViewChecked(): void {
    if (this.needsCardMount && !this.cardMounted) {
      this.mountCardElement();
    }
  }

  private initStripe(): void {
    this.paymentService.getConfig().subscribe({
      next: (config) => {
        loadStripe(config.publishableKey).then(stripe => {
          this.stripe = stripe;
          this.isLoadingStripe.set(false);
        });
      },
      error: () => {
        this.isLoadingStripe.set(false);
        this.error.set('Greška pri inicijalizaciji sistema za plaćanje.');
      }
    });
  }

  private loadAddress(): void {
    this.isLoadingAddress.set(true);
    this.profileService.getAddress().subscribe({
      next: (addr) => {
        this.address.set(addr);
        this.isLoadingAddress.set(false);
      },
      error: () => {
        this.address.set(null);
        this.isLoadingAddress.set(false);
      }
    });
  }

  private createPaymentIntent(): void {
    this.isLoadingStripe.set(true);
    this.error.set(null);

    const items = this.cartService.items().map(item => ({
      productId: item.productId,
      quantity: item.quantity
    }));

    const couponCode = this.cartService.appliedCoupon()?.code;
    this.paymentService.createPaymentIntent(items, couponCode).subscribe({
      next: (response) => {
        this.clientSecret = response.clientSecret;
        this.paymentIntentId = response.paymentIntentId;
        this.isLoadingStripe.set(false);
        this.needsCardMount = true;
      },
      error: (err) => {
        this.isLoadingStripe.set(false);
        this.error.set(err?.error?.message || 'Greška pri kreiranju zahtjeva za plaćanje.');
      }
    });
  }

  private mountCardElement(): void {
    const container = document.getElementById('card-element');
    if (!container || !this.stripe || this.cardMounted) return;

    const elements = this.stripe.elements();
    this.cardElement = elements.create('card', {
      style: {
        base: {
          color: '#2c1810',
          fontFamily: "'Space Grotesk', sans-serif",
          fontSize: '16px',
          '::placeholder': { color: '#a08070' }
        },
        invalid: { color: '#BF616A' }
      }
    });

    this.cardElement.mount('#card-element');
    this.cardMounted = true;
    this.needsCardMount = false;

    this.cardElement.on('change', (event) => {
      this.cardComplete.set(event.complete);
      this.cardError.set(event.error?.message ?? null);
    });
  }

  protected goToStep(step: CheckoutStep): void {
    if (step === 'confirmation' && !this.orderSuccess()) return;
    this.error.set(null);
    this.currentStep.set(step);
  }

  protected nextStep(): void {
    const stepOrder: CheckoutStep[] = ['review', 'address', 'payment', 'confirmation'];
    const currentIndex = stepOrder.indexOf(this.currentStep());
    if (currentIndex < stepOrder.length - 1) {
      const nextStepKey = stepOrder[currentIndex + 1];
      this.currentStep.set(nextStepKey);

      // When entering payment step, create payment intent
      if (nextStepKey === 'payment' && !this.clientSecret) {
        this.cardMounted = false;
        this.createPaymentIntent();
      } else if (nextStepKey === 'payment' && this.clientSecret && !this.cardMounted) {
        this.needsCardMount = true;
      }
    }
  }

  protected prevStep(): void {
    const stepOrder: CheckoutStep[] = ['review', 'address', 'payment', 'confirmation'];
    const currentIndex = stepOrder.indexOf(this.currentStep());
    if (currentIndex > 0) {
      this.currentStep.set(stepOrder[currentIndex - 1]);
    }
  }

  protected startEditAddress(): void {
    const addr = this.address();
    if (addr) {
      this.addressForm = {
        street: addr.street,
        city: addr.city,
        postalCode: addr.postalCode,
        country: addr.country
      };
    } else {
      this.addressForm = { street: '', city: '', postalCode: '', country: 'Bosna i Hercegovina' };
    }
    this.isEditingAddress.set(true);
  }

  protected cancelEditAddress(): void {
    this.isEditingAddress.set(false);
  }

  protected saveAddress(): void {
    const addr = this.address();
    const req = { ...this.addressForm };

    if (addr) {
      this.profileService.updateAddress(req).subscribe({
        next: (updated) => {
          this.address.set(updated);
          this.isEditingAddress.set(false);
        }
      });
    } else {
      this.profileService.createAddress(req).subscribe({
        next: (created) => {
          this.address.set(created);
          this.isEditingAddress.set(false);
        }
      });
    }
  }

  protected async confirmPayment(): Promise<void> {
    if (!this.stripe || !this.cardElement || !this.clientSecret) return;

    if (!this.address()) {
      this.error.set('Morate dodati adresu za dostavu.');
      this.currentStep.set('address');
      return;
    }

    this.isSubmitting.set(true);
    this.error.set(null);
    this.cardError.set(null);

    // Confirm card payment with Stripe
    const { error, paymentIntent } = await this.stripe.confirmCardPayment(this.clientSecret, {
      payment_method: { card: this.cardElement }
    });

    if (error) {
      this.cardError.set(error.message ?? 'Plaćanje nije uspjelo.');
      this.isSubmitting.set(false);
      return;
    }

    if (paymentIntent?.status !== 'succeeded') {
      this.cardError.set('Plaćanje nije dovršeno. Pokušajte ponovo.');
      this.isSubmitting.set(false);
      return;
    }

    // Payment succeeded — create order on backend
    const request = {
      items: this.cartService.items().map(item => ({
        productId: item.productId,
        quantity: item.quantity
      })),
      stripePaymentIntentId: this.paymentIntentId!,
      couponCode: this.cartService.appliedCoupon()?.code
    };

    this.profileService.checkout(request).subscribe({
      next: (order) => {
        this.orderId.set(order.id);
        this.orderSuccess.set(true);
        this.cartService.clear();
        this.isSubmitting.set(false);
        this.currentStep.set('confirmation');
      },
      error: (err) => {
        this.error.set(err?.error?.message || 'Plaćanje uspješno, ali greška pri kreiranju narudžbe. Kontaktirajte podršku.');
        this.isSubmitting.set(false);
      }
    });
  }

  protected formatPrice(price: number): string {
    return price.toFixed(2);
  }

  protected getStepIndex(step: CheckoutStep): number {
    return this.steps.findIndex(s => s.key === step);
  }

  protected isStepCompleted(step: CheckoutStep): boolean {
    return this.getStepIndex(step) < this.getStepIndex(this.currentStep());
  }

  protected isStepActive(step: CheckoutStep): boolean {
    return step === this.currentStep();
  }
}
