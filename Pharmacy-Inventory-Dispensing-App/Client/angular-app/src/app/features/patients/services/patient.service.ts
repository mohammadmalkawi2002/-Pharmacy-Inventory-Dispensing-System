import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Patient, CreatePatientDto, UpdatePatientDto, PatientQueryParams, PatientLookupDto } from '../models/patient.models';
import { PaginatedResponse } from '../../../shared/models/paginated-response';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PatientService {
  readonly #http = inject(HttpClient);
  readonly #baseUrl = `${environment.apiUrl}/api/v1/patients`;





  getPatients(params: PatientQueryParams): Observable<PaginatedResponse<Patient>> {
    const httpParams = this.#buildParams(params);
    return this.#http.get<PaginatedResponse<Patient>>(this.#baseUrl, { params: httpParams });
  }

  lookupPatients(searchTerm?: string): Observable<PatientLookupDto[]> {
    let params = new HttpParams();
    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }
    return this.#http.get<PatientLookupDto[]>(`${this.#baseUrl}/lookup`, { params });
  }


  getArchivedPatients(params: PatientQueryParams): Observable<PaginatedResponse<Patient>> {
    const httpParams = this.#buildParams(params);
    return this.#http.get<PaginatedResponse<Patient>>(`${this.#baseUrl}/archived`, { params: httpParams });
  }

  getPatient(id: string): Observable<Patient> {
    return this.#http.get<Patient>(`${this.#baseUrl}/${id}`);
  }

  createPatient(dto: CreatePatientDto): Observable<Patient> {
    return this.#http.post<Patient>(this.#baseUrl, dto);
  }

  updatePatient(id: string, dto: UpdatePatientDto): Observable<Patient> {
    return this.#http.put<Patient>(`${this.#baseUrl}/${id}`, dto);
  }

  archivePatient(id: string): Observable<void> {
    return this.#http.delete<void>(`${this.#baseUrl}/${id}`);
  }

  restorePatient(id: string): Observable<void> {
    return this.#http.post<void>(`${this.#baseUrl}/${id}/restore`, null);
  }

  #buildParams(params: PatientQueryParams): HttpParams {
    let httpParams = new HttpParams()
      .set('PageNumber', params.pageNumber.toString())
      .set('PageSize', params.pageSize.toString());

    if (params.searchTerm) {
      httpParams = httpParams.set('SearchTerm', params.searchTerm);
    }
    if (params.documentType) {
      httpParams = httpParams.set('DocumentType', params.documentType);
    }
    if (params.sortBy) {
      httpParams = httpParams.set('SortBy', params.sortBy);
    }
    if (params.isDescending !== undefined) {
      httpParams = httpParams.set('IsDescending', params.isDescending.toString());
    }

    return httpParams;
  }
}
