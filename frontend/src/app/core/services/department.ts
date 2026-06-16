import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { Department, DepartmentCreatePayload } from '../models/department.model';
import { PagesResult } from '../models/pagination.model';
import { SKIP_AUTH } from '../Interceptors/auth-interceptor';

function normalizePagesResult<T>(raw: any): PagesResult<T> {
  return {
    items: raw.Items ?? raw.items ?? [],
    totalCount: raw.TotalCount ?? raw.totalCount ?? 0,
    pageNumber: raw.PageNumber ?? raw.pageNumber ?? 1,
    pageSize: raw.PageSize ?? raw.pageSize ?? 10,
  };
}

@Injectable({
  providedIn: 'root',
})
export class DepartmentService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/departments`;

  getDepartments(pageNumber = 1, pageSize = 10, search?: string, sortBy?: string, descending?: boolean): Observable<PagesResult<Department>> {
    let params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);
    if (search) {
      params = params.set('search', search);
    }
    if (sortBy) {
      params = params.set('sortBy', sortBy);
    }
    if (descending) {
      params = params.set('descending', descending);
    }
    return this.http.get<PagesResult<Department>>(`${this.apiUrl}`, { params }).pipe(
      map(res => normalizePagesResult<Department>(res))
    );
  }

  getDepartmentsPublic(pageNumber = 1, pageSize = 100): Observable<PagesResult<Department>> {
    const params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', pageSize);
    return this.http.get<PagesResult<Department>>(`${this.apiUrl}`, {
      params,
      context: new HttpContext().set(SKIP_AUTH, true),
    }).pipe(
      map(res => normalizePagesResult<Department>(res))
    );
  }

  create(payLoad: DepartmentCreatePayload): Observable<Department> {
    return this.http.post<Department>(`${this.apiUrl}`, payLoad);
  }

  update(id: number, payLoad: DepartmentCreatePayload): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, payLoad);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getById(id: number): Observable<Department> {
    return this.http.get<Department>(`${this.apiUrl}/${id}`);
  }
  
}
