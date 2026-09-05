import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Prescription, CreatePrescriptionDto, UpdatePrescriptionDto, PrescriptionQueryParams } from '../models/prescription.models';
import { PaginatedResponse } from '../../../shared/models/paginated-response';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PrescriptionService {
  readonly #http = inject(HttpClient);
  readonly #baseUrl = `${environment.apiUrl}/api/v1/prescriptions`;

  getPrescriptions(params: PrescriptionQueryParams): Observable<PaginatedResponse<Prescription>> {
    const httpParams = this.#buildParams(params);
    return this.#http.get<PaginatedResponse<Prescription>>(this.#baseUrl, { params: httpParams });
  }

  getPrescription(id: string): Observable<Prescription> {
    return this.#http.get<Prescription>(`${this.#baseUrl}/${id}`);
  }

  createPrescription(dto: CreatePrescriptionDto): Observable<Prescription> {
    return this.#http.post<Prescription>(this.#baseUrl, dto);
  }

  updatePrescription(id: string, dto: UpdatePrescriptionDto): Observable<Prescription> {
    return this.#http.put<Prescription>(`${this.#baseUrl}/${id}`, dto);
  }

  cancelPrescription(id: string): Observable<void> {
    return this.#http.post<void>(`${this.#baseUrl}/${id}/cancel`, null);
  }

  lookupPrescription(prescriptionNumber: string, patientDocumentId: string): Observable<Prescription> {
    const params = new HttpParams()
      .set('prescriptionNumber', prescriptionNumber)
      .set('patientDocumentId', patientDocumentId);
    return this.#http.get<Prescription>(`${this.#baseUrl}/lookup`, { params });
  }

  #buildParams(params: PrescriptionQueryParams): HttpParams {
    let httpParams = new HttpParams()
      .set('pageNumber', params.pageNumber.toString())
      .set('pageSize', params.pageSize.toString());

    if (params.searchTerm) {
      httpParams = httpParams.set('searchTerm', params.searchTerm);
    }
    if (params.status) {
      httpParams = httpParams.set('status', params.status);
    }
    if (params.sortBy) {
      httpParams = httpParams.set('sortBy', params.sortBy);
    }
    if (params.isDescending !== undefined) {
      httpParams = httpParams.set('isDescending', params.isDescending.toString());
    }

    return httpParams;
  }
}
