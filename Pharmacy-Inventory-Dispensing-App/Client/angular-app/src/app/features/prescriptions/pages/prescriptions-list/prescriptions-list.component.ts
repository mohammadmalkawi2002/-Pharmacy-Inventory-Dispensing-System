import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PrescriptionService } from '../../services/prescription.service';
import { Prescription, getPrescriptionStatusSeverity, CreatePrescriptionDto, PrescriptionStatus } from '../../models/prescription.models';
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
  selector: 'app-prescriptions-list',
  standalone: true,
  imports: [
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    TagModule,
    TooltipModule,
    SelectModule,
    LoadingComponent,
    EmptyStateComponent,
    HasPermissionDirective,
    DatePipe
  ],
  templateUrl: './prescriptions-list.component.html',
  styleUrls: ['./prescriptions-list.component.css']
})
export class PrescriptionsListComponent implements OnInit {
  readonly #prescriptionService = inject(PrescriptionService);
  readonly #confirmationService = inject(ConfirmationService);
  readonly #messageService = inject(MessageService);
  readonly #router = inject(Router);
  readonly Permissions = Permissions;

  prescriptions = signal<Prescription[]>([]);
  totalRecords = signal(0);
  isLoading = signal(true);
  isSubmitting = signal(false);

  searchQuery = '';
  status: string | undefined = undefined;
  sortField: string | undefined = 'createdAtUtc';
  isDescending: boolean | undefined = true;

  statusOptions = [
    { label: 'All', value: undefined },
    { label: 'Active', value: 'Active' },
    { label: 'Cancelled', value: 'Cancelled' },
    { label: 'Expired', value: 'Expired' }
  ];

  currentPage = 1;
  pageSize = environment.defaultPageSize;

  ngOnInit(): void {
    this.loadPrescriptions();
  }

  loadPrescriptions(): void {
    this.isLoading.set(true);
    this.#prescriptionService.getPrescriptions({
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      searchTerm: this.searchQuery?.trim() || undefined,
      status: this.status,
      sortBy: this.sortField,
      isDescending: this.isDescending
    }).subscribe({
      next: (res) => {
        this.prescriptions.set(res.items);
        this.totalRecords.set(res.totalCount);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadPrescriptions();
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.loadPrescriptions();
  }

  onPageChange(event: any): void {
    const rows = event.rows ?? this.pageSize;
    const first = event.first ?? 0;

    const newSortField = event.sortField;
    const newIsDescending = newSortField ? (event.sortOrder === -1) : true;

    if (this.sortField !== newSortField || this.isDescending !== newIsDescending) {
      this.currentPage = 1;
      this.sortField = newSortField;
      this.isDescending = newIsDescending;
    } else {
      this.currentPage = Math.floor(first / rows) + 1;
    }

    this.pageSize = rows;
    this.loadPrescriptions();
  }

  getStatusSeverity(status: PrescriptionStatus): 'success' | 'danger' | 'warn' | 'secondary' {
    return getPrescriptionStatusSeverity(status);
  }

  openCreateDialog(): void {
    this.#router.navigate(['/prescriptions/create']);
  }

  openViewDialog(prescription: Prescription): void {
    this.#router.navigate(['/prescriptions', prescription.id], { state: { data: prescription } });
  }

  confirmCancel(prescription: Prescription): void {
    this.#confirmationService.confirm({
      header: 'Cancel Prescription',
      message: `Are you sure you want to cancel prescription ${prescription.prescriptionNumber}?`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Yes, Cancel',
      rejectLabel: 'No, Keep it',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.#prescriptionService.cancelPrescription(prescription.id).subscribe({
          next: () => {
            this.#messageService.add({
              severity: 'success',
              summary: 'Cancelled',
              detail: 'Prescription has been cancelled.',
              life: 3000
            });
            this.loadPrescriptions();
          }
        });
      }
    });
  }
}
