import { Component, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Patient, CreatePatientDto } from '../../models/patient.models';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-patient-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    ButtonModule,
    InputTextModule
  ],
  templateUrl: './patient-form.component.html',
  styleUrls: ['./patient-form.component.css']
})
export class PatientFormComponent implements OnInit {
  readonly #fb = inject(FormBuilder);

  @Input() patient?: Patient;
  @Input() isSubmitting = false;
  @Output() save = new EventEmitter<CreatePatientDto>();
  @Output() cancel = new EventEmitter<void>();

  patientForm!: FormGroup;
  todayString = new Date().toISOString().split('T')[0];

  ngOnInit(): void {
    this.initForm();
  }

  initForm(): void {
    this.patientForm = this.#fb.group({
      documentId: [
        this.patient?.documentId ?? '',
        [
          Validators.required,
          Validators.pattern(/^[12][0-9]{9}$/)
        ]
      ],

      fullName: [
        this.patient?.fullName ?? '',
        [
          Validators.required,
          Validators.maxLength(200)
        ]
      ],

      dateOfBirth: [
        this.patient?.dateOfBirth?.split('T')[0] ?? '',
        [Validators.required, this.noFutureDateValidator]
      ],

      phoneNumber: [
        this.patient?.phoneNumber ?? '',
        [
          Validators.required,
          Validators.maxLength(16),
          Validators.pattern(/^\+?[0-9]{9,15}$/)
        ]
      ]
    });
  }

  noFutureDateValidator(
    control: AbstractControl
  ): ValidationErrors | null {
    if (!control.value) {
      return null;
    }

    const selectedDate = String(control.value).split('T')[0];
    const todayUtc = new Date().toISOString().split('T')[0];

    return selectedDate > todayUtc
      ? { futureDate: true }
      : null;
  }

  onSubmit(): void {
    if (this.patientForm.invalid) {
      this.patientForm.markAllAsTouched();
      return;
    }
    this.save.emit(this.patientForm.value);
  }
}
