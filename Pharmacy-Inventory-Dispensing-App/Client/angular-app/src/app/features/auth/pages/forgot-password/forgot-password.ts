import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/auth/auth.service';
import { ThemeService } from '../../../../core/services/theme.service';
import { extractApiErrorMessage } from '../../../../core/models/api-error.model';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    ButtonModule,
    InputTextModule,
    ToastModule
  ],
  templateUrl: './forgot-password.html',
  styleUrls: ['../login/login.component.css'],
  providers: [MessageService]
})
export class ForgotPasswordComponent implements OnInit {
  readonly #fb = inject(FormBuilder);
  readonly #authService = inject(AuthService);
  readonly themeService = inject(ThemeService);
  readonly #router = inject(Router);
  readonly #messageService = inject(MessageService);

  forgotForm!: FormGroup;
  isSubmitting = false;
  successMessage = false;

  ngOnInit(): void {
    this.forgotForm = this.#fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit(): void {
    if (this.forgotForm.invalid) {
      this.forgotForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const email = this.forgotForm.value.email;

    this.#authService.forgotPassword(email).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.successMessage = true;
        this.#messageService.add({
          severity: 'success',
          summary: 'Email Sent',
          detail: 'If an account exists, a reset link has been sent.'
        });
      },
      error: (err) => {
        this.isSubmitting = false;
        this.#messageService.add({
          severity: 'error',
          summary: 'Failed',
          detail: extractApiErrorMessage(err) || 'Something went wrong.'
        });
      }
    });
  }
}
