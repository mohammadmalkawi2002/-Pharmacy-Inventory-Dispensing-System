import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, WritableSignal, inject, signal } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Subject, Subscription, EMPTY, of } from 'rxjs';
import { debounceTime, map, distinctUntilChanged, switchMap, catchError } from 'rxjs/operators';
import { CreatePrescriptionDto } from '../../models/prescription.models';
import { PatientService } from '../../../patients/services/patient.service';
import { MedicineService } from '../../../medicines/services/medicine.service';
import { PatientLookupDto } from '../../../patients/models/patient.models';
import { MedicineLookupDto } from '../../../medicines/models/medicine.models';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TextareaModule } from 'primeng/textarea';
import { SelectModule } from 'primeng/select';
import { InputNumberModule } from 'primeng/inputnumber';

export interface MedicineRowState {
  search$: Subject<string>;
  results: WritableSignal<MedicineLookupDto[]>;
  loading: WritableSignal<boolean>;
  subscription: Subscription;
}

@Component({
  selector: 'app-prescription-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    ButtonModule,
    InputTextModule,
    TextareaModule,
    SelectModule,
    InputNumberModule
  ],
  templateUrl: './prescription-form.component.html',
  styleUrls: ['./prescription-form.component.css']
})
export class PrescriptionFormComponent implements OnInit, OnDestroy {
  readonly #fb = inject(FormBuilder);
  readonly #patientService = inject(PatientService);
  readonly #medicineService = inject(MedicineService);

  @Input() isSubmitting = false;
  @Output() save = new EventEmitter<CreatePrescriptionDto>();
  @Output() cancel = new EventEmitter<void>();

  prescriptionForm!: FormGroup;

  // Patient Lookup State
  patientSearch$ = new Subject<string>();
  patientResults = signal<PatientLookupDto[]>([]);
  patientLoading = signal<boolean>(false);
  private patientSub?: Subscription;

  // Per-row Medicine Lookup State
  medicineRowStates: MedicineRowState[] = [];

  ngOnInit(): void {
    this.initForm();
    this.setupPatientLookup();
  }

  ngOnDestroy(): void {
    this.patientSub?.unsubscribe();
    this.medicineRowStates.forEach(state => state.subscription.unsubscribe());
  }

