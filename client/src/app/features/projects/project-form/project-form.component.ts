import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil, switchMap } from 'rxjs/operators';
import { ProjectService } from '../../../core/services/project.service';
import { HobbyService } from '../../../core/services/hobby.service';
import { ProductService } from '../../../core/services/product.service';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { HobbyResponse } from '../../../core/models/hobby.model';
import { ProductResponse } from '../../../core/models/product.model';
import { ProjectResponse } from '../../../core/models/project.model';
import { HeaderComponent } from '../../../shared/components/header/header.component';
import { EmberBackgroundComponent } from '../../../shared/components/ember-background/ember-background.component';
import { getImageUrl } from '../../../shared/utils/image.utils';

@Component({
  selector: 'app-project-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    HeaderComponent,
    EmberBackgroundComponent
  ],
  templateUrl: './project-form.component.html',
  styleUrl: './project-form.component.scss'
})
export class ProjectFormComponent implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly projectService = inject(ProjectService);
  private readonly hobbyService = inject(HobbyService);
  private readonly productService = inject(ProductService);
  private readonly authService = inject(AuthService);
  private readonly notificationService = inject(NotificationService);
  private readonly destroy$ = new Subject<void>();

  // State signals
  protected readonly isLoading = signal(false);
  protected readonly isSaving = signal(false);
  protected readonly isEditMode = signal(false);
  protected readonly projectId = signal<number | null>(null);
  protected readonly hobbies = signal<HobbyResponse[]>([]);
  protected readonly products = signal<ProductResponse[]>([]);
  protected readonly isUploading = signal(false);
  protected readonly isDragging = signal(false);
  protected readonly currentImageUrl = signal<string | null>(null);
  protected readonly pendingFile = signal<File | null>(null);
  protected readonly pendingImagePreview = signal<string | null>(null);
  protected readonly isRemovingImage = signal(false);
  protected readonly imageWasRemoved = signal(false);

  // Reactive form
  protected readonly form: FormGroup = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(200)]],
    description: ['', [Validators.maxLength(2000)]],
    hobbyId: [null as number | null, [Validators.required]],
    productId: [null as number | null],
    hoursSpent: [null as number | null, [Validators.min(0), Validators.max(10000)]]
  });

  ngOnInit(): void {
    this.loadHobbies();
    this.loadProducts();

    const id = this.route.snapshot.paramMap.get('id');
    if (id && /^\d+$/.test(id)) {
      this.isEditMode.set(true);
      this.projectId.set(+id);
      this.loadProject(+id);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadHobbies(): void {
    this.hobbyService.getHobbies()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (hobbies) => this.hobbies.set(hobbies),
        error: () => this.notificationService.error({
          title: 'Greska',
          message: 'Nije moguce ucitati hobije.'
        })
      });
  }

  private loadProducts(): void {
    this.productService.getProducts({ pageNumber: 1, pageSize: 100 })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => this.products.set(result.items),
        error: () => {
          // Products are optional, so we silently handle the error
        }
      });
  }

  private loadProject(id: number): void {
    this.isLoading.set(true);

    this.projectService.getProjectById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (project) => {
          // Verify ownership
          const currentUser = this.authService.currentUser();
          if (!currentUser || currentUser.id !== project.userId) {
            this.notificationService.error({
              title: 'Neovlasten pristup',
              message: 'Nemate dozvolu za uredivanje ovog projekta.'
            });
            this.router.navigate(['/projects']);
            return;
          }

          this.populateForm(project);
          this.isLoading.set(false);
        },
        error: () => {
          this.notificationService.error({
            title: 'Greska',
            message: 'Projekat nije pronadjen.'
          });
          this.isLoading.set(false);
          this.router.navigate(['/projects']);
        }
      });
  }

  private populateForm(project: ProjectResponse): void {
    this.form.patchValue({
      title: project.title,
      description: project.description || '',
      hobbyId: project.hobbyId,
      productId: project.productId || null,
      hoursSpent: project.hoursSpent || null
    });

    if (project.imageUrl) {
      this.currentImageUrl.set(getImageUrl(project.imageUrl));
    }
  }

  protected onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSaving.set(true);
    const formValue = this.form.value;
    const currentUser = this.authService.currentUser();

    if (!currentUser) {
      this.notificationService.error({
        title: 'Greska',
        message: 'Morate biti prijavljeni.'
      });
      this.isSaving.set(false);
      return;
    }

    if (this.isEditMode()) {
      this.updateProject(formValue);
    } else {
      this.createProject(formValue, currentUser.id);
    }
  }

  private createProject(formValue: any, userId: number): void {
    const request = {
      title: formValue.title,
      description: formValue.description || undefined,
      hoursSpent: formValue.hoursSpent || undefined,
      userId: userId,
      hobbyId: formValue.hobbyId,
      productId: formValue.productId || undefined
    };

    this.projectService.createProject(request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (created) => {
          // If there is a pending image, upload it
          const pendingFile = this.pendingFile();
          if (pendingFile) {
            this.isUploading.set(true);
            this.projectService.uploadProjectImage(created.id, pendingFile)
              .pipe(takeUntil(this.destroy$))
              .subscribe({
                next: () => {
                  this.isUploading.set(false);
                  this.isSaving.set(false);
                  this.notificationService.success({
                    title: 'Uspjesno',
                    message: 'Projekat je kreiran.'
                  });
                  this.router.navigate(['/projects', created.id]);
                },
                error: () => {
                  this.isUploading.set(false);
                  this.isSaving.set(false);
                  this.notificationService.warning({
                    title: 'Djelimicno uspjesno',
                    message: 'Projekat je kreiran, ali slika nije ucitana.'
                  });
                  this.router.navigate(['/projects', created.id]);
                }
              });
          } else {
            this.isSaving.set(false);
            this.notificationService.success({
              title: 'Uspjesno',
              message: 'Projekat je kreiran.'
            });
            this.router.navigate(['/projects', created.id]);
          }
        },
        error: (err) => {
          this.isSaving.set(false);
          this.notificationService.error({
            title: 'Greska',
            message: err.error?.error || 'Nije moguce kreirati projekat.'
          });
        }
      });
  }

  private updateProject(formValue: any): void {
    const projectId = this.projectId()!;
    const request = {
      title: formValue.title,
      description: formValue.description || undefined,
      hoursSpent: formValue.hoursSpent ?? undefined,
      hobbyId: formValue.hobbyId,
      productId: formValue.productId || undefined
    };

    this.projectService.updateProject(projectId, request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          // Handle pending image upload in edit mode
          const pendingFile = this.pendingFile();
          if (pendingFile) {
            this.isUploading.set(true);
            this.projectService.uploadProjectImage(projectId, pendingFile)
              .pipe(takeUntil(this.destroy$))
              .subscribe({
                next: () => {
                  this.isUploading.set(false);
                  this.isSaving.set(false);
                  this.notificationService.success({
                    title: 'Uspjesno',
                    message: 'Projekat je azuriran.'
                  });
                  this.router.navigate(['/projects', projectId]);
                },
                error: () => {
                  this.isUploading.set(false);
                  this.isSaving.set(false);
                  this.notificationService.warning({
                    title: 'Djelimicno uspjesno',
                    message: 'Projekat je azuriran, ali slika nije ucitana.'
                  });
                  this.router.navigate(['/projects', projectId]);
                }
              });
          } else if (this.imageWasRemoved()) {
            // Image was removed during editing, delete from server
            this.projectService.deleteProjectImage(projectId)
              .pipe(takeUntil(this.destroy$))
              .subscribe({
                next: () => {
                  this.isSaving.set(false);
                  this.notificationService.success({
                    title: 'Uspjesno',
                    message: 'Projekat je azuriran.'
                  });
                  this.router.navigate(['/projects', projectId]);
                },
                error: () => {
                  this.isSaving.set(false);
                  this.notificationService.warning({
                    title: 'Djelimicno uspjesno',
                    message: 'Projekat je azuriran, ali slika nije uklonjena.'
                  });
                  this.router.navigate(['/projects', projectId]);
                }
              });
          } else {
            this.isSaving.set(false);
            this.notificationService.success({
              title: 'Uspjesno',
              message: 'Projekat je azuriran.'
            });
            this.router.navigate(['/projects', projectId]);
          }
        },
        error: (err) => {
          this.isSaving.set(false);
          this.notificationService.error({
            title: 'Greska',
            message: err.error?.error || 'Nije moguce azurirati projekat.'
          });
        }
      });
  }

  // Image handling methods
  protected onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      this.handleFile(input.files[0]);
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
        this.handleFile(file);
      } else {
        this.notificationService.warning({
          title: 'Neispravan format',
          message: 'Dozvoljeni su samo formati slika (PNG, JPG, JPEG).'
        });
      }
    }
  }

  private handleFile(file: File): void {
    // Validate file size (5MB max)
    if (file.size > 5 * 1024 * 1024) {
      this.notificationService.warning({
        title: 'Prevelika slika',
        message: 'Slika ne smije biti veca od 5MB.'
      });
      return;
    }

    // Validate file type
    const allowedTypes = ['image/png', 'image/jpeg', 'image/jpg', 'image/webp'];
    if (!allowedTypes.includes(file.type)) {
      this.notificationService.warning({
        title: 'Neispravan format',
        message: 'Dozvoljeni su samo formati slika (PNG, JPG, JPEG, WebP).'
      });
      return;
    }

    // Store file as pending and show preview
    this.pendingFile.set(file);
    this.imageWasRemoved.set(false);

    const reader = new FileReader();
    reader.onload = (e) => {
      this.pendingImagePreview.set(e.target?.result as string);
    };
    reader.readAsDataURL(file);
  }

  protected removeImage(): void {
    if (this.pendingFile() || this.pendingImagePreview()) {
      // Clear pending file
      this.pendingFile.set(null);
      this.pendingImagePreview.set(null);
    }

    if (this.isEditMode() && this.currentImageUrl()) {
      // Mark for removal on save
      this.currentImageUrl.set(null);
      this.imageWasRemoved.set(true);
    }
  }

  protected hasError(field: string, error: string): boolean {
    const control = this.form.get(field);
    return control ? control.hasError(error) && control.touched : false;
  }

  protected get hasImage(): boolean {
    return !!(this.currentImageUrl() || this.pendingImagePreview());
  }
}
