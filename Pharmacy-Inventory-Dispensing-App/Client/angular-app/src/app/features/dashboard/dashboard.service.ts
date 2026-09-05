import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DashboardStats } from './dashboard.models';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  readonly #http = inject(HttpClient);
  readonly #apiUrl = `${environment.apiUrl}/api/v1/dashboard/summary`;

  getSummary(): Observable<DashboardStats> {
    return this.#http.get<DashboardStats>(this.#apiUrl);
  }
}
