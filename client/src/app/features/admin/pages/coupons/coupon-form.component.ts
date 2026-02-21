import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { CouponService } from '../../../../core/services/coupon.service';
import { CreateCouponRequest, UpdateCouponRequest } from '../../../../core/models/coupon.model';
import { NotificationService } from '../../../../core/services/notification.service';

@Component({
  selector: 'app-coupon-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './coupon-form.component.html',
  styleUrl: './coupon-form.component.scss'
})
export class CouponFormComponent implements OnInit {
  private readonly couponService = inject(CouponService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly notification = inject(NotificationService);

  protected readonly isLoading = signal(true);
  protected readonly isSaving = signal(false);
  protected readonly isEditMode = signal(false);
  protected readonly couponId = signal<number | null>(null);

  // Form fields
  protected code = '';
  protected discountType = 'Percentage';
  protected discountValue = 0;
  protected minimumOrderAmount: number | null = null;
  protected expiryDate = '';
  protected maxUses: number | null = null;
  protected isActive = true;
  protected isFeatured = false;

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.couponId.set(+id);
      this.loadCoupon(+id);
    } else {
      this.isLoading.set(false);
    }
  }

  private loadCoupon(id: number) {
    this.couponService.getCouponById(id).subscribe({
      next: (coupon) => {
        this.code = coupon.code;
        this.discountType = coupon.discountType;
        this.discountValue = coupon.discountValue;
        this.minimumOrderAmount = coupon.minimumOrderAmount ?? null;
        this.expiryDate = coupon.expiryDate ? coupon.expiryDate.split('T')[0] : '';
        this.maxUses = coupon.maxUses ?? null;
        this.isActive = coupon.isActive;
        this.isFeatured = coupon.isFeatured;
        this.isLoading.set(false);
      },
      error: () => {
        this.notification.error({ title: 'Greška', message: 'Greška prilikom učitavanja kupona.' });
        this.router.navigate(['/admin/coupons']);
      }
    });
  }

  protected onSubmit() {
    if (!this.validateForm()) return;

    this.isSaving.set(true);

    const request: CreateCouponRequest | UpdateCouponRequest = {
      code: this.code.trim(),
      discountType: this.discountType,
      discountValue: this.discountValue,
      minimumOrderAmount: this.minimumOrderAmount ?? undefined,
      expiryDate: this.expiryDate || undefined,
      maxUses: this.maxUses ?? undefined,
      isActive: this.isActive,
      isFeatured: this.isFeatured
    };

    const operation = this.isEditMode()
      ? this.couponService.updateCoupon(this.couponId()!, request as UpdateCouponRequest)
      : this.couponService.createCoupon(request as CreateCouponRequest);

    operation.subscribe({
      next: () => {
        this.notification.success({
          title: 'Uspješno spremljeno',
          message: this.isEditMode() ? 'Kupon je uspješno ažuriran.' : 'Kupon je uspješno kreiran.'
        });
        this.router.navigate(['/admin/coupons']);
      },
      error: (error) => {
        this.isSaving.set(false);
        const errorMessage = error?.error?.error || 'Greška prilikom spremanja kupona.';
        this.notification.error({ title: 'Greška', message: errorMessage });
      }
    });
  }

  private validateForm(): boolean {
    if (!this.code.trim()) {
      this.notification.error({ title: 'Greška', message: 'Kod kupona je obavezan.' });
      return false;
    }

    if (this.discountValue <= 0) {
      this.notification.error({ title: 'Greška', message: 'Vrijednost popusta mora biti veća od 0.' });
      return false;
    }

    if (this.discountType === 'Percentage' && this.discountValue > 100) {
      this.notification.error({ title: 'Greška', message: 'Procenat popusta ne može biti veći od 100.' });
      return false;
    }

    if (this.minimumOrderAmount !== null && this.minimumOrderAmount < 0) {
      this.notification.error({ title: 'Greška', message: 'Minimalni iznos ne može biti negativan.' });
      return false;
    }

    if (this.maxUses !== null && this.maxUses < 1) {
      this.notification.error({ title: 'Greška', message: 'Maksimalan broj korištenja mora biti barem 1.' });
      return false;
    }

    return true;
  }

  protected onCancel() {
    this.router.navigate(['/admin/coupons']);
  }
}
