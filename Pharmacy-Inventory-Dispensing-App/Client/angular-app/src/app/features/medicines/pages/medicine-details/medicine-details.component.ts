import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Medicine, getStockSeverity } from '../../models/medicine.models';
import { MedicineService } from '../../services/medicine.service';
import { DatePipe } from '@angular/common';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { LoadingComponent } from '../../../../shared/components/loading/loading.component';

@Component({
  selector: 'app-medicine-details',
  standalone: true,
  imports: [TagModule, ButtonModule, DatePipe, EmptyStateComponent, LoadingComponent],
  templateUrl: './medicine-details.component.html'
})
export class MedicineDetailsComponent implements OnInit {
  readonly #medicineService = inject(MedicineService);
  readonly #router = inject(Router);
  readonly #route = inject(ActivatedRoute);

  medicine = signal<Medicine | null>(null);
  isLoading = signal(true);
  error = signal<string | null>(null);

  ngOnInit(): void {
    const id = this.#route.snapshot.paramMap.get('id');
    if (!id) {
      this.error.set('Medicine ID not provided in the route.');
      this.isLoading.set(false);
      return;
    }

    // Try to grab from router state first to avoid blink, but still fetch to ensure we have full details
    const navigation = this.#router.getCurrentNavigation();
    if (navigation?.extras.state && navigation.extras.state['data']) {
      this.medicine.set(navigation.extras.state['data'] as Medicine);
    } else {
      const historyState = window.history.state;
      if (historyState && historyState['data']) {
        this.medicine.set(historyState['data'] as Medicine);
      }
    }

    this.#medicineService.getMedicine(id).subscribe({
      next: (data) => {
        this.medicine.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Failed to load medicine details. It might have been deleted or you lack permissions.');
        this.isLoading.set(false);
      }
    });
  }

  getStockSeverity(status: string) {
    return getStockSeverity(status);
  }

  onBack(): void {
    this.#router.navigate(['/medicines']);
  }
}
