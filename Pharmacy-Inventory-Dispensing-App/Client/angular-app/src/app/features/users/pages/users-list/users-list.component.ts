import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../services/user.service';
import { SystemUser, CreateUserDto, UpdateUserDto } from '../../models/user.models';
import { Permissions } from '../../../../core/auth/auth.models';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { SelectModule } from 'primeng/select';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import { LoadingComponent } from '../../../../shared/components/loading/loading.component';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { environment } from '../../../../../environments/environment';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    DialogModule,
    TagModule,
    TooltipModule,
    SelectModule,
    LoadingComponent,
    EmptyStateComponent,
    HasPermissionDirective,
    DatePipe
  ],
  templateUrl: './users-list.component.html',
  styleUrls: ['./users-list.component.css']
})
export class UsersListComponent implements OnInit {
  readonly #userService = inject(UserService);
  readonly #confirmationService = inject(ConfirmationService);
  readonly #messageService = inject(MessageService);
  readonly #router = inject(Router);
  readonly Permissions = Permissions;

  users = signal<SystemUser[]>([]);
  totalRecords = signal(0);
  isLoading = signal(true);
  isSubmitting = signal(false);

  searchQuery = '';
  selectedRole = '';
  roleOptions = [
    { label: 'All Roles', value: '' },
    { label: 'Pharmacist', value: 'Pharmacist' },
    { label: 'Doctor', value: 'Doctor' },
    { label: 'Receptionist', value: 'Receptionist' }
  ];
  currentPage = 1;
  pageSize = environment.defaultPageSize;

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading.set(true);
    this.#userService.getUsers({
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      searchTerm: this.searchQuery || undefined,
      role: this.selectedRole || undefined
    }).subscribe({
      next: (res) => {
        this.users.set(res.items);
        this.totalRecords.set(res.totalCount);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadUsers();
  }

  onRoleChange(): void {
    this.currentPage = 1;
    this.loadUsers();
  }

  onPageChange(event: any): void {
    const rows = event.rows ?? this.pageSize;
    const first = event.first ?? 0;
    this.currentPage = Math.floor(first / rows) + 1;
    this.pageSize = rows;
    this.loadUsers();
  }

  getRoleSeverity(role: string): 'success' | 'info' | 'warn' | 'danger' | 'secondary' {
    switch (role) {
      case 'Pharmacist': return 'success';
      case 'Doctor': return 'info';
      case 'Receptionist': return 'warn';
      default: return 'secondary';
    }
  }

  openCreateDialog(): void {
    this.#router.navigate(['/users/create']);
  }

  openEditDialog(user: SystemUser): void {
    this.#router.navigate(['/users', user.id, 'edit'], { state: { data: user } });
  }

  toggleActivation(user: SystemUser): void {
    const action = user.isActive ? 'Deactivate' : 'Activate';
    const fullName = `${user.firstName} ${user.lastName}`;
    const iconClass = user.isActive ? 'pi pi-lock text-red-500' : 'pi pi-unlock text-green-500';

    this.#confirmationService.confirm({
      header: `${action} User`,
      message: `Are you sure you want to ${action.toLowerCase()} user "${fullName}"?`,
      icon: iconClass,
      acceptLabel: action,
      rejectLabel: 'Cancel',
      acceptButtonStyleClass: user.isActive ? 'p-button-danger' : 'p-button-success',
      accept: () => {
        const obs = user.isActive 
          ? this.#userService.deactivateUser(user.id) 
          : this.#userService.activateUser(user.id);
        
        obs.subscribe({
          next: () => {
            this.#messageService.add({ severity: 'success', summary: `User ${action}d`, detail: `User successfully ${action.toLowerCase()}d.`, life: 3000 });
            this.loadUsers();
          }
        });
      }
    });
  }
}
