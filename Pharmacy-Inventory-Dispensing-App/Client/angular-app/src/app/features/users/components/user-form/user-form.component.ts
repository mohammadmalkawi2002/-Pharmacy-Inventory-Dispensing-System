import { Component, EventEmitter, Input, OnInit, Output, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { SystemUser, CreateUserDto } from '../../models/user.models';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { PasswordModule } from 'primeng/password';
import { UserRole } from '../../../../core/auth/auth.models';

@Component({
  selector: 'app-user-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    ButtonModule,
    InputTextModule,
    SelectModule,
    PasswordModule
  ],
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.css']
})
export class UserFormComponent implements OnInit {
  readonly #fb = inject(FormBuilder);

  @Input() user?: SystemUser;
  @Input() isSubmitting = false;
  @Output() save = new EventEmitter<CreateUserDto>();
  @Output() cancel = new EventEmitter<void>();

  userForm!: FormGroup;

  readonly roleOptions: { label: string; value: UserRole }[] = [
    { label: 'Pharmacist', value: 'Pharmacist' },
    { label: 'Doctor', value: 'Doctor' },
    { label: 'Receptionist', value: 'Receptionist' }
  ];

  ngOnInit(): void {
    this.initForm();
  }

  initForm(): void {
    this.userForm = this.#fb.group({
      firstName: [this.user?.firstName || '', [Validators.required, Validators.minLength(2)]],
      lastName: [this.user?.lastName || '', [Validators.required, Validators.minLength(2)]],
      email: [this.user?.email || '', [Validators.required, Validators.email]],
      role: [this.user?.role || 'Pharmacist', [Validators.required]],
      password: ['', this.user ? [] : [Validators.required, Validators.minLength(8)]]
    });
  }

  onSubmit(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }
    this.save.emit(this.userForm.value);
  }
}
