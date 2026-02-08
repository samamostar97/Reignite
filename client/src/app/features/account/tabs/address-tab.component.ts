import { Component, inject, signal, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProfileService } from '../../../core/services/profile.service';
import { NotificationService } from '../../../core/services/notification.service';
import { UserAddressResponse } from '../../../core/models/user.model';

@Component({
  selector: 'app-address-tab',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './address-tab.component.html',
  styleUrl: './address-tab.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AddressTabComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly profileService = inject(ProfileService);
  private readonly notificationService = inject(NotificationService);

  protected readonly isLoading = signal(true);
  protected readonly isSaving = signal(false);
  protected readonly isEditing = signal(false);
  protected readonly address = signal<UserAddressResponse | null>(null);

  protected readonly form: FormGroup = this.fb.group({
    street: ['', [Validators.required, Validators.maxLength(200)]],
    city: ['', [Validators.required, Validators.maxLength(100)]],
    postalCode: ['', [Validators.required, Validators.maxLength(20)]],
    country: ['', [Validators.required, Validators.maxLength(100)]]
  });

  ngOnInit() {
    this.loadAddress();
  }

  private loadAddress() {
    this.isLoading.set(true);
    this.profileService.getAddress().subscribe({
      next: (address) => {
        this.address.set(address);
        this.form.patchValue({
          street: address.street,
          city: address.city,
          postalCode: address.postalCode,
          country: address.country
        });
        this.isLoading.set(false);
      },
      error: () => {
        this.address.set(null);
        this.isLoading.set(false);
      }
    });
  }

  protected startEditing() {
    const addr = this.address();
    if (addr) {
      this.form.patchValue({
        street: addr.street,
        city: addr.city,
        postalCode: addr.postalCode,
        country: addr.country
      });
    } else {
      this.form.reset();
    }
    this.isEditing.set(true);
  }

  protected cancelEditing() {
    this.isEditing.set(false);
  }

  protected onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    const data = this.form.value;
    const operation = this.address()
      ? this.profileService.updateAddress(data)
      : this.profileService.createAddress(data);

    operation.subscribe({
      next: (address) => {
        this.address.set(address);
        this.isEditing.set(false);
        this.isSaving.set(false);
        this.notificationService.success({
          title: 'Uspjeh',
          message: this.address() ? 'Adresa je ažurirana.' : 'Adresa je kreirana.'
        });
      },
      error: (err) => {
        this.isSaving.set(false);
        this.notificationService.error({
          title: 'Greška',
          message: err.error?.error || 'Greška pri spremanju adrese.'
        });
      }
    });
  }

  protected deleteAddress() {
    if (!confirm('Da li ste sigurni da želite obrisati adresu?')) return;

    this.profileService.deleteAddress().subscribe({
      next: () => {
        this.address.set(null);
        this.form.reset();
        this.isEditing.set(false);
        this.notificationService.success({
          title: 'Uspjeh',
          message: 'Adresa je obrisana.'
        });
      },
      error: (err) => {
        this.notificationService.error({
          title: 'Greška',
          message: err.error?.error || 'Greška pri brisanju adrese.'
        });
      }
    });
  }

  protected hasError(field: string, error: string): boolean {
    const control = this.form.get(field);
    return control ? control.hasError(error) && control.touched : false;
  }
}
