import { Injectable, signal } from '@angular/core';
import { Subject } from 'rxjs';

export interface ConfirmDialogConfig {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  confirmButtonClass?: 'danger' | 'primary';
}

export interface ConfirmDialogState extends ConfirmDialogConfig {
  isOpen: boolean;
  isLoading: boolean;
  error: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class ConfirmDialogService {
  private readonly _state = signal<ConfirmDialogState>({
    isOpen: false,
    isLoading: false,
    error: null,
    title: '',
    message: '',
    confirmText: 'Obriši',
    cancelText: 'Otkaži',
    confirmButtonClass: 'danger'
  });

  private confirmSubject = new Subject<boolean>();

  readonly state = this._state.asReadonly();

  open(config: ConfirmDialogConfig): Promise<boolean> {
    this._state.set({
      isOpen: true,
      isLoading: false,
      error: null,
      title: config.title,
      message: config.message,
      confirmText: config.confirmText ?? 'Obriši',
      cancelText: config.cancelText ?? 'Otkaži',
      confirmButtonClass: config.confirmButtonClass ?? 'danger'
    });

    return new Promise<boolean>((resolve) => {
      const subscription = this.confirmSubject.subscribe((result) => {
        subscription.unsubscribe();
        resolve(result);
      });
    });
  }

  confirm(): void {
    this.confirmSubject.next(true);
  }

  cancel(): void {
    this._state.update(state => ({ ...state, isOpen: false, error: null }));
    this.confirmSubject.next(false);
  }

  setLoading(loading: boolean): void {
    this._state.update(state => ({ ...state, isLoading: loading }));
  }

  setError(error: string): void {
    this._state.update(state => ({ ...state, error, isLoading: false }));
  }

  close(): void {
    this._state.update(state => ({ ...state, isOpen: false, error: null, isLoading: false }));
  }
}
