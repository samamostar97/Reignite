import { Component, inject, signal, computed, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { FaqService } from '../../../../core/services/faq.service';
import { FaqResponse } from '../../../../core/models/faq.model';
import { ConfirmDialogService } from '../../../../shared/services/confirm-dialog.service';
import { NotificationService } from '../../../../core/services/notification.service';

@Component({
  selector: 'app-faq-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './faq-list.component.html',
  styleUrl: './faq-list.component.scss'
})
export class FaqListComponent implements OnInit, OnDestroy {
  private readonly faqService = inject(FaqService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly notification = inject(NotificationService);
  private readonly destroy$ = new Subject<void>();
  private readonly searchSubject = new Subject<string>();

  protected readonly faqs = signal<FaqResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly totalCount = signal(0);
  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal(10);
  protected readonly searchQuery = signal('');

  protected readonly totalPages = computed(() =>
    Math.ceil(this.totalCount() / this.pageSize())
  );

  ngOnInit() {
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(query => {
      this.searchQuery.set(query);
      this.currentPage.set(1);
      this.loadFaqs();
    });

    this.loadFaqs();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadFaqs() {
    this.isLoading.set(true);
    this.faqService.getAll({
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      search: this.searchQuery() || undefined
    }).pipe(takeUntil(this.destroy$)).subscribe({
      next: (result) => {
        this.faqs.set(result.items);
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
    this.loadFaqs();
  }

  protected async deleteFaq(id: number, question: string): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Brisanje pitanja',
      message: `Da li ste sigurni da želite obrisati pitanje "${question}"?`,
      confirmText: 'Obriši',
      cancelText: 'Otkaži'
    });

    if (!confirmed) return;

    this.faqService.delete(id).subscribe({
      next: () => {
        this.notification.success('Pitanje je uspješno obrisano.');
        this.loadFaqs();
      },
      error: () => {
        this.notification.error('Greška prilikom brisanja pitanja.');
      }
    });
  }

  protected formatDate(dateString?: string): string {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleDateString('bs-BA', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric'
    });
  }

  protected truncateText(text: string, maxLength: number = 100): string {
    if (text.length <= maxLength) return text;
    return text.substring(0, maxLength) + '...';
  }
}
