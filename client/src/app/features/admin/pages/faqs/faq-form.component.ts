import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { FaqService } from '../../../../core/services/faq.service';
import { CreateFaqRequest, UpdateFaqRequest } from '../../../../core/models/faq.model';
import { NotificationService } from '../../../../core/services/notification.service';

@Component({
  selector: 'app-faq-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './faq-form.component.html',
  styleUrl: './faq-form.component.scss'
})
export class FaqFormComponent implements OnInit {
  private readonly faqService = inject(FaqService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly notification = inject(NotificationService);

  protected readonly isLoading = signal(true);
  protected readonly isSaving = signal(false);
  protected readonly isEditMode = signal(false);
  protected readonly faqId = signal<number | null>(null);

  // Form fields
  protected question = '';
  protected answer = '';

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode.set(true);
      this.faqId.set(+id);
      this.loadFaq(+id);
    } else {
      this.isLoading.set(false);
    }
  }

  private loadFaq(id: number) {
    this.faqService.getById(id).subscribe({
      next: (faq) => {
        this.question = faq.question;
        this.answer = faq.answer;
        this.isLoading.set(false);
      },
      error: () => {
        this.notification.error({ title: 'Greška', message: 'Greška prilikom učitavanja pitanja.' });
        this.router.navigate(['/admin/faqs']);
      }
    });
  }

  protected onSubmit() {
    if (!this.validateForm()) return;

    this.isSaving.set(true);

    const request: CreateFaqRequest | UpdateFaqRequest = {
      question: this.question.trim(),
      answer: this.answer.trim()
    };

    const operation = this.isEditMode()
      ? this.faqService.update(this.faqId()!, request as UpdateFaqRequest)
      : this.faqService.create(request as CreateFaqRequest);

    operation.subscribe({
      next: () => {
        this.notification.success({
          title: 'Uspješno spremljeno',
          message: this.isEditMode() ? 'Pitanje je uspješno ažurirano.' : 'Pitanje je uspješno kreirano.'
        });
        this.router.navigate(['/admin/faqs']);
      },
      error: (error) => {
        this.isSaving.set(false);
        const errorMessage = error?.error?.error || 'Greška prilikom spremanja pitanja.';
        this.notification.error({ title: 'Greška', message: errorMessage });
      }
    });
  }

  private validateForm(): boolean {
    if (!this.question.trim()) {
      this.notification.error({ title: 'Greška', message: 'Pitanje je obavezno.' });
      return false;
    }

    if (this.question.length > 500) {
      this.notification.error({ title: 'Greška', message: 'Pitanje ne smije biti duže od 500 karaktera.' });
      return false;
    }

    if (!this.answer.trim()) {
      this.notification.error({ title: 'Greška', message: 'Odgovor je obavezan.' });
      return false;
    }

    if (this.answer.length > 2000) {
      this.notification.error({ title: 'Greška', message: 'Odgovor ne smije biti duži od 2000 karaktera.' });
      return false;
    }

    return true;
  }

  protected get questionCharCount(): string {
    return `${this.question.length} / 500`;
  }

  protected get answerCharCount(): string {
    return `${this.answer.length} / 2000`;
  }
}
