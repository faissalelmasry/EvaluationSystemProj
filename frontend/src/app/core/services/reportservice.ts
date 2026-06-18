import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class Reportservice {
  private apiUrl = `${environment.apiUrl}/Reports`;

  constructor(private http: HttpClient) { }

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('jwt_token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  getDashboard(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/dashboard`, { headers: this.getHeaders() });
  }

  getByDepartment(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/by-department`, { headers: this.getHeaders() });
  }

  getByUserId(userId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/user/${userId}`, { headers: this.getHeaders() });
  }

  getCompletionRate(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/completion-rate`, { headers: this.getHeaders() });
  }

  getTopScores(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/top-scores`, { headers: this.getHeaders() });
  }

  getAssignmentPdf(id: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/assignment/${id}/pdf`, {
      headers: this.getHeaders(),
      responseType: 'blob'
    });
  }
}