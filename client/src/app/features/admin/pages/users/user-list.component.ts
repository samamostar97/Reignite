import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="placeholder-page">
      <div class="placeholder-icon">
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" d="M15 19.128a9.38 9.38 0 0 0 2.625.372 9.337 9.337 0 0 0 4.121-.952 4.125 4.125 0 0 0-7.533-2.493M15 19.128v-.003c0-1.113-.285-2.16-.786-3.07M15 19.128v.106A12.318 12.318 0 0 1 8.624 21c-2.331 0-4.512-.645-6.374-1.766l-.001-.109a6.375 6.375 0 0 1 11.964-3.07M12 6.375a3.375 3.375 0 1 1-6.75 0 3.375 3.375 0 0 1 6.75 0Zm8.25 2.25a2.625 2.625 0 1 1-5.25 0 2.625 2.625 0 0 1 5.25 0Z" />
        </svg>
      </div>
      <h2>Upravljanje Korisnicima</h2>
      <p>Ova funkcionalnost će uskoro biti dostupna.</p>
      <div class="features-preview">
        <h4>Planirane funkcije:</h4>
        <ul>
          <li>Pregled svih korisnika</li>
          <li>Promjena korisničkih uloga</li>
          <li>Deaktivacija/aktivacija računa</li>
          <li>Pregled korisničke aktivnosti</li>
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
export class UserListComponent {}
