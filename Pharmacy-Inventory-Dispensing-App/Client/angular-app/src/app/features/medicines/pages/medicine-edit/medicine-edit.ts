import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { MedicineService } from '../../services/medicine.service';
import { Medicine, UpdateMedicineDto, CreateMedicineDto } from '../../models/medicine.models';
import { MedicineFormComponent } from '../../components/medicine-form/medicine-form.component';
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
  medicine = signal<Medicine | undefined>(undefined);
  medicineId = signal<string | null>(null);

  ngOnInit(): void {
    this.medicineId.set(this.#route.snapshot.paramMap.get('id'));
    
    // TEMPORARY Router-state handling
    const navigation = this.#router.getCurrentNavigation();
    if (navigation?.extras.state && navigation.extras.state['data']) {
      this.medicine.set(navigation.extras.state['data'] as Medicine);
    } else {
      const historyState = window.history.state;
      if (historyState && historyState['data']) {
        this.medicine.set(historyState['data'] as Medicine);
      }
    }
  }

  onSave(dto: CreateMedicineDto): void {
    if (!this.medicineId()) return;
    
    this.isSubmitting.set(true);
    this.#medicineService.updateMedicine(this.medicineId()!, dto as UpdateMedicineDto).subscribe({
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
