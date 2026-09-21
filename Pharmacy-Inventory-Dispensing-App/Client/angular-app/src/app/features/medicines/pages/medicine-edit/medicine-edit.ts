import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { MedicineService } from '../../services/medicine.service';
import { Medicine, UpdateMedicineDto } from '../../models/medicine.models';
import { MedicineFormComponent, MedicineFormSaveEvent } from '../../components/medicine-form/medicine-form.component';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'app-medicine-edit',
  standalone: true,
  imports: [MedicineFormComponent, EmptyStateComponent],
  templateUrl: './medicine-edit.html'
})
export class MedicineEdit implements OnInit {
  readonly #medicineService = inject(MedicineService);
  readonly #router = inject(Router);
  readonly #route = inject(ActivatedRoute);
  readonly #messageService = inject(MessageService);

  isSubmitting = signal(false);
  isLoading = signal(true);
  loadError = signal(false);
  medicine = signal<Medicine | undefined>(undefined);
  medicineId = signal<string | null>(null);

  ngOnInit(): void {
    const id = this.#route.snapshot.paramMap.get('id');
    this.medicineId.set(id);

    if (!id) {
      this.loadError.set(true);
      this.isLoading.set(false);
      return;
    }

    this.#medicineService.getMedicine(id).subscribe({
      next: (data) => {
        this.medicine.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.loadError.set(true);
        this.isLoading.set(false);
      }
    });
  }

  onSave(event: MedicineFormSaveEvent): void {
    if (!this.medicineId()) return;
    
    this.isSubmitting.set(true);
    this.#medicineService.updateMedicine(this.medicineId()!, event.dto as UpdateMedicineDto, event.image).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.#messageService.add({ severity: 'success', summary: 'Medicine Updated', detail: 'Medicine details updated successfully.', life: 3000 });
        this.#router.navigate(['/medicines']);
      },
      error: () => this.isSubmitting.set(false)
    });
  }

  onCancel(): void {
    this.#router.navigate(['/medicines']);
  }
}
