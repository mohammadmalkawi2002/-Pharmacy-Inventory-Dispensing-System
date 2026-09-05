import { Component, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Medicine, CreateMedicineDto, MEDICINE_FORM_OPTIONS, STOCK_UNIT_OPTIONS, PACKAGE_UNIT_OPTIONS } from '../../models/medicine.models';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';

@Component({
  selector: 'app-medicine-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    ButtonModule,
    InputTextModule,
    InputNumberModule,
    SelectModule
  ],
  templateUrl: './medicine-form.component.html',
  styleUrls: ['./medicine-form.component.css']
})
export class MedicineFormComponent implements OnInit {
  readonly #fb = inject(FormBuilder);

  @Input() medicine?: Medicine;
  @Input() isSubmitting = false;
  @Output() save = new EventEmitter<CreateMedicineDto>();
  @Output() cancel = new EventEmitter<void>();

  medicineForm!: FormGroup;
  readonly formOptions = MEDICINE_FORM_OPTIONS;
  readonly stockUnitOptions = STOCK_UNIT_OPTIONS;
  readonly packageUnitOptions = PACKAGE_UNIT_OPTIONS;

  ngOnInit(): void {
    this.initForm();
  }

  initForm(): void {
    this.medicineForm = this.#fb.group({
      code: [this.medicine?.code || '', [Validators.required, Validators.maxLength(15), Validators.pattern(/^\d+$/)]],
      name: [this.medicine?.name || '', [Validators.required, Validators.maxLength(100)]],
      strength: [this.medicine?.strength || '', [Validators.required, Validators.maxLength(50)]],
      form: [this.medicine?.form || 'Tablet', [Validators.required]],
      stockUnit: [this.medicine?.stockUnit || 'Tablet', [Validators.required]],
      packageUnit: [this.medicine?.packageUnit || 'Box', [Validators.required]],
      unitsPerPackage: [this.medicine?.unitsPerPackage ?? 30, [Validators.required, Validators.min(1)]],
      reorderLevel: [this.medicine?.reorderLevel ?? 20, [Validators.required, Validators.min(0)]]
    });
  }

  onSubmit(): void {
    if (this.medicineForm.invalid) {
      this.medicineForm.markAllAsTouched();
      return;
    }
    this.save.emit(this.medicineForm.value);
  }
}