  private setupPatientLookup(): void {
    this.patientSub = this.patientSearch$.pipe(
      debounceTime(300),
      map(term => (term ?? '').trim()),
      distinctUntilChanged(),
      switchMap(term => {
        if (term.length < 2) {
          this.patientResults.set([]);
          this.patientLoading.set(false);
          return EMPTY;
        }
        this.patientLoading.set(true);
        return this.#patientService.lookupPatients(term).pipe(
          catchError(() => {
            this.patientLoading.set(false);
            return of([]);
          })
        );
      })
    ).subscribe(newResults => {
      this.patientLoading.set(false);
      this.preserveSelectedPatient(newResults);
    });
  }

  private preserveSelectedPatient(newResults: PatientLookupDto[]): void {
    const currentId = this.prescriptionForm.get('patientId')?.value;
    if (!currentId) {
      this.patientResults.set(newResults);
      return;
    }
    const currentSelected = this.patientResults().find(p => p.id === currentId);
    if (currentSelected && !newResults.some(p => p.id === currentId)) {
      this.patientResults.set([currentSelected, ...newResults]);
    } else {
      this.patientResults.set(newResults);
    }
  }

  onPatientFilter(event: any): void {
    const term = typeof event?.filter === 'string' ? event.filter : (event?.target?.value ?? '');
    this.patientSearch$.next(term);
  }

  onMedicineFilter(index: number, event: any): void {
    const term = typeof event?.filter === 'string' ? event.filter : (event?.target?.value ?? '');
    this.medicineRowStates[index]?.search$.next(term);
  }

  initForm(): void {
    const today = new Date();
    const defaultValidFrom = today.toISOString().split('T')[0];
    const defaultValidTo = new Date(today.setMonth(today.getMonth() + 1)).toISOString().split('T')[0];

    this.prescriptionForm = this.#fb.group({
      patientId: ['', [Validators.required]],
      validFrom: [defaultValidFrom, [Validators.required]],
      validTo: [defaultValidTo, [Validators.required]],
      notes: ['', [Validators.maxLength(500)]],
      items: this.#fb.array([], [Validators.required, Validators.minLength(1), this.uniqueMedicineValidator])
    }, { validators: this.dateRangeValidator });

    // Add one empty item by default
    this.addItem();
  }

  uniqueMedicineValidator(control: import('@angular/forms').AbstractControl) {
    const arr = control as FormArray;
    if (!arr || !arr.controls) return null;
    const medicineIds = arr.controls.map(ctrl => ctrl.get('medicineId')?.value).filter(id => id);
    const hasDuplicates = new Set(medicineIds).size !== medicineIds.length;
    if (hasDuplicates) {
      return { duplicateMedicine: true };
    }
    return null;
  }

  dateRangeValidator(control: import('@angular/forms').AbstractControl) {
    const group = control as FormGroup;
    const from = group.get('validFrom')?.value;
    const to = group.get('validTo')?.value;
    if (from && to && new Date(to) < new Date(from)) {
      return { invalidDateRange: true };
    }
    return null;
  }

  get items(): FormArray {
    return this.prescriptionForm.get('items') as FormArray;
  }

  addItem(): void {
    const search$ = new Subject<string>();
    const results = signal<MedicineLookupDto[]>([]);
    const loading = signal<boolean>(false);

    const rowGroup = this.#fb.group({
      medicineId: ['', [Validators.required]],
      quantityPrescribed: [1, [Validators.required, Validators.min(1)]],
      maxFillCount: [1, [Validators.required, Validators.min(1)]],
      dosageInstructions: ['', [Validators.maxLength(500)]]
    });

    const subscription = search$.pipe(
      debounceTime(300),
      map(term => (term ?? '').trim()),
      distinctUntilChanged(),
      switchMap(term => {
        if (term.length < 2) {
          results.set([]);
          loading.set(false);
          return EMPTY;
        }
        loading.set(true);
        return this.#medicineService.lookupMedicines(term).pipe(
          catchError(() => {
            loading.set(false);
            return of([]);
          })
        );
      })
    ).subscribe(newResults => {
      loading.set(false);
      this.preserveSelectedMedicine(rowGroup, results, newResults);
    });

    this.medicineRowStates.push({ search$, results, loading, subscription });
    this.items.push(rowGroup);
  }

  private preserveSelectedMedicine(
    rowGroup: FormGroup,
    resultsSignal: WritableSignal<MedicineLookupDto[]>,
    newResults: MedicineLookupDto[]
  ): void {
    const currentId = rowGroup.get('medicineId')?.value;
    if (!currentId) {
      resultsSignal.set(newResults);
      return;
    }
    const currentSelected = resultsSignal().find(m => m.id === currentId);
    if (currentSelected && !newResults.some(m => m.id === currentId)) {
      resultsSignal.set([currentSelected, ...newResults]);
    } else {
      resultsSignal.set(newResults);
    }
  }

  removeItem(index: number): void {
    if (this.items.length > 1) {
      const state = this.medicineRowStates[index];
      if (state) {
        state.subscription.unsubscribe();
        this.medicineRowStates.splice(index, 1);
      }
      this.items.removeAt(index);
    }
  }

  onSubmit(): void {
    if (this.prescriptionForm.invalid) {
      this.prescriptionForm.markAllAsTouched();
      // Also touch all items
      this.items.controls.forEach(control => {
        if (control instanceof FormGroup) {
          control.markAllAsTouched();
        }
      });
      return;
    }
    this.save.emit(this.prescriptionForm.value);
  }
}
