import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { UserService } from '../../services/user.service';
import { CreateUserDto } from '../../models/user.models';
import { UserFormComponent } from '../../components/user-form/user-form.component';

@Component({
  selector: 'app-user-create',
  standalone: true,
  imports: [UserFormComponent],
  templateUrl: './user-create.html',
  styleUrl: './user-create.css'
})
export class UserCreate {
  readonly #userService = inject(UserService);
  readonly #router = inject(Router);
  readonly #messageService = inject(MessageService);

  isSubmitting = signal(false);

  onSave(dto: CreateUserDto): void {
    this.isSubmitting.set(true);
    this.#userService.createUser(dto).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.#messageService.add({ severity: 'success', summary: 'User Created', detail: 'User created successfully.', life: 3000 });
        this.#router.navigate(['/users']);
      },
      error: () => this.isSubmitting.set(false)
    });
  }

  onCancel(): void {
    this.#router.navigate(['/users']);
  }
}
