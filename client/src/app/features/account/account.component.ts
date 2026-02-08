import { Component, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-account',
  standalone: true,
  template: `<div class="p-8 text-warm-50"><p>Učitavanje...</p></div>`,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AccountComponent {}
