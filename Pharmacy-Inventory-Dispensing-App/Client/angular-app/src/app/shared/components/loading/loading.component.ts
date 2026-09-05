import { Component, Input } from '@angular/core';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { SkeletonModule } from 'primeng/skeleton';

@Component({
  selector: 'app-loading',
  standalone: true,
  imports: [ProgressSpinnerModule, SkeletonModule],
  template: `
    @if (type === 'spinner') {
      <div class="flex flex-col items-center justify-center p-8 gap-4 min-h-[180px]">
        <div class="spinner-container">
          <p-progressspinner
            styleClass="w-12 h-12"
            strokeWidth="4"
            fill="transparent"
            animationDuration=".8s"
            ariaLabel="loading" />
        </div>
        @if (text) {
          <p class="text-sm font-medium text-surface-500 animate-pulse">{{ text }}</p>
        }
      </div>
    }

    @if (type === 'skeleton') {
      <div class="w-full space-y-4 p-4">
        <div class="flex items-center gap-4">
          <p-skeleton shape="circle" size="3rem" />
          <div class="flex-1 space-y-2">
            <p-skeleton width="60%" height="1.25rem" />
            <p-skeleton width="40%" height="1rem" />
          </div>
        </div>
        <p-skeleton width="100%" height="4rem" borderRadius="8px" />
        <div class="grid grid-cols-3 gap-4 pt-2">
          <p-skeleton height="2.5rem" />
          <p-skeleton height="2.5rem" />
          <p-skeleton height="2.5rem" />
        </div>
      </div>
    }
  `,
  styles: [`
    :host {
      display: block;
      width: 100%;
    }

    .spinner-container {
      display: flex;
      align-items: center;
      justify-content: center;
      filter: drop-shadow(0 2px 8px rgba(2, 132, 199, 0.25));
    }
  `]
})
export class LoadingComponent {
  @Input() text = 'Loading data...';
  @Input() type: 'spinner' | 'skeleton' = 'spinner';
}
