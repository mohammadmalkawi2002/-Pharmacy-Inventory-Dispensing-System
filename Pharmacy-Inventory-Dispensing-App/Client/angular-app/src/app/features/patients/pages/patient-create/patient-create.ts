import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { PatientService } from '../../services/patient.service';
import { CreatePatientDto } from '../../models/patient.models';
import { PatientFormComponent } from '../../components/patient-form/patient-form.component';

@Component({
  selector: 'app-patient-create',
  standalone: true,
  imports: [PatientFormComponent],
  templateUrl: './patient-create.html'
})
export class PatientCreate {
  readonly #patientService = inject(PatientService);
  readonly #router = inject(Router);
  readonly #messageService = inject(MessageService);

  isSubmitting = signal(false);

  onSave(dto: CreatePatientDto): void {
    const payload: CreatePatientDto = {
      documentId: dto.documentId,
      fullName: dto.fullName,
      phoneNumber: dto.phoneNumber,
      dateOfBirth: dto.dateOfBirth ? new Date(`${dto.dateOfBirth}T00:00:00.000Z`).toISOString() : ''
    };

    this.isSubmitting.set(true);
    this.#patientService.createPatient(payload).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.#messageService.add({ severity: 'success', summary: 'Patient Created', detail: 'New patient record has been created successfully.', life: 3000 });
        this.#router.navigate(['/patients']);
      },
      error: () => this.isSubmitting.set(false)
    });
  }

  onCancel(): void {
    this.#router.navigate(['/patients']);
  }
}
