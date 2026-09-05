import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/auth/auth.service';
import { ThemeService } from '../../../../core/services/theme.service';
import { extractApiErrorMessage } from '../../../../core/models/api-error.model';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    ButtonModule,
    InputTextModule,
    ToastModule
  ],
  templateUrl: './reset-password.html',
  styleUrls: ['../login/login.component.css'],
  providers: [MessageService]
})
export class ResetPasswordComponent implements OnInit {
  readonly #fb = inject(FormBuilder);
  readonly #authService = inject(AuthService);
  readonly themeService = inject(ThemeService);
  readonly #router = inject(Router);
  readonly #route = inject(ActivatedRoute);
  readonly #messageService = inject(MessageService);

  resetForm!: FormGroup;
  isSubmitting = false;
  successMessage = false;

  email = '';
  token = '';
  invalidLink = false;

  ngOnInit(): void {
    // Read query params from URL
    this.#route.queryParams.subscribe(params => {
      this.email = params['email'] || '';
      this.token = params['token'] || '';

      if (!this.email || !this.token) {
        this.invalidLink = true;
      }
    });

    this.resetForm = this.#fb.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('newPassword')?.value === g.get('confirmPassword')?.value
      ? null : { mismatch: true };
  }

  onSubmit(): void {
    if (this.resetForm.invalid || this.invalidLink) {
      this.resetForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    const payload = {
      email: this.email,
      token: this.token,
      newPassword: this.resetForm.value.newPassword
    };

    this.#authService.resetPassword(payload).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.successMessage = true;
        this.#messageService.add({
          severity: 'success',
          summary: 'Password Reset',
          detail: 'Your password has been successfully reset.'
        });
        
        // Auto redirect to login after 2 seconds
        setTimeout(() => {
          this.#router.navigate(['/login']);
        }, 2000);
      },
      error: (err) => {
        this.isSubmitting = false;
        this.#messageService.add({
          severity: 'error',
          summary: 'Failed',
          detail: extractApiErrorMessage(err) || 'Failed to reset password. The link may have expired.'
        });
      }
    });
  }
}
