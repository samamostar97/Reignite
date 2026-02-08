import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { EmberBackgroundComponent } from '../../../shared/components/ember-background/ember-background.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, EmberBackgroundComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly notificationService = inject(NotificationService);

  protected readonly isLoading = signal(false);
  protected readonly errorMessage = signal<string | null>(null);
  protected readonly showPassword = signal(false);

  protected readonly loginForm: FormGroup = this.fb.group({
    email: ['', [Validators.required]],
    password: ['', [Validators.required]]
  });

  protected togglePassword(): void {
    this.showPassword.set(!this.showPassword());
  }

  protected onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.authService.login(this.loginForm.value).subscribe({
      next: () => {
        this.notificationService.success({
          title: 'Uspješna prijava',
          message: 'Dobrodošli nazad!'
        });

        // Redirect based on user role
        const redirectUrl = this.authService.isAdmin() ? '/admin' : '/';
        this.router.navigate([redirectUrl]);
      },
      error: (err) => {
        this.isLoading.set(false);
        const errorMsg = err.error?.error || 'Greška pri prijavi.';
        this.errorMessage.set(errorMsg);
        this.notificationService.error({
          title: 'Greška pri prijavi',
          message: errorMsg
        });
      }
    });
  }

  protected hasError(field: string, error: string): boolean {
    const control = this.loginForm.get(field);
    return control ? control.hasError(error) && control.touched : false;
  }
}
