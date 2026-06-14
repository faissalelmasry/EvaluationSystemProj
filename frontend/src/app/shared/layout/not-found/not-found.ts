import { Component } from '@angular/core';

@Component({
  selector: 'app-not-found',
  standalone: true,
  template: `
    <div class="not-found-container">
      <h1>404 Not Found</h1>
      <p>The page you are looking for does not exist.</p>
      <a routerLink="/admin" class="back-link">Go back to Admin Panel</a>
    </div>
  `,
  styles: [
    `
      .not-found-container {
        min-height: calc(100vh - 120px);
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        text-align: center;
        padding: 2rem;
      }

      .not-found-container h1 {
        font-size: 3rem;
        margin-bottom: 1rem;
        color: #1f2937;
      }

      .not-found-container p {
        color: #475569;
        margin-bottom: 1.5rem;
        max-width: 36rem;
      }

      .back-link {
        display: inline-block;
        padding: 0.9rem 1.4rem;
        border-radius: 999px;
        background: #4d42ff;
        color: #ffffff;
        text-decoration: none;
        font-weight: 600;
      }

      .back-link:hover {
        background: #3c31c8;
      }
    `,
  ],
})
export class NotFoundComponent {}
