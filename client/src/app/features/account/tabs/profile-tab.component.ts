import { Component, inject, signal, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProfileService } from '../../../core/services/profile.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { UserResponse } from '../../../core/models/user.model';
import { getImageUrl, getInitials } from '../../../shared/utils/image.utils';

@Component({
  selector: 'app-profile-tab',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './profile-tab.component.html',
  styleUrl: './profile-tab.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileTabComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly profileService = inject(ProfileService);
  private readonly authService = inject(AuthService);
  private readonly notificationService = inject(NotificationService);

  protected readonly isLoading = signal(true);
  protected readonly isSaving = signal(false);
  protected readonly isUploading = signal(false);
  protected readonly isDragging = signal(false);
  protected readonly imageUrl = signal<string | null>(null);
  protected readonly user = signal<UserResponse | null>(null);

  private readonly phonePattern = /^(\+387|0)\s?6[0-9]\s?[0-9]{3}\s?[0-9]{3,4}$/;

  protected readonly form: FormGroup = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    username: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.required, Validators.pattern(this.phonePattern)]]
  });

  ngOnInit() {
    this.loadProfile();
  }

  private loadProfile() {
    this.isLoading.set(true);
    this.profileService.getProfile().subscribe({
      next: (user) => {
        this.user.set(user);
        this.form.patchValue({
          firstName: user.firstName,
          lastName: user.lastName,
          username: user.username,
          email: user.email,
          phoneNumber: user.phoneNumber
        });
        if (user.profileImageUrl) {
          this.imageUrl.set(getImageUrl(user.profileImageUrl));
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.notificationService.error({
          title: 'Greška',
          message: 'Nije moguće učitati profil.'
        });
      }
    });
  }

  protected getInitials(): string {
    const u = this.user();
    return u ? getInitials(u.firstName, u.lastName) : '';
  }

  protected onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    this.profileService.updateProfile(this.form.value).subscribe({
      next: (updated) => {
        this.user.set(updated);
        this.isSaving.set(false);
        this.authService.refreshCurrentUser({
          firstName: updated.firstName,
          lastName: updated.lastName,
          email: updated.email
        });
        this.notificationService.success({
          title: 'Uspjeh',
          message: 'Profil je ažuriran.'
        });
      },
      error: (err) => {
        this.isSaving.set(false);
        this.notificationService.error({
          title: 'Greška',
          message: err.error?.error || 'Greška pri ažuriranju profila.'
        });
      }
    });
  }

  protected hasError(field: string, error: string): boolean {
    const control = this.form.get(field);
    return control ? control.hasError(error) && control.touched : false;
  }

  // Image upload
  protected onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.uploadImage(input.files[0]);
    }
  }

  protected onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging.set(true);
  }

  protected onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging.set(false);
  }

  protected onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging.set(false);

    if (event.dataTransfer?.files && event.dataTransfer.files[0]) {
      const file = event.dataTransfer.files[0];
      if (file.type.startsWith('image/')) {
        this.uploadImage(file);
      }
    }
  }

  private uploadImage(file: File): void {
    if (file.size > 5 * 1024 * 1024) {
      this.notificationService.warning({
        title: 'Prevelika slika',
        message: 'Slika ne smije biti veća od 5MB.'
      });
      return;
    }

    this.isUploading.set(true);
    this.profileService.uploadImage(file).subscribe({
      next: (updated) => {
        if (updated.profileImageUrl) {
          this.imageUrl.set(getImageUrl(updated.profileImageUrl));
        }
        this.user.set(updated);
        this.isUploading.set(false);
        this.notificationService.success({
          title: 'Uspjeh',
          message: 'Slika je postavljena.'
        });
      },
      error: () => {
        this.isUploading.set(false);
        this.notificationService.error({
          title: 'Greška',
          message: 'Greška pri postavljanju slike.'
        });
      }
    });
  }

  protected removeImage(): void {
    this.isUploading.set(true);
    this.profileService.deleteImage().subscribe({
      next: () => {
        this.imageUrl.set(null);
        const u = this.user();
        if (u) {
          this.user.set({ ...u, profileImageUrl: null });
        }
        this.isUploading.set(false);
      },
      error: () => {
        this.isUploading.set(false);
        this.notificationService.error({
          title: 'Greška',
          message: 'Greška pri uklanjanju slike.'
        });
      }
    });
  }
}
