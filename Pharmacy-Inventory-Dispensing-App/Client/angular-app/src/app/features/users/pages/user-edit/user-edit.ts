import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MessageService } from 'primeng/api';
import { UserService } from '../../services/user.service';
import { SystemUser, UpdateUserDto, CreateUserDto } from '../../models/user.models';
import { UserFormComponent } from '../../components/user-form/user-form.component';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { LoadingComponent } from '../../../../shared/components/loading/loading.component';

@Component({
  selector: 'app-user-edit',
  standalone: true,
  imports: [UserFormComponent, EmptyStateComponent, LoadingComponent],
  templateUrl: './user-edit.html',
  styleUrl: './user-edit.css'
})
export class UserEdit implements OnInit {
  readonly #userService = inject(UserService);
  readonly #router = inject(Router);
  readonly #route = inject(ActivatedRoute);
  readonly #messageService = inject(MessageService);

  isSubmitting = signal(false);
  isLoading = signal(true);
  user = signal<SystemUser | undefined>(undefined);
  userId = signal<string | null>(null);

  ngOnInit(): void {
    const id = this.#route.snapshot.paramMap.get('id');
    this.userId.set(id);
    
    if (id) {
      this.#userService.getUser(id).subscribe({
        next: (data) => {
          this.user.set(data);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
          this.#messageService.add({ severity: 'error', summary: 'Error', detail: 'Failed to fetch user details.', life: 3000 });
        }
      });
    } else {
      this.isLoading.set(false);
    }
  }

  onSave(dto: CreateUserDto): void {
    if (!this.userId()) return;
    
    const payload: UpdateUserDto = {
      firstName: dto.firstName,
      lastName: dto.lastName,
      email: dto.email,
      role: dto.role
    };

    this.isSubmitting.set(true);
    this.#userService.updateUser(this.userId()!, payload).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.#messageService.add({ severity: 'success', summary: 'User Updated', detail: 'User updated successfully.', life: 3000 });
        this.#router.navigate(['/users']);
      },
      error: () => this.isSubmitting.set(false)
    });
  }

  onCancel(): void {
    this.#router.navigate(['/users']);
  }
}
