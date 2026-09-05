import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { DispensingService } from '../../services/dispensing.service';
import {
  LookupPrescriptionResponse,
  LookupPrescriptionItemDto,
  CreateDispenseRequest,
  DispenseDetailsDto
} from '../../models/dispensing.models';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { StepsModule } from 'primeng/steps';
import { MessageService } from 'primeng/api';
import { MenuItem } from 'primeng/api';
import { CheckboxModule } from 'primeng/checkbox';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { DatePipe } from '@angular/common';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { Permissions } from '../../../../core/auth/auth.models';

@Component({
  selector: 'app-dispensing-workflow',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    RouterModule,
    ButtonModule,
    InputTextModule,
    StepsModule,
    CheckboxModule,
    TagModule,
    TooltipModule,
    DatePipe,
    HasPermissionDirective
  ],
  templateUrl: './dispensing-workflow.component.html',
  styleUrls: ['./dispensing-workflow.component.css']
})
export class DispensingWorkflowComponent implements OnInit {
  readonly #fb = inject(FormBuilder);
  readonly #router = inject(Router);
  readonly #dispensingService = inject(DispensingService);
  readonly #messageService = inject(MessageService);
  readonly Permissions = Permissions;

  steps: MenuItem[] = [];
  currentStep = signal(0);
  isSubmitting = signal(false);

  lookupForm!: FormGroup;
  prescription = signal<LookupPrescriptionResponse | null>(null);
  dispenseResult = signal<DispenseDetailsDto | null>(null);
  
  // Track items to dispense
  dispenseItems = signal<LookupPrescriptionItemDto[]>([]);

  ngOnInit(): void {
    this.steps = [
      { label: 'Lookup RX' },
      { label: 'Review & Dispense' },
      { label: 'Confirmation' }
    ];

    this.lookupForm = this.#fb.group({
      prescriptionNumber: ['', [Validators.required, Validators.pattern(/^RX-\d{6}$/)]],
      patientDocumentId: ['', [Validators.required, Validators.pattern(/^[12][0-9]{9}$/)]]
    });
  }

  onLookupSubmit(): void {
    if (this.lookupForm.invalid) {
      this.lookupForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    this.#dispensingService.lookupPrescription(this.lookupForm.value).subscribe({
      next: (rx) => {
        this.prescription.set(rx);
        const mappedItems: LookupPrescriptionItemDto[] = rx.items.map(item => ({
          ...item,
          selected: this.isItemSelectableFromRx(item, rx)
        }));
        this.dispenseItems.set(mappedItems);
        this.isSubmitting.set(false);
        this.currentStep.set(1);
      },
      error: () => {
        this.isSubmitting.set(false);
      }
    });
  }

  isItemSelectable(item: LookupPrescriptionItemDto): boolean {
    return this.isItemSelectableFromRx(item, this.prescription());
  }

  private isItemSelectableFromRx(item: LookupPrescriptionItemDto, rx: LookupPrescriptionResponse | null): boolean {
    if (!rx || !rx.canDispense) return false;
    return item.canDispense;
  }

  getItemUnselectableReason(item: LookupPrescriptionItemDto): string | null {
    const rx = this.prescription();
    if (!rx || !rx.canDispense) {
      return rx?.unavailableReason || 'Prescription is unavailable for dispensing';
    }
    if (!item.canDispense) {
      return item.unavailableReason || 'Item is not authorized for dispensing';
    }
    return null;
  }

  toggleItemSelection(index: number, selected: boolean): void {
    const items = [...this.dispenseItems()];
    const item = items[index];
    if (this.isItemSelectable(item)) {
      item.selected = selected;
      this.dispenseItems.set(items);
    }
  }

  isAllSelectableSelected(): boolean {
    const selectableItems = this.dispenseItems().filter(i => this.isItemSelectable(i));
    if (selectableItems.length === 0) return false;
    return selectableItems.every(i => i.selected);
  }

  toggleSelectAll(checked: boolean): void {
    const items = this.dispenseItems().map(item => {
      const selectable = this.isItemSelectable(item);
      return {
        ...item,
        selected: selectable ? checked : false
      };
    });
    this.dispenseItems.set(items);
  }

  canDispenseAll(): boolean {
    const rx = this.prescription();
    if (!rx || !rx.canDispense) return false;
    return this.dispenseItems().some(i => i.selected && this.isItemSelectable(i));
  }

  onDispenseSubmit(): void {
    const rx = this.prescription();
    if (!rx || !this.canDispenseAll() || this.isSubmitting()) return;

    const selectedItems = this.dispenseItems().filter(i => i.selected && this.isItemSelectable(i));

    const request: CreateDispenseRequest = {
      prescriptionId: rx.prescriptionId,
      documentId: rx.patientDocumentId,
      prescriptionItemIds: selectedItems.map(i => i.prescriptionItemId),
      notes: null
    };

    this.isSubmitting.set(true);
    this.#dispensingService.createDispense(request).subscribe({
      next: (result) => {
        this.dispenseResult.set(result);
        this.isSubmitting.set(false);
        this.currentStep.set(2);
        this.#messageService.add({
          severity: 'success',
          summary: 'Dispensed',
          detail: 'Medicines dispensed successfully.',
          life: 3000
        });
      },
      error: () => {
        this.isSubmitting.set(false);
      }
    });
  }

  viewDispenseRecord(): void {
    const result = this.dispenseResult();
    if (result?.id) {
      this.#router.navigate(['/dispensing/records', result.id]);
    }
  }

  resetWorkflow(): void {
    this.lookupForm.reset();
    this.prescription.set(null);
    this.dispenseItems.set([]);
    this.dispenseResult.set(null);
    this.currentStep.set(0);
  }
}
