import { Component, ChangeDetectionStrategy } from '@angular/core';

@Component({
  selector: 'app-password-tab',
  standalone: true,
  template: `
    <div class="tab-panel">
      <h3 class="tab-title">Lozinka</h3>
      <p class="tab-description">Promijenite svoju lozinku.</p>
      <div class="placeholder">
        <p>Učitavanje...</p>
      </div>
    </div>
  `,
  styles: [`
    .tab-panel {
      background: #1A1410;
      border-radius: 16px;
      border: 1px solid rgba(255, 255, 255, 0.04);
      padding: 2rem;
    }
    .tab-title {
      font-family: 'Cinzel', serif;
      font-size: 1.35rem;
      font-weight: 700;
      color: #FAF5F0;
      margin: 0 0 0.25rem 0;
    }
    .tab-description {
      color: #7A6B59;
      font-size: 0.9rem;
      margin: 0 0 2rem 0;
    }
    .placeholder {
      text-align: center;
      padding: 3rem;
      color: #7A6B59;
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PasswordTabComponent {}
