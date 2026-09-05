import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MedicineService } from '../../services/medicine.service';
import { Medicine, CreateMedicineDto, UpdateMedicineDto, getStockSeverity, StockStatus, MEDICINE_FORM_OPTIONS, STOCK_UNIT_OPTIONS, MedicineForm, StockUnit } from '../../models/medicine.models';
import { Permissions } from '../../../../core/auth/auth.models';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { InputNumberModule } from 'primeng/inputnumber';
import { SelectModule } from 'primeng/select';
import { MenuModule } from 'primeng/menu';
import { DrawerModule } from 'primeng/drawer';
import { ConfirmationService, MessageService, MenuItem } from 'primeng/api';
import { Router } from '@angular/router';
import { AuthStore } from '../../../../core/auth/auth.store';
import { LoadingComponent } from '../../../../shared/components/loading/loading.component';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { environment } from '../../../../../environments/environment';

import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-medicines-list',
  standalone: true,
  imports: [
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    DialogModule,
    TagModule,
    TooltipModule,
    InputNumberModule,
    SelectModule,
    MenuModule,
    DrawerModule,
    DatePipe,
    LoadingComponent,
    EmptyStateComponent,
    HasPermissionDirective
  ],
  templateUrl: './medicines-list.component.html',
  styleUrls: ['./medicines-list.component.css']
})
export class MedicinesListComponent implements OnInit {
  readonly #medicineService = inject(MedicineService);
  readonly #confirmationService = inject(ConfirmationService);
  readonly #messageService = inject(MessageService);
  readonly #router = inject(Router);
  readonly #authStore = inject(AuthStore);
  readonly Permissions = Permissions;

  medicines = signal<Medicine[]>([]);
  totalRecords = signal(0);
  isLoading = signal(true);
  isSubmitting = signal(false);

  searchQuery = '';
  formFilter?: MedicineForm;
  stockUnitFilter?: StockUnit;
  isActiveFilter?: boolean;

  formOptions = MEDICINE_FORM_OPTIONS;
  stockUnitOptions = STOCK_UNIT_OPTIONS;
  statusOptions = [
    { label: 'Active', value: true },
    { label: 'Inactive', value: false }
  ];

  currentPage = 1;
  pageSize = environment.defaultPageSize;
  sortBy = 'CreatedAtUtc';
  sortOrder: 'asc' | 'desc' = 'desc';
  activeTab: 'all' | 'low-stock' | 'archived' = 'all';

  // Details & Actions State
  actionMenuItems = signal<MenuItem[]>([]);
  selectedMedicine = signal<Medicine | null>(null);

  ngOnInit(): void {
    this.loadMedicines();
  }

