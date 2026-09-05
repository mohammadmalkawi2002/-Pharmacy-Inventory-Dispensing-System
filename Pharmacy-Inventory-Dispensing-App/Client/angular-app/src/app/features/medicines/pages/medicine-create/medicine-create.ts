import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { MedicineService } from '../../services/medicine.service';
import { CreateMedicineDto } from '../../models/medicine.models';
import { MedicineFormComponent } from '../../components/medicine-form/medicine-form.component';

@Component({
  selector: 'app-medicine-create',
  standalone: true,
  imports: [MedicineFormComponent],
  templateUrl: './medicine-create.html'
})
export class MedicineCreate {
  readonly #medicineService = inject(MedicineService);
  readonly #router = inject(Router);
  readonly #messageService = inject(MessageService);

  isSubmitting = signal(false);

  onSave(dto: CreateMedicineDto): void {
    this.isSubmitting.set(true);
    this.#medicineService.createMedicine(dto).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.#messageService.add({ severity: 'success', summary: 'Medicine Added', detail: 'New medicine added to catalog successfully.', life: 3000 });
        this.#router.navigate(['/medicines']);
      },
      error: () => this.isSubmitting.set(false)
    });
  }

  onCancel(): void {
    this.#router.navigate(['/medicines']);
  }
}
