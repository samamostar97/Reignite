import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { SupplierService } from '../../../../core/services/supplier.service';
import { SupplierResponse } from '../../../../core/models/supplier.model';
import { ConfirmDialogService } from '../../../../shared/services/confirm-dialog.service';

@Component({
  selector: 'app-supplier-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './supplier-list.component.html',
  styleUrl: './supplier-list.component.scss'
})
export class SupplierListComponent implements OnInit, OnDestroy {
  private readonly supplierService = inject(SupplierService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly destroy$ = new Subject<void>();
  private readonly searchSubject = new Subject<string>();

  protected readonly suppliers = signal<SupplierResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly isAdding = signal(false);
  protected readonly editingId = signal<number | null>(null);
  protected readonly totalCount = signal(0);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly searchQuery = signal('');
  protected readonly errorMessage = signal<string | null>(null);

  protected readonly totalPages = computed(() =>
    Math.ceil(this.totalCount() / this.pageSize())
  );

  protected newSupplierName = '';
  protected newSupplierWebsite = '';
  protected editSupplierName = '';
  protected editSupplierWebsite = '';

  ngOnInit() {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(query => {
      this.searchQuery.set(query);
      this.currentPage.set(1);
      this.loadSuppliers();
    });

    this.loadSuppliers();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadSuppliers() {
    this.isLoading.set(true);
    this.supplierService.getSuppliers({
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      search: this.searchQuery() || undefined
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: (result) => {
        this.suppliers.set(result.items);
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

  protected goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadSuppliers();
  }

  protected showAddForm() {
    this.isAdding.set(true);
    this.newSupplierName = '';
    this.newSupplierWebsite = '';
    this.errorMessage.set(null);
  }

  protected cancelAdd() {
    this.isAdding.set(false);
    this.newSupplierName = '';
    this.newSupplierWebsite = '';
    this.errorMessage.set(null);
  }

  private readonly urlPattern = /^(https?:\/\/)?([\w-]+\.)+[\w-]+(\/[\w-./?%&=]*)?$/;

  protected addSupplier() {
    const name = this.newSupplierName.trim();
    const website = this.newSupplierWebsite.trim();

    if (!name) {
      this.errorMessage.set('Naziv dobavljača je obavezan.');
      return;
    }
    if (name.length < 2 || name.length > 100) {
      this.errorMessage.set('Naziv mora imati između 2 i 100 znakova.');
      return;
    }
    if (website && !this.urlPattern.test(website)) {
      this.errorMessage.set('Website mora biti validan URL.');
      return;
    }
    if (website && website.length > 200) {
      this.errorMessage.set('Website može imati najviše 200 znakova.');
      return;
    }

    this.errorMessage.set(null);
    this.supplierService.createSupplier({
      name,
      website: website || undefined
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.cancelAdd();
        this.loadSuppliers();
      },
      error: (err) => {
        this.errorMessage.set(err.error?.error || 'Greška pri dodavanju dobavljača.');
      }
    });
  }

  protected startEdit(supplier: SupplierResponse) {
    this.editingId.set(supplier.id);
    this.editSupplierName = supplier.name;
    this.editSupplierWebsite = supplier.website || '';
    this.errorMessage.set(null);
  }

  protected cancelEdit() {
    this.editingId.set(null);
    this.editSupplierName = '';
    this.editSupplierWebsite = '';
    this.errorMessage.set(null);
  }

  protected saveEdit(id: number) {
    const name = this.editSupplierName.trim();
    const website = this.editSupplierWebsite.trim();

    if (!name) {
      this.errorMessage.set('Naziv dobavljača je obavezan.');
      return;
    }
    if (name.length < 2 || name.length > 100) {
      this.errorMessage.set('Naziv mora imati između 2 i 100 znakova.');
      return;
    }
    if (website && !this.urlPattern.test(website)) {
      this.errorMessage.set('Website mora biti validan URL.');
      return;
    }
    if (website && website.length > 200) {
      this.errorMessage.set('Website može imati najviše 200 znakova.');
      return;
    }

    this.errorMessage.set(null);
    this.supplierService.updateSupplier(id, {
      name,
      website: website || undefined
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: () => {
        this.cancelEdit();
        this.loadSuppliers();
      },
      error: (err) => {
        this.errorMessage.set(err.error?.error || 'Greška pri ažuriranju dobavljača.');
      }
    });
  }

  protected async deleteSupplier(supplier: SupplierResponse) {
    const confirmed = await this.confirmDialog.open({
      title: 'Obriši dobavljača',
      message: `Da li ste sigurni da želite obrisati dobavljača "${supplier.name}"?`,
      confirmText: 'Obriši',
      cancelText: 'Otkaži',
      confirmButtonClass: 'danger'
    });

    if (confirmed) {
      this.confirmDialog.setLoading(true);
      this.supplierService.deleteSupplier(supplier.id).pipe(takeUntil(this.destroy$)).subscribe({
        next: () => {
          this.confirmDialog.close();
          this.loadSuppliers();
        },
        error: (err) => {
          this.confirmDialog.setError(err.error?.error || 'Greška pri brisanju dobavljača.');
        }
      });
    }
  }
}
