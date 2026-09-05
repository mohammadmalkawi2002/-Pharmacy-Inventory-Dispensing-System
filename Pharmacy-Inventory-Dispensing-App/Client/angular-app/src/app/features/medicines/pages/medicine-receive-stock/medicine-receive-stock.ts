import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { MedicineService } from '../../services/medicine.service';
import { Medicine } from '../../models/medicine.models';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'app-medicine-receive-stock',
  standalone: true,
  imports: [FormsModule, ButtonModule, InputNumberModule, EmptyStateComponent],
  templateUrl: './medicine-receive-stock.html'
})
export class MedicineReceiveStock implements OnInit {
  readonly #medicineService = inject(MedicineService);
  readonly #router = inject(Router);
  readonly #route = inject(ActivatedRoute);
  readonly #messageService = inject(MessageService);

  isSubmitting = signal(false);
  medicine = signal<Medicine | undefined>(undefined);
  medicineId = signal<string | null>(null);
  packageQuantity = 0;

  ngOnInit(): void {
    const id = this.#route.snapshot.paramMap.get('id');
    this.medicineId.set(id);
    
    // TEMPORARY Router-state handling
    const navigation = this.#router.getCurrentNavigation();
    if (navigation?.extras.state && navigation.extras.state['data']) {
      this.medicine.set(navigation.extras.state['data'] as Medicine);
    } else {
      const historyState = window.history.state;
      if (historyState && historyState['data']) {
        this.medicine.set(historyState['data'] as Medicine);
      } else if (id) {
        // Fetch medicine if not found in state
        this.#medicineService.getMedicine(id).subscribe({
          next: (med) => this.medicine.set(med)
        });
      }
    }
  }

  onReceiveStock(): void {
    const currentMedicine = this.medicine();
    if (!currentMedicine || !this.medicineId() || this.packageQuantity <= 0) return;
    
    this.isSubmitting.set(true);
    this.#medicineService.receiveStock(this.medicineId()!, { packageQuantity: this.packageQuantity }).subscribe({
      next: (response) => {
        this.isSubmitting.set(false);
        this.#messageService.add({ severity: 'success', summary: 'Stock Received', detail: `${response.receivedQuantity} ${response.stockUnit}s added successfully. New total: ${response.newQuantity}.`, life: 4000 });
        this.#router.navigate(['/medicines'], { state: { viewMedicineId: this.medicineId() } });
      },
      error: () => this.isSubmitting.set(false)
    });
  }

  onCancel(): void {
    this.#router.navigate(['/medicines']);
  }
}
