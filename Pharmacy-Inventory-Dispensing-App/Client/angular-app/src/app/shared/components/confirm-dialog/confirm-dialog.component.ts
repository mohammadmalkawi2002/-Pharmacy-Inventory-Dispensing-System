import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule, ConfirmDialogModule],
  template: `
    <p-confirmdialog [key]="key" [style]="{ width: '450px', maxWidth: '90vw' }">
      <ng-template #message let-msg>
        <div class="flex items-center gap-3 p-2">
          <i [class]="msg.icon || 'pi pi-exclamation-triangle'" class="text-3xl text-amber-500"></i>
          <div>
            <div class="font-bold text-lg text-surface-900">{{ msg.header || 'تأكيد العملية' }}</div>
            <div class="text-sm text-surface-600 mt-1">{{ msg.message }}</div>
          </div>
        </div>
      </ng-template>
    </p-confirmdialog>
  `
})
export class ConfirmDialogComponent {
  @Input() key = 'global-confirm';
}
