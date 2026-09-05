import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { PrescriptionService } from '../../services/prescription.service';
import { CreatePrescriptionDto } from '../../models/prescription.models';
import { PrescriptionFormComponent } from '../../components/prescription-form/prescription-form.component';

@Component({
  selector: 'app-prescription-create',
  standalone: true,
  imports: [PrescriptionFormComponent],
  templateUrl: './prescription-create.html'
})
export class PrescriptionCreate {
  readonly #prescriptionService = inject(PrescriptionService);
  readonly #router = inject(Router);
  readonly #messageService = inject(MessageService);

  isSubmitting = signal(false);

  onSave(dto: CreatePrescriptionDto): void {
    this.isSubmitting.set(true);
    this.#prescriptionService.createPrescription(dto).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.#messageService.add({
          severity: 'success',
          summary: 'Prescription Created',
          detail: 'Prescription has been created successfully.',
          life: 3000
        });
        this.#router.navigate(['/prescriptions']);
      },
      error: () => this.isSubmitting.set(false)
    });
  }

  onCancel(): void {
    this.#router.navigate(['/prescriptions']);
  }
}
