import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { PatientService } from '../../services/patient.service';
import { Patient, UpdatePatientDto, CreatePatientDto } from '../../models/patient.models';
import { PatientFormComponent } from '../../components/patient-form/patient-form.component';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { LoadingComponent } from '../../../../shared/components/loading/loading.component';

@Component({
  selector: 'app-patient-edit',
  standalone: true,
  imports: [PatientFormComponent, EmptyStateComponent, LoadingComponent],
  templateUrl: './patient-edit.html'
})
export class PatientEdit implements OnInit {
  readonly #patientService = inject(PatientService);
  readonly #router = inject(Router);
  readonly #route = inject(ActivatedRoute);
  readonly #messageService = inject(MessageService);

  isSubmitting = signal(false);
  isLoading = signal(true);
  patient = signal<Patient | undefined>(undefined);
  patientId = signal<string | null>(null);

  ngOnInit(): void {
    const id = this.#route.snapshot.paramMap.get('id');
    this.patientId.set(id);
    
    if (id) {
      this.#patientService.getPatient(id).subscribe({
        next: (data) => {
          this.patient.set(data);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.#messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to fetch patient details.', life: 3000 });
        }
      });
    } else {
      this.isLoading.set(false);
    }
  }

  onSave(dto: CreatePatientDto): void {
    if (!this.patientId()) return;

    const payload: UpdatePatientDto = {
      documentId: dto.documentId,
      fullName: dto.fullName,
      phoneNumber: dto.phoneNumber,
      dateOfBirth: dto.dateOfBirth ? new Date(`${dto.dateOfBirth}T00:00:00.000Z`).toISOString() : ''
    };
    
    this.isSubmitting.set(true);
    this.#patientService.updatePatient(this.patientId()!, payload).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.#messageService.add({ severity: 'success', summary: 'Patient Updated', detail: 'Patient information has been updated successfully.', life: 3000 });
        this.#router.navigate(['/patients']);
      },
      error: () => this.isSubmitting.set(false)
    });
  }

  onCancel(): void {
    this.#router.navigate(['/patients']);
  }
}
