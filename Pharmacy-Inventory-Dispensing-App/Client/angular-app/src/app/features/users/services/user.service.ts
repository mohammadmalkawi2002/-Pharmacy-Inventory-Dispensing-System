import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SystemUser, CreateUserDto, UpdateUserDto } from '../models/user.models';
import { PaginatedResponse, PaginationParams } from '../../../shared/models/paginated-response';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  readonly #http = inject(HttpClient);
  readonly #baseUrl = `${environment.apiUrl}/api/v1/users`;

  getUsers(params: PaginationParams): Observable<PaginatedResponse<SystemUser>> {
    const httpParams = this.#buildParams(params);
    return this.#http.get<PaginatedResponse<SystemUser>>(this.#baseUrl, { params: httpParams });
  }

  getUser(id: string): Observable<SystemUser> {
    return this.#http.get<SystemUser>(`${this.#baseUrl}/${id}`);
  }

  createUser(dto: CreateUserDto): Observable<SystemUser> {
    return this.#http.post<SystemUser>(this.#baseUrl, dto);
  }

  updateUser(id: string, dto: UpdateUserDto): Observable<SystemUser> {
    return this.#http.put<SystemUser>(`${this.#baseUrl}/${id}`, dto);
  }

  activateUser(id: string): Observable<void> {
    return this.#http.patch<void>(`${this.#baseUrl}/${id}/activate`, {});
  }

  deactivateUser(id: string): Observable<void> {
    return this.#http.patch<void>(`${this.#baseUrl}/${id}/deactivate`, {});
  }

  #buildParams(params: PaginationParams): HttpParams {
    let httpParams = new HttpParams()
      .set('PageNumber', params.pageNumber.toString())
      .set('PageSize', params.pageSize.toString());

    if (params.searchTerm) {
      httpParams = httpParams.set('SearchTerm', params.searchTerm);
    }
    
    if (params['role']) {
      httpParams = httpParams.set('Role', params['role'].toString());
    }

    return httpParams;
  }
}
