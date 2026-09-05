import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/auth/auth.service';
import { AuthStore } from '../../../../core/auth/auth.store';
import { ThemeService } from '../../../../core/services/theme.service';
import { extractApiErrorMessage } from '../../../../core/models/api-error.model';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterModule,
    ButtonModule,
    InputTextModule,
    ToastModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  providers: [MessageService]
})
export class LoginComponent implements OnInit {
  readonly #fb = inject(FormBuilder);
  readonly #authService = inject(AuthService);
  readonly authStore = inject(AuthStore);
  readonly themeService = inject(ThemeService);
  readonly #route = inject(ActivatedRoute);
  readonly #messageService = inject(MessageService);

  loginForm!: FormGroup;
  isSubmitting = false;

  ngOnInit(): void {
    this.loginForm = this.#fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    if (this.#route.snapshot.queryParams['sessionExpired']) {
      this.#messageService.add({
        severity: 'warn',
        summary: 'Session Expired',
        detail: 'Your session has expired. Please sign in again.',
        life: 5000
      });
    }
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.#authService.login(this.loginForm.value).subscribe({
      next: (res) => {
        this.isSubmitting = false;
      },
      error: (err) => {
        this.isSubmitting = false;
        const errorMessage = extractApiErrorMessage(err);
        this.#messageService.add({
          severity: 'error',
          summary: 'Login Failed',
          detail: errorMessage,
          life: 5000
        });
      }
    });
  }
}
