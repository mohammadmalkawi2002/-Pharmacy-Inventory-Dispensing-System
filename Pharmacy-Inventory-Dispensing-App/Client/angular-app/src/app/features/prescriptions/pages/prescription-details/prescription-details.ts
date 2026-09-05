import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { Prescription, getPrescriptionStatusSeverity, PrescriptionStatus } from '../../models/prescription.models';
import { PrescriptionService } from '../../services/prescription.service';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { LoadingComponent } from '../../../../shared/components/loading/loading.component';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-prescription-details',
  standalone: true,
  imports: [TagModule, ButtonModule, TooltipModule, EmptyStateComponent, LoadingComponent, DatePipe],
  templateUrl: './prescription-details.html'
})
export class PrescriptionDetails implements OnInit {
  readonly #router = inject(Router);
  readonly #route = inject(ActivatedRoute);
  readonly #prescriptionService = inject(PrescriptionService);

  prescription = signal<Prescription | undefined>(undefined);
  prescriptionId = signal<string | null>(null);
  isLoading = signal<boolean>(true);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    const id = this.#route.snapshot.paramMap.get('id');
    this.prescriptionId.set(id);

    // Initial seed from router state if available
    const navigation = this.#router.getCurrentNavigation();
    const stateData = navigation?.extras.state?.['data'] || window.history.state?.['data'];
    if (stateData) {
      this.prescription.set(stateData as Prescription);
    }

    if (id) {
      this.loadPrescription(id);
    } else {
      this.isLoading.set(false);
    }
  }

  loadPrescription(id: string): void {
    if (!this.prescription()) {
      this.isLoading.set(true);
    }
    this.#prescriptionService.getPrescription(id).subscribe({
      next: (data) => {
        this.prescription.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error fetching prescription details', err);
        this.errorMessage.set('Failed to load prescription details.');
        this.isLoading.set(false);
      }
    });
  }

  getStatusSeverity(status: PrescriptionStatus): 'success' | 'danger' | 'warn' | 'secondary' {
    return getPrescriptionStatusSeverity(status);
  }

  onClose(): void {
    this.#router.navigate(['/prescriptions']);
  }
}
