import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Medicine, CreateMedicineDto, UpdateMedicineDto, ReceiveStockDto, ReceiveStockResponse, MedicineLookupDto } from '../models/medicine.models';
import { PaginatedResponse, PaginationParams } from '../../../shared/models/paginated-response';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MedicineService {
  readonly #http = inject(HttpClient);
  readonly #baseUrl = `${environment.apiUrl}/api/v1/medicines`;


  getMedicines(params: PaginationParams): Observable<PaginatedResponse<Medicine>> {
    const httpParams = this.#buildParams(params);
    return this.#http.get<PaginatedResponse<Medicine>>(this.#baseUrl, { params: httpParams });
  }

  lookupMedicines(searchTerm?: string): Observable<MedicineLookupDto[]> {
    let params = new HttpParams();
    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }
    return this.#http.get<MedicineLookupDto[]>(`${this.#baseUrl}/lookup`, { params });
  }

  getArchivedMedicines(params: PaginationParams): Observable<PaginatedResponse<Medicine>> {
    const httpParams = this.#buildParams(params);
    return this.#http.get<PaginatedResponse<Medicine>>(`${this.#baseUrl}/archived`, { params: httpParams });
  }

  getLowStockMedicines(params: PaginationParams): Observable<PaginatedResponse<Medicine>> {
    const httpParams = this.#buildParams(params);
    return this.#http.get<PaginatedResponse<Medicine>>(`${this.#baseUrl}/low-stock`, { params: httpParams });
  }

  getActiveMedicines(): Observable<Medicine[]> {
    return this.#http.get<Medicine[]>(`${this.#baseUrl}/active`);
  }

  getMedicine(id: string): Observable<Medicine> {
    return this.#http.get<Medicine>(`${this.#baseUrl}/${id}`);
  }

  createMedicine(dto: CreateMedicineDto): Observable<Medicine> {
    return this.#http.post<Medicine>(this.#baseUrl, dto);
  }

  updateMedicine(id: string, dto: UpdateMedicineDto): Observable<Medicine> {
    return this.#http.put<Medicine>(`${this.#baseUrl}/${id}`, dto);
  }

  receiveStock(id: string, dto: ReceiveStockDto): Observable<ReceiveStockResponse> {
    return this.#http.post<ReceiveStockResponse>(`${this.#baseUrl}/${id}/stock/receive`, dto);
  }

  activateMedicine(id: string): Observable<void> {
    return this.#http.post<void>(`${this.#baseUrl}/${id}/activate`, null);
  }

  deactivateMedicine(id: string): Observable<void> {
    return this.#http.post<void>(`${this.#baseUrl}/${id}/deactivate`, null);
  }

  archiveMedicine(id: string): Observable<void> {
    return this.#http.delete<void>(`${this.#baseUrl}/${id}`);
  }

  restoreMedicine(id: string): Observable<void> {
    return this.#http.post<void>(`${this.#baseUrl}/${id}/restore`, null);
  }

  #buildParams(params: PaginationParams): HttpParams {
    let httpParams = new HttpParams()
      .set('PageNumber', params.pageNumber.toString())
      .set('PageSize', params.pageSize.toString());

    if (params.searchTerm) {
      httpParams = httpParams.set('SearchTerm', params.searchTerm);
    }
    if (params['form']) {
      httpParams = httpParams.set('Form', params['form'].toString());
    }
    if (params['stockUnit']) {
      httpParams = httpParams.set('StockUnit', params['stockUnit'].toString());
    }
    if (params['isActive'] !== undefined) {
      httpParams = httpParams.set('IsActive', params['isActive'].toString());
    }
    if (params.sortBy) {
      httpParams = httpParams.set('SortBy', params.sortBy);
    }
    if (params.sortOrder) {
      httpParams = httpParams.set('IsDescending', (params.sortOrder === 'desc').toString());
    }

    return httpParams;
  }
}