  loadMedicines(): void {
    this.isLoading.set(true);

    const isArchived = this.activeTab === 'archived';

    const params: any = {
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      searchTerm: this.searchQuery || undefined
    };

    if (!isArchived) {
      params.form = this.formFilter || undefined;
      params.stockUnit = this.stockUnitFilter || undefined;
      params.isActive = this.isActiveFilter !== undefined ? this.isActiveFilter : undefined;
      params.sortBy = this.sortBy;
      params.sortOrder = this.sortOrder;
    }

    let loader = this.#medicineService.getMedicines(params);
    if (isArchived) {
      loader = this.#medicineService.getArchivedMedicines(params);
    } else if (this.activeTab === 'low-stock') {
      loader = this.#medicineService.getLowStockMedicines(params);
    }

    loader.subscribe({
      next: (res) => {
        this.medicines.set(res.items);
        this.totalRecords.set(res.totalCount);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadMedicines();
  }

  onPageChange(event: any): void {
    const rows = event.rows ?? this.pageSize;
    const first = event.first ?? 0;
    this.currentPage = Math.floor(first / rows) + 1;
    this.pageSize = rows;

    if (event.sortField) {
      this.sortBy = event.sortField;
      this.sortOrder = event.sortOrder === 1 ? 'asc' : 'desc';
    }

    this.loadMedicines();
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.loadMedicines();
  }

  clearFilters(): void {
    this.searchQuery = '';
    this.formFilter = undefined;
    this.stockUnitFilter = undefined;
    this.isActiveFilter = undefined;
    this.onFilterChange();
  }

  switchTab(tab: 'all' | 'low-stock' | 'archived'): void {
    this.activeTab = tab;
    this.currentPage = 1;
    this.searchQuery = '';
    this.loadMedicines();
  }

  getStockSeverity(status: StockStatus | string): 'success' | 'warn' | 'danger' {
    return getStockSeverity(status as StockStatus);
  }

  openCreateDialog(): void {
    this.#router.navigate(['/medicines/create']);
  }

  openEditDialog(medicine: Medicine): void {
    this.#router.navigate(['/medicines', medicine.id, 'edit'], { state: { data: medicine } });
  }

  // Activate / Deactivate
  toggleActivation(medicine: Medicine): void {
    const action = medicine.isActive ? 'Deactivate' : 'Activate';
    this.#confirmationService.confirm({
      header: `${action} Medicine`,
      message: `Are you sure you want to ${action.toLowerCase()} "${medicine.name}"?`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: action,
      rejectLabel: 'Cancel',
      acceptButtonStyleClass: medicine.isActive ? 'p-button-danger' : 'p-button-success',
      accept: () => {
        const obs = medicine.isActive
          ? this.#medicineService.deactivateMedicine(medicine.id)
          : this.#medicineService.activateMedicine(medicine.id);
        obs.subscribe({
          next: () => {
            this.#messageService.add({ severity: 'success', summary: `Medicine ${action}d`, detail: `Medicine has been ${action.toLowerCase()}d successfully.`, life: 3000 });
            this.loadMedicines();
          }
        });
      }
    });
  }

  // Archive / Restore
  confirmArchive(medicine: Medicine): void {
    this.#confirmationService.confirm({
      header: 'Archive Medicine',
      message: `Are you sure you want to archive "${medicine.name}"?`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Archive',
      rejectLabel: 'Cancel',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        this.#medicineService.archiveMedicine(medicine.id).subscribe({
          next: () => {
            this.#messageService.add({ severity: 'success', summary: 'Medicine Archived', detail: 'Medicine has been archived successfully.', life: 3000 });
            this.loadMedicines();
          }
        });
      }
    });
  }

  restoreMedicine(medicine: Medicine): void {
    this.#confirmationService.confirm({
      header: 'Restore Medicine',
      message: `Are you sure you want to restore "${medicine.name}"?`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Restore',
      rejectLabel: 'Cancel',
      acceptButtonStyleClass: 'p-button-success',
      accept: () => {
        this.#medicineService.restoreMedicine(medicine.id).subscribe({
          next: () => {
            this.#messageService.add({ severity: 'success', summary: 'Medicine Restored', detail: 'Medicine has been restored successfully.', life: 3000 });
            this.loadMedicines();
          }
        });
      }
    });
  }

  navigateToReceiveStock(medicine: Medicine): void {
    this.#router.navigate(['/medicines', medicine.id, 'receive-stock'], { state: { data: medicine } });
  }

  showMedicineActions(event: Event, medicine: Medicine, menu: any): void {
    this.selectedMedicine.set(medicine);

    const items: MenuItem[] = [];

    // Common action: View Details (Always available)
    items.push({
      label: 'View Details',
      icon: 'pi pi-eye',
      command: () => this.viewMedicineDetails(medicine)
    });

    items.push({ separator: true });

    if (this.activeTab !== 'archived') {
      if (this.#authStore.hasPermission(Permissions.MedicinesUpdate)) {
        items.push({
          label: 'Edit Medicine',
          icon: 'pi pi-pencil',
          command: () => this.openEditDialog(medicine)
        });
      }

      const canToggle = medicine.isActive 
        ? this.#authStore.hasPermission(Permissions.MedicinesDeactivate)
        : this.#authStore.hasPermission(Permissions.MedicinesActivate);
        
      if (canToggle) {
        items.push({
          label: medicine.isActive ? 'Deactivate' : 'Activate',
          icon: medicine.isActive ? 'pi pi-ban' : 'pi pi-check-circle',
          command: () => this.toggleActivation(medicine)
        });
      }

      if (this.#authStore.hasPermission(Permissions.MedicinesDelete)) {
        items.push({ separator: true });
        items.push({
          label: 'Archive',
          icon: 'pi pi-trash',
          styleClass: 'text-red-500',
          command: () => this.confirmArchive(medicine)
        });
      }
    } else {
      if (this.#authStore.hasPermission(Permissions.MedicinesDelete)) {
        items.push({
          label: 'Restore',
          icon: 'pi pi-refresh',
          styleClass: 'text-green-500',
          command: () => this.restoreMedicine(medicine)
        });
      }
    }

    this.actionMenuItems.set(items);
    menu.toggle(event);
  }

  viewMedicineDetails(medicine: Medicine): void {
    this.#router.navigate(['/medicines', medicine.id, 'details'], { state: { data: medicine } });
  }
}
