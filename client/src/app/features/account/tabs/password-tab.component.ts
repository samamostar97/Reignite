import { Component, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProfileService } from '../../../core/services/profile.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-password-tab',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './password-tab.component.html',
  styleUrl: './password-tab.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PasswordTabComponent {
  private readonly fb = inject(FormBuilder);
  private readonly profileService = inject(ProfileService);
  private readonly notificationService = inject(NotificationService);

  protected readonly isSaving = signal(false);
  protected readonly errorMessage = signal<string | null>(null);

  protected readonly form: FormGroup = this.fb.group({
    currentPassword: ['', [Validators.required]],
    newPassword: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(100)]],
    confirmPassword: ['', [Validators.required]]
  });

  protected onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { newPassword, confirmPassword } = this.form.value;
    if (newPassword !== confirmPassword) {
      this.errorMessage.set('Lozinke se ne poklapaju.');
      return;
    }

    this.isSaving.set(true);
    this.errorMessage.set(null);

    this.profileService.changePassword(this.form.value).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.form.reset();
        this.notificationService.success({
          title: 'Uspjeh',
          message: 'Lozinka je uspješno promijenjena.'
        });
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err.error || 'Greška pri promjeni lozinke.');
      }
    });
  }

  protected hasError(field: string, error: string): boolean {
    const control = this.form.get(field);
    return control ? control.hasError(error) && control.touched : false;
  }
}
