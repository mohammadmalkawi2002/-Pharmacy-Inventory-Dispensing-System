import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { DispensingService } from '../../services/dispensing.service';
import { DispenseResponseDto } from '../../models/dispensing.models';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TooltipModule } from 'primeng/tooltip';
import { DatePipe } from '@angular/common';
import { LoadingComponent } from '../../../../shared/components/loading/loading.component';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';
import { HasPermissionDirective } from '../../../../shared/directives/has-permission.directive';
import { Permissions } from '../../../../core/auth/auth.models';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-dispensing-records',
  standalone: true,
  imports: [
    FormsModule,
    RouterModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    TooltipModule,
    DatePipe,
    LoadingComponent,
    EmptyStateComponent,
    HasPermissionDirective
  ],
  templateUrl: './dispensing-records.component.html'
})
export class DispensingRecordsComponent implements OnInit {
  readonly #dispensingService = inject(DispensingService);
  readonly #router = inject(Router);
  readonly Permissions = Permissions;

  records = signal<DispenseResponseDto[]>([]);
  totalRecords = signal(0);
  isLoading = signal(true);

  searchQuery = '';
  fromDate: string | undefined = undefined;
  toDate: string | undefined = undefined;

  currentPage = 1;
  pageSize = environment.defaultPageSize;

  ngOnInit(): void {
    this.loadRecords();
  }

  loadRecords(): void {
    this.isLoading.set(true);
    this.#dispensingService.getDispenses({
      pageNumber: this.currentPage,
      pageSize: this.pageSize,
      searchTerm: this.searchQuery?.trim() || undefined,
      fromDate: this.fromDate || undefined,
      toDate: this.toDate || undefined
    }).subscribe({
      next: (res) => {
        this.records.set(res.items);
        this.totalRecords.set(res.totalCount);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadRecords();
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.loadRecords();
  }

  clearFilters(): void {
    this.searchQuery = '';
    this.fromDate = undefined;
    this.toDate = undefined;
    this.currentPage = 1;
    this.loadRecords();
  }

  onPageChange(event: any): void {
    const rows = event.rows ?? this.pageSize;
    const first = event.first ?? 0;
    this.currentPage = Math.floor(first / rows) + 1;
    this.pageSize = rows;
    this.loadRecords();
  }

  viewDetails(record: DispenseResponseDto): void {
    this.#router.navigate(['/dispensing/records', record.id]);
  }

  navigateToNewDispense(): void {
    this.#router.navigate(['/dispensing']);
  }
}
