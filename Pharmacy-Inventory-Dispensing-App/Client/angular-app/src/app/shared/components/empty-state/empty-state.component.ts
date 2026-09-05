import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [ButtonModule],
  template: `
    <div class="empty-state-container card-glass">
      <div class="empty-icon-box">
        <i [class]="icon + ' empty-icon'"></i>
      </div>
      <h3 class="empty-title">{{ title }}</h3>
      @if (description) {
        <p class="empty-desc">{{ description }}</p>
      }
      @if (actionLabel) {
        <div class="empty-action">
          <p-button
            [label]="actionLabel"
            [icon]="actionIcon"
            (onClick)="action.emit()"
            severity="primary"
            styleClass="shadow px-6" />
        </div>
      }
    </div>
  `,
  styles: [`
    .empty-state-container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 3.5rem 1.5rem;
      text-align: center;
      border: 1px dashed var(--surface-300);
      border-radius: var(--border-radius);
      margin: 1rem 0;
    }

    [data-theme='dark'] .empty-state-container {
      border-color: var(--surface-700);
    }

    .empty-icon-box {
      width: 4.5rem;
      height: 4.5rem;
      border-radius: 1.25rem;
      background: var(--primary-color-light);
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 1.25rem;
      box-shadow: 0 4px 12px rgba(2, 132, 199, 0.15);
    }

    .empty-icon {
      font-size: 2rem;
      color: var(--primary-color);
    }

    .empty-title {
      font-size: 1.15rem;
      font-weight: 800;
      color: var(--text-color);
      margin-bottom: 0.35rem;
      letter-spacing: -0.01em;
    }

    .empty-desc {
      font-size: 0.875rem;
      color: var(--text-color-secondary);
      max-width: 26rem;
      margin-bottom: 1.5rem;
      line-height: 1.5;
    }

    .empty-action {
      margin-top: 0.25rem;
    }
  `]
})
export class EmptyStateComponent {
  @Input() title = 'No Data Available';
  @Input() description = 'No records were found matching your current filters.';
  @Input() icon = 'pi pi-folder-open';
  @Input() actionLabel?: string;
  @Input() actionIcon = 'pi pi-plus';
  @Output() action = new EventEmitter<void>();
}
