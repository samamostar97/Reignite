import { Component, inject, signal, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { UserService } from '../../../../../core/services/user.service';
import { environment } from '../../../../../../environments/environment';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './user-form.component.html',
  styleUrl: './user-form.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  protected readonly isEditMode = signal(false);
  protected readonly userId = signal<number | null>(null);
  protected readonly isLoading = signal(false);
  protected readonly isSaving = signal(false);
  protected readonly currentImageUrl = signal<string | null>(null);
  protected readonly isUploading = signal(false);
  protected readonly isRemovingImage = signal(false);
  protected readonly isDragging = signal(false);
  protected readonly pendingImage = signal<File | null>(null);
  protected readonly pendingImagePreview = signal<string | null>(null);

  protected readonly form: FormGroup = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName: ['', [Validators.required, Validators.minLength(2)]],
    username: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.required]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.userId.set(+id);
      // Remove password validation for edit mode
      this.form.get('password')?.clearValidators();
      this.form.get('password')?.updateValueAndValidity();
      this.loadUser(+id);
    }
  }

  private loadUser(id: number) {
    this.isLoading.set(true);
    this.userService.getUserById(id).subscribe({
      next: (user) => {
        this.form.patchValue({
          firstName: user.firstName,
          lastName: user.lastName,
          username: user.username,
          email: user.email,
          phoneNumber: user.phoneNumber
        });
        if (user.profileImageUrl) {
          // If it's already an absolute URL (Unsplash, etc.), use as-is
          if (user.profileImageUrl.startsWith('http://') || user.profileImageUrl.startsWith('https://')) {
            this.currentImageUrl.set(user.profileImageUrl);
          } else {
            this.currentImageUrl.set(`${environment.baseUrl}${user.profileImageUrl}`);
          }
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.router.navigate(['/admin/users']);
      }
    });
  }

  protected onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    const data = this.form.value;

    if (this.isEditMode()) {
      // Don't send password if empty in edit mode
      const updateData = { ...data };
      delete updateData.password;

      this.userService.updateUser(this.userId()!, updateData).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.router.navigate(['/admin/users']);
        },
        error: () => this.isSaving.set(false)
      });
    } else {
      this.userService.createUser(data, this.pendingImage() ?? undefined).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.router.navigate(['/admin/users']);
        },
        error: () => this.isSaving.set(false)
      });
    }
  }

  protected hasError(field: string, error: string): boolean {
    const control = this.form.get(field);
    return control ? control.hasError(error) && control.touched : false;
  }

  // Image upload methods
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
      alert('Slika ne smije biti veca od 5MB');
      return;
    }

    if (this.isEditMode()) {
      // Edit mode: upload immediately
      const userId = this.userId();
      if (!userId) return;

      this.isUploading.set(true);
      this.userService.uploadUserImage(userId, file).subscribe({
        next: (result) => {
          if (result.fileUrl) {
            this.currentImageUrl.set(`${environment.baseUrl}${result.fileUrl}`);
          }
          this.isUploading.set(false);
        },
        error: () => {
          this.isUploading.set(false);
          alert('Greska pri ucitavanju slike');
        }
      });
    } else {
      // Create mode: store file locally and show preview
      this.pendingImage.set(file);
      const reader = new FileReader();
      reader.onload = (e) => {
        this.pendingImagePreview.set(e.target?.result as string);
      };
      reader.readAsDataURL(file);
    }
  }

  protected removeImage(): void {
    if (this.isEditMode()) {
      // Edit mode: delete from server
      const userId = this.userId();
      if (!userId) return;

      this.isRemovingImage.set(true);
      this.userService.deleteUserImage(userId).subscribe({
        next: () => {
          this.currentImageUrl.set(null);
          this.isRemovingImage.set(false);
        },
        error: () => {
          this.isRemovingImage.set(false);
          alert('Greska pri uklanjanju slike');
        }
      });
    } else {
      // Create mode: just clear the pending image
      this.pendingImage.set(null);
      this.pendingImagePreview.set(null);
    }
  }
}
