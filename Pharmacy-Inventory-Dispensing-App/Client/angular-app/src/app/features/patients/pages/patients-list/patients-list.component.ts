import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PatientService } from '../../services/patient.service';
import { Patient, CreatePatientDto, UpdatePatientDto } from '../../models/patient.models';
import { Permissions } from '../../../../core/auth/auth.models';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { TabsModule } from 'primeng/tabs';
import { SelectModule } from 'primeng/select';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import { LoadingComponent } from '../../../../shared/components/loading/loading.component';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { environment } from '../../../../../environments/environment';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-patients-list',
  standalone: true,
  imports: [
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    DialogModule,
    TagModule,
    TooltipModule,
    TabsModule,
    SelectModule,
    LoadingComponent,
    EmptyStateComponent,
    HasPermissionDirective,
    DatePipe
  ],
  templateUrl: './patients-list.component.html',
  styleUrls: ['./patients-list.component.css']
})
export class PatientsListComponent implements OnInit {
  readonly #patientService = inject(PatientService);
  readonly #confirmationService = inject(ConfirmationService);
  readonly #messageService = inject(MessageService);
  readonly #router = inject(Router);
  readonly Permissions = Permissions;

  patients = signal<Patient[]>([]);
  totalRecords = signal(0);
  isLoading = signal(true);
  isSubmitting = signal(false);

  searchQuery = '';
  documentType: string | undefined = undefined;
  sortField: string | undefined = undefined;
  isDescending: boolean | undefined = true;

  documentTypeOptions = [
    { label: 'All', value: undefined },
    { label: 'Citizen', value: 'Citizen' },
    { label: 'Resident', value: 'Resident' }
  ];

  currentPage = 1;
  pageSize = environment.defaultPageSize;
  activeTab: 'active' | 'archived' = 'active';

  ngOnInit(): void {
    this.loadPatients();
  }

  loadPatients(): void {
    this.isLoading.set(true);
    
    const params = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      searchTerm: this.searchQuery?.trim() || undefined,
      documentType: this.documentType,
      sortBy: this.sortField,
      isDescending: this.isDescending
    };

    const loader = this.activeTab === 'archived'
      ? this.#patientService.getArchivedPatients(params)
      : this.#patientService.getPatients(params);

    loader.subscribe({
      next: (res) => {
        this.patients.set(res.items);
        this.totalRecords.set(res.totalCount);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.loadPatients();
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
    this.loadPatients();
  }

  switchTab(tab: 'active' | 'archived'): void {
    this.activeTab = tab;
    this.currentPage = 1;
    this.searchQuery = '';
    this.loadPatients();
  }

  openCreateDialog(): void {
    this.#router.navigate(['/patients/create']);
  }

  openEditDialog(patient: Patient): void {
    this.#router.navigate(['/patients', patient.id, 'edit'], { state: { data: patient } });
  }

  confirmArchive(patient: Patient): void {
    this.#confirmationService.confirm({
      header: 'Archive Patient',
      message: `Are you sure you want to archive the patient "${patient.fullName}"?`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Archive',
      rejectLabel: 'Cancel',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.#patientService.archivePatient(patient.id).subscribe({
          next: () => {
            this.#messageService.add({
              severity: 'success',
              summary: 'Patient Archived',
              detail: 'Patient has been archived successfully.',
              life: 3000
            });
            this.loadPatients();
          }
        });
      }
    });
  }

  restorePatient(patient: Patient): void {
    this.#confirmationService.confirm({
      header: 'Restore Patient',
      message: `Are you sure you want to restore the patient "${patient.fullName}"?`,
      icon: 'pi pi-refresh',
      acceptLabel: 'Restore',
      rejectLabel: 'Cancel',
      accept: () => {
        this.#patientService.restorePatient(patient.id).subscribe({
          next: () => {
            this.#messageService.add({
              severity: 'success',
              summary: 'Patient Restored',
              detail: 'Patient has been restored successfully.',
              life: 3000
            });
            this.loadPatients();
          }
        });
      }
    });
  }
}
