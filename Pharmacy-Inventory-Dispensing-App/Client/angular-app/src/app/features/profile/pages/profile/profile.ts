import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../core/auth/auth.service';
import { UserProfile, UserRole } from '../../../../core/auth/auth.models';
import { CardModule } from 'primeng/card';
import { TagModule } from 'primeng/tag';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ButtonModule } from 'primeng/button';
import { DividerModule } from 'primeng/divider';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { DialogModule } from 'primeng/dialog';
import { PasswordModule } from 'primeng/password';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule, 
    CardModule, 
    TagModule, 
    ProgressSpinnerModule, 
    ButtonModule, 
    DividerModule,
    ReactiveFormsModule,
    DialogModule,
    PasswordModule,
    InputTextModule
  ],
  styleUrl: './profile.css',
  templateUrl: './profile.html',
})
export class ProfileComponent implements OnInit {
  readonly authService = inject(AuthService);
  readonly cdr = inject(ChangeDetectorRef);
  readonly fb = inject(FormBuilder);
  readonly messageService = inject(MessageService);
  
  profile: UserProfile | null = null;
  loading = true;
  error = false;

  // Change Password State
  showChangePasswordDialog = false;
  changePasswordForm!: FormGroup;
  isSubmitting = false;

  ngOnInit() {
    this.initForm();
    console.log('Fetching profile...');
    this.authService.getMe().pipe(
      catchError((err) => {
        console.error('Error fetching profile:', err);
        this.error = true;
        this.loading = false;
        this.cdr.detectChanges();
        return of(null);
      })
    ).subscribe((data) => {
      console.log('Profile data received:', data);
      if (data) {
        // Handle wrapper object if present (e.g., { data: { ... } })
        this.profile = (data as any).data || (data as any).value || data;
      }
      this.loading = false;
      this.cdr.detectChanges();
    });
  }

  getRoleSeverity(role: string): 'success' | 'info' | 'warn' | 'danger' | 'secondary' | 'contrast' {
    switch (role as UserRole) {
      case 'Admin': return 'danger';
      case 'Doctor': return 'info';
      case 'Pharmacist': return 'success';
      case 'Receptionist': return 'warn';
      default: return 'secondary';
    }
  }

  getPermissionCategory(permission: string): string {
    const parts = permission.split('.');
    if (parts.length >= 2) {
      return parts[1];
    }
    return 'Other';
  }

  getGroupedPermissions() {
    if (!this.profile || !this.profile.permissions) return {};
    
    const groups: { [key: string]: string[] } = {};
    this.profile.permissions.forEach(p => {
      const category = this.getPermissionCategory(p);
      if (!groups[category]) {
        groups[category] = [];
      }
      const parts = p.split('.');
      groups[category].push(parts.length > 2 ? parts.slice(2).join('.') : p);
    });
    return groups;
  }

  // Change Password Logic
  initForm(): void {
    this.changePasswordForm = this.fb.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('newPassword')?.value === g.get('confirmPassword')?.value
      ? null : { mismatch: true };
  }

  openChangePassword(): void {
    this.changePasswordForm.reset();
    this.showChangePasswordDialog = true;
  }

  onChangePasswordSubmit(): void {
    if (this.changePasswordForm.invalid) {
      this.changePasswordForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const { currentPassword, newPassword } = this.changePasswordForm.value;

    this.authService.changePassword({ currentPassword, newPassword }).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Success',
          detail: 'Password changed successfully.',
          life: 3000
        });
        this.showChangePasswordDialog = false;
        this.isSubmitting = false;
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: err?.error?.message || 'Failed to change password. Please check your current password.',
          life: 4000
        });
        this.isSubmitting = false;
        this.cdr.detectChanges();
      }
    });
  }
}
