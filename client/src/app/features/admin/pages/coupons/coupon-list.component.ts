import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { CouponService } from '../../../../core/services/coupon.service';
import { CouponResponse } from '../../../../core/models/coupon.model';
import { ConfirmDialogService } from '../../../../shared/services/confirm-dialog.service';
import { NotificationService } from '../../../../core/services/notification.service';

@Component({
  selector: 'app-coupon-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './coupon-list.component.html',
  styleUrl: './coupon-list.component.scss'
})
export class CouponListComponent implements OnInit, OnDestroy {
  private readonly couponService = inject(CouponService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly notification = inject(NotificationService);
  private readonly destroy$ = new Subject<void>();
  private readonly searchSubject = new Subject<string>();

  protected readonly coupons = signal<CouponResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly totalCount = signal(0);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly searchQuery = signal('');
  protected readonly activeFilter = signal<boolean | null>(null);

  protected readonly totalPages = computed(() =>
    Math.ceil(this.totalCount() / this.pageSize())
  );

  ngOnInit() {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(query => {
      this.searchQuery.set(query);
      this.currentPage.set(1);
      this.loadCoupons();
    });

    this.loadCoupons();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCoupons() {
    this.isLoading.set(true);
    this.couponService.getCoupons({
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      search: this.searchQuery() || undefined,
      isActive: this.activeFilter() ?? undefined
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: (result) => {
        this.coupons.set(result.items);
        this.totalCount.set(result.totalCount);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  protected onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchSubject.next(value);
  }

  protected onFilterActive(isActive: boolean | null): void {
    this.activeFilter.set(isActive);
    this.currentPage.set(1);
    this.loadCoupons();
  }

  protected goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadCoupons();
  }

  protected async deleteCoupon(id: number, code: string): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Brisanje kupona',
      message: `Da li ste sigurni da želite obrisati kupon "${code}"?`,
      confirmText: 'Obriši',
      cancelText: 'Otkaži'
    });

    if (!confirmed) return;

    this.couponService.deleteCoupon(id).subscribe({
      next: () => {
        this.notification.success('Kupon je uspješno obrisan.');
        this.loadCoupons();
      },
      error: () => {
        this.notification.error('Greška prilikom brisanja kupona.');
      }
    });
  }

  protected formatDate(dateString?: string): string {
    if (!dateString) return 'Bez isteka';
    const date = new Date(dateString);
    return date.toLocaleDateString('bs-BA', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  protected formatDiscount(coupon: CouponResponse): string {
    if (coupon.discountType === 'Percentage') {
      return `${coupon.discountValue}%`;
    }
    return `${coupon.discountValue.toFixed(2)} KM`;
  }

  protected getStatusBadgeClass(coupon: CouponResponse): string {
    if (!coupon.isActive) return 'badge-inactive';
    if (coupon.isExpired) return 'badge-expired';
    if (coupon.isMaxedOut) return 'badge-maxed';
    return 'badge-active';
  }

  protected getStatusText(coupon: CouponResponse): string {
    if (!coupon.isActive) return 'Neaktivan';
    if (coupon.isExpired) return 'Istekao';
    if (coupon.isMaxedOut) return 'Iskorišten';
    return 'Aktivan';
  }
}
