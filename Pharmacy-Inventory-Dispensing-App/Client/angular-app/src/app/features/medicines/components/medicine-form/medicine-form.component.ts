import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, inject, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Medicine, CreateMedicineDto, MEDICINE_FORM_OPTIONS, STOCK_UNIT_OPTIONS, PACKAGE_UNIT_OPTIONS } from '../../models/medicine.models';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';

const ACCEPTED_EXTENSIONS = ['.jpg', '.jpeg', '.png', '.webp'];
const MAX_SIZE_BYTES = 5 * 1024 * 1024; // 5 MB

export interface MedicineFormSaveEvent {
  dto: CreateMedicineDto;
  image: File | null;
}

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
export class MedicineFormComponent implements OnInit, OnDestroy {
  readonly #fb = inject(FormBuilder);

  @Input() medicine?: Medicine;
  @Input() isSubmitting = false;
  /** True when editing a medicine that already has a stored image. */
  @Input() hasImage = false;
  @Output() save = new EventEmitter<MedicineFormSaveEvent>();
  @Output() cancel = new EventEmitter<void>();

  medicineForm!: FormGroup;
  readonly formOptions = MEDICINE_FORM_OPTIONS;
  readonly stockUnitOptions = STOCK_UNIT_OPTIONS;
  readonly packageUnitOptions = PACKAGE_UNIT_OPTIONS;

  selectedFile = signal<File | null>(null);
  previewUrl = signal<string | null>(null);
  imageError = signal<string | null>(null);

  ngOnInit(): void {
    this.initForm();
  }

  ngOnDestroy(): void {
    this.#revokePreview();
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

  onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;

    this.imageError.set(null);
    this.#revokePreview();

    if (!file) {
      this.selectedFile.set(null);
      this.previewUrl.set(null);
      return;
    }

    const extension = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();
    if (!ACCEPTED_EXTENSIONS.includes(extension)) {
      this.imageError.set('Unsupported file type. Please upload a JPG, PNG, or WebP image.');
      this.selectedFile.set(null);
      input.value = '';
      return;
    }

    if (file.size > MAX_SIZE_BYTES) {
      this.imageError.set('File is too large. Maximum allowed size is 5 MB.');
      this.selectedFile.set(null);
      input.value = '';
      return;
    }

    this.selectedFile.set(file);
    this.previewUrl.set(URL.createObjectURL(file));
  }

  onSubmit(): void {
    if (this.medicineForm.invalid) {
      this.medicineForm.markAllAsTouched();
      return;
    }
    this.save.emit({ dto: this.medicineForm.value, image: this.selectedFile() });
  }

  #revokePreview(): void {
    const url = this.previewUrl();
    if (url) {
      URL.revokeObjectURL(url);
      this.previewUrl.set(null);
    }
  }
}
