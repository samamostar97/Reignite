import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="placeholder-page">
      <div class="placeholder-icon">
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" d="M9 12h3.75M9 15h3.75M9 18h3.75m3 .75H18a2.25 2.25 0 0 0 2.25-2.25V6.108c0-1.135-.845-2.098-1.976-2.192a48.424 48.424 0 0 0-1.123-.08m-5.801 0c-.065.21-.1.433-.1.664 0 .414.336.75.75.75h4.5a.75.75 0 0 0 .75-.75 2.25 2.25 0 0 0-.1-.664m-5.8 0A2.251 2.251 0 0 1 13.5 2.25H15c1.012 0 1.867.668 2.15 1.586m-5.8 0c-.376.023-.75.05-1.124.08C9.095 4.01 8.25 4.973 8.25 6.108V8.25m0 0H4.875c-.621 0-1.125.504-1.125 1.125v11.25c0 .621.504 1.125 1.125 1.125h9.75c.621 0 1.125-.504 1.125-1.125V9.375c0-.621-.504-1.125-1.125-1.125H8.25ZM6.75 12h.008v.008H6.75V12Zm0 3h.008v.008H6.75V15Zm0 3h.008v.008H6.75V18Z" />
        </svg>
      </div>
      <h2>Upravljanje Narudžbama</h2>
      <p>Ova funkcionalnost će uskoro biti dostupna.</p>
      <div class="features-preview">
        <h4>Planirane funkcije:</h4>
        <ul>
          <li>Pregled svih narudžbi</li>
          <li>Promjena statusa narudžbe</li>
          <li>Detalji narudžbe i stavke</li>
          <li>Statistika prodaje</li>
        </ul>
      </div>
    </div>
  `,
  styles: [`
    .placeholder-page {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      min-height: 60vh;
      text-align: center;
      padding: 2rem;
    }

    .placeholder-icon {
      width: 80px;
      height: 80px;
      background: rgba(255, 107, 53, 0.1);
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 1.5rem;

      svg {
        width: 40px;
        height: 40px;
        color: #ff6b35;
      }
    }

    h2 {
      font-size: 1.5rem;
      font-weight: 600;
      color: #2c1810;
      margin: 0 0 0.5rem 0;
      font-family: 'Georgia', serif;
    }

    p {
      color: #5a3a2a;
      margin: 0 0 2rem 0;
    }

    .features-preview {
      background: #fff;
      padding: 1.5rem 2rem;
      border-radius: 12px;
      border: 1px solid rgba(0, 0, 0, 0.08);
      text-align: left;

      h4 {
        font-size: 0.9rem;
        color: #2c1810;
        margin: 0 0 1rem 0;
        text-transform: uppercase;
        letter-spacing: 0.05em;
      }

      ul {
        margin: 0;
        padding-left: 1.25rem;

        li {
          color: #5a3a2a;
          padding: 0.25rem 0;
        }
      }
    }
  `]
})
export class OrderListComponent {}
