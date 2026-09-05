import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  LookupPrescriptionResponse,
  PrescriptionLookupRequest,
  CreateDispenseRequest,
  DispenseDetailsDto,
  DispenseResponseDto,
  DispenseQueryParams
} from '../models/dispensing.models';
import { PaginatedResponse } from '../../../shared/models/paginated-response';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DispensingService {
  readonly #http = inject(HttpClient);
  readonly #baseUrl = `${environment.apiUrl}/api/v1/dispenses`;

  lookupPrescription(request: PrescriptionLookupRequest): Observable<LookupPrescriptionResponse> {
    const params = new HttpParams()
      .set('prescriptionNumber', request.prescriptionNumber.trim())
      .set('documentId', request.patientDocumentId.trim());
    return this.#http.get<LookupPrescriptionResponse>(`${environment.apiUrl}/api/v1/prescriptions/lookup`, { params });
  }

  createDispense(request: CreateDispenseRequest): Observable<DispenseDetailsDto> {
    return this.#http.post<DispenseDetailsDto>(this.#baseUrl, request);
  }

  getDispenses(params: DispenseQueryParams): Observable<PaginatedResponse<DispenseResponseDto>> {
    let httpParams = new HttpParams()
      .set('pageNumber', params.pageNumber.toString())
      .set('pageSize', params.pageSize.toString());

    if (params.searchTerm?.trim()) {
      httpParams = httpParams.set('searchTerm', params.searchTerm.trim());
    }
    if (params.fromDate) {
      httpParams = httpParams.set('fromDate', params.fromDate);
    }
    if (params.toDate) {
      httpParams = httpParams.set('toDate', params.toDate);
    }

    return this.#http.get<PaginatedResponse<DispenseResponseDto>>(this.#baseUrl, { params: httpParams });
  }

  getDispenseById(id: string): Observable<DispenseDetailsDto> {
    return this.#http.get<DispenseDetailsDto>(`${this.#baseUrl}/${id}`);
  }
}
