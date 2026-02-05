import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { ProductService } from '../../../../../core/services/product.service';
import { CategoryService } from '../../../../../core/services/category.service';
import { ProductCategoryResponse } from '../../../../../core/models/category.model';
import { environment } from '../../../../../../environments/environment';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './product-form.component.html',
  styleUrl: './product-form.component.scss'
})
export class ProductFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productService = inject(ProductService);
  private readonly categoryService = inject(CategoryService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  protected readonly isEditMode = signal(false);
  protected readonly productId = signal<number | null>(null);
  protected readonly categories = signal<ProductCategoryResponse[]>([]);
  protected readonly isLoading = signal(false);
  protected readonly isSaving = signal(false);
  protected readonly currentImageUrl = signal<string | null>(null);
  protected readonly isUploading = signal(false);
  protected readonly isRemovingImage = signal(false);
  protected readonly isDragging = signal(false);
  protected readonly pendingImage = signal<File | null>(null);
  protected readonly pendingImagePreview = signal<string | null>(null);

  protected readonly form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    description: [''],
    price: [0, [Validators.required, Validators.min(0)]],
    productCategoryId: [null, [Validators.required]]
  });

  ngOnInit() {
    this.loadCategories();

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.productId.set(+id);
      this.loadProduct(+id);
    }
  }

  private loadCategories() {
    this.categoryService.getCategories({ pageSize: 100 }).subscribe({
      next: (result) => this.categories.set(result.items)
    });
  }

  private loadProduct(id: number) {
    this.isLoading.set(true);
    this.productService.getProductById(id).subscribe({
      next: (product) => {
        this.form.patchValue({
          name: product.name,
          description: product.description,
          price: product.price,
          productCategoryId: product.productCategoryId
        });
        if (product.productImageUrl) {
          this.currentImageUrl.set(`${environment.baseUrl}${product.productImageUrl}`);
        }
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.router.navigate(['/admin/products']);
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
      this.productService.updateProduct(this.productId()!, data).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.router.navigate(['/admin/products']);
        },
        error: () => this.isSaving.set(false)
      });
    } else {
      this.productService.createProduct(data, this.pendingImage() ?? undefined).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.router.navigate(['/admin/products']);
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
    // Validate file size (5MB max)
    if (file.size > 5 * 1024 * 1024) {
      alert('Slika ne smije biti veca od 5MB');
      return;
    }

    if (this.isEditMode()) {
      // Edit mode: upload immediately
      const productId = this.productId();
      if (!productId) return;

      this.isUploading.set(true);
      this.productService.uploadProductImage(productId, file).subscribe({
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
      const productId = this.productId();
      if (!productId) return;

      this.isRemovingImage.set(true);
      this.productService.deleteProductImage(productId).subscribe({
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
