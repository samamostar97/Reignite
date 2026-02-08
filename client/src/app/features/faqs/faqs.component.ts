import { Component, inject, signal, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { FaqService } from '../../core/services/faq.service';
import { FaqResponse } from '../../core/models/faq.model';
import { HeaderComponent } from '../../core/layout/header/header.component';
import { FooterComponent } from '../../shared/components/footer/footer.component';

@Component({
  selector: 'app-faqs',
  standalone: true,
  imports: [CommonModule, HeaderComponent, FooterComponent],
  templateUrl: './faqs.component.html',
  styleUrl: './faqs.component.scss'
})
export class FaqsComponent implements OnInit, OnDestroy {
  private readonly faqService = inject(FaqService);
  private readonly destroy$ = new Subject<void>();

  protected readonly faqs = signal<FaqResponse[]>([]);
  protected readonly isLoading = signal(true);
  protected readonly expandedFaqId = signal<number | null>(null);

  ngOnInit() {
    this.loadFaqs();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadFaqs() {
    this.isLoading.set(true);
    this.faqService.getAll({ pageSize: 100 }).pipe(takeUntil(this.destroy$)).subscribe({
      next: (result) => {
        this.faqs.set(result.items);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  protected toggleFaq(faqId: number): void {
    if (this.expandedFaqId() === faqId) {
      this.expandedFaqId.set(null);
    } else {
      this.expandedFaqId.set(faqId);
    }
  }

  protected isExpanded(faqId: number): boolean {
    return this.expandedFaqId() === faqId;
  }
}
