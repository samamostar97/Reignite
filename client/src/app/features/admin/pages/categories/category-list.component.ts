import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CategoryService } from '../../../../core/services/category.service';
import { ProductCategoryResponse } from '../../../../core/models/category.model';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.scss'
})
export class CategoryListComponent implements OnInit {
  private readonly categoryService = inject(CategoryService);

  protected readonly categories = signal<ProductCategoryResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly isAdding = signal(false);
  protected readonly editingId = signal<number | null>(null);

  protected newCategoryName = '';
  protected editCategoryName = '';

  ngOnInit() {
    this.loadCategories();
  }

  private loadCategories() {
    this.isLoading.set(true);
    this.categoryService.getCategories({ pageSize: 100 }).subscribe({
      next: (result) => {
        this.categories.set(result.items);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  protected showAddForm() {
    this.isAdding.set(true);
    this.newCategoryName = '';
  }

  protected cancelAdd() {
    this.isAdding.set(false);
    this.newCategoryName = '';
  }

  protected addCategory() {
    if (!this.newCategoryName.trim()) return;

    this.categoryService.createCategory({ name: this.newCategoryName.trim() }).subscribe({
      next: () => {
        this.cancelAdd();
        this.loadCategories();
      }
    });
  }

  protected startEdit(category: ProductCategoryResponse) {
    this.editingId.set(category.id);
    this.editCategoryName = category.name;
  }

  protected cancelEdit() {
    this.editingId.set(null);
    this.editCategoryName = '';
  }

  protected saveEdit(id: number) {
    if (!this.editCategoryName.trim()) return;

    this.categoryService.updateCategory(id, { name: this.editCategoryName.trim() }).subscribe({
      next: () => {
        this.cancelEdit();
        this.loadCategories();
      }
    });
  }

  protected deleteCategory(category: ProductCategoryResponse) {
    if (confirm(`Da li ste sigurni da želite obrisati kategoriju "${category.name}"?`)) {
      this.categoryService.deleteCategory(category.id).subscribe({
        next: () => this.loadCategories()
      });
    }
  }
}
