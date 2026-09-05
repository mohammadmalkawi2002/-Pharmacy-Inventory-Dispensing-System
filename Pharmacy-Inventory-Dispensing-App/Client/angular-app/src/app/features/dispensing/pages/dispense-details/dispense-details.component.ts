import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { DispensingService } from '../../services/dispensing.service';
import { DispenseDetailsDto } from '../../models/dispensing.models';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';
import { DatePipe } from '@angular/common';
import { LoadingComponent } from '../../../../shared/components/loading/loading.component';
import { EmptyStateComponent } from '../../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'app-dispense-details',
  standalone: true,
  imports: [
    RouterModule,
    ButtonModule,
    TooltipModule,
    DatePipe,
    LoadingComponent,
    EmptyStateComponent
  ],
  templateUrl: './dispense-details.component.html'
})
export class DispenseDetailsComponent implements OnInit {
  readonly #router = inject(Router);
  readonly #route = inject(ActivatedRoute);
  readonly #dispensingService = inject(DispensingService);

  dispense = signal<DispenseDetailsDto | undefined>(undefined);
  isLoading = signal(true);

  ngOnInit(): void {
    const id = this.#route.snapshot.paramMap.get('id');
    if (id) {
      this.loadDetails(id);
    } else {
      this.isLoading.set(false);
    }
  }

  loadDetails(id: string): void {
    this.isLoading.set(true);
    this.#dispensingService.getDispenseById(id).subscribe({
      next: (data) => {
        this.dispense.set(data);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load dispense details', err);
        this.isLoading.set(false);
      }
    });
  }

  onClose(): void {
    this.#router.navigate(['/dispensing/records']);
  }
}
