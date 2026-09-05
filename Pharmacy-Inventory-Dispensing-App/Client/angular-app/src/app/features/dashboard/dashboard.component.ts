import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthStore } from '../../core/auth/auth.store';
import { Permissions } from '../../core/auth/auth.models';
import { HasPermissionDirective } from '../../shared/directives/has-permission.directive';
import { DashboardService } from './dashboard.service';
import { DashboardStats } from './dashboard.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterModule, HasPermissionDirective],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  readonly authStore = inject(AuthStore);
  readonly #dashboardService = inject(DashboardService);
  readonly Permissions = Permissions;

  stats = signal<DashboardStats | null>(null);
  isLoading = signal(true);
  errorMessage = signal<string | null>(null);

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.isLoading.set(true);
    this.errorMessage.set(null);

    this.#dashboardService.getSummary()
      .pipe(finalize(() => this.isLoading.set(false)))
      .subscribe({
        next: (data) => {
          this.stats.set(data);
        },
        error: (err) => {
          console.error('Failed to load dashboard summary:', err);
          this.errorMessage.set('Failed to load dashboard statistics.');
        }
      });
  }
}

