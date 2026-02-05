import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ProductService } from '../../../../../core/services/product.service';
import { ProductResponse } from '../../../../../core/models/product.model';
import { environment } from '../../../../../../environments/environment';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss'
})
export class ProductListComponent implements OnInit {
  private readonly productService = inject(ProductService);

  protected readonly products = signal<ProductResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly totalCount = signal(0);
  protected readonly searchQuery = signal('');

  protected readonly filteredProducts = computed(() => {
    const query = this.searchQuery().toLowerCase().trim();
    const allProducts = this.products();

    if (!query) {
      return allProducts;
    }

    return allProducts.filter(product =>
      product.name.toLowerCase().includes(query) ||
      product.productCategoryName?.toLowerCase().includes(query) ||
      product.description?.toLowerCase().includes(query)
    );
  });

  ngOnInit() {
    this.loadProducts();
  }

  private loadProducts() {
    this.isLoading.set(true);
    this.productService.getProducts({ pageSize: 50 }).subscribe({
      next: (result) => {
        this.products.set(result.items);
        this.totalCount.set(result.totalCount);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  protected onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.searchQuery.set(value);
  }

  protected getImageUrl(path: string | undefined): string {
    if (!path) return '';
    return `${environment.baseUrl}${path}`;
  }

  protected deleteProduct(product: ProductResponse) {
    if (confirm(`Da li ste sigurni da želite obrisati "${product.name}"?`)) {
      this.productService.deleteProduct(product.id).subscribe({
        next: () => {
          this.loadProducts();
        }
      });
    }
  }
}
