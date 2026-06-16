import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AssignmentResponseDto, CreateAssignmentDto } from '../models/assignmentmodels';
import { Observable } from 'rxjs/internal/Observable';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class Assignment {
  private apiUrl = `${environment.apiUrl}/EvaluationAssignments`;

  constructor(private http: HttpClient) { }

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('jwt_token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  private getAdminIdFromToken(): number {
    const token = localStorage.getItem('jwt_token');
    if (!token) return 0;
    try {
      const decoded: any = jwtDecode(token);
      const adminId = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || 
                      decoded['id'] || 
                      decoded['sub'];
      return adminId ? Number(adminId) : 0;
    } catch {
      return 0;
    }
  }

  createAssignment(dto: CreateAssignmentDto): Observable<AssignmentResponseDto> {
    const adminId = this.getAdminIdFromToken();
    
    return this.http.post<AssignmentResponseDto>(
      `${this.apiUrl}?adminId=${adminId}`, 
      dto, 
      { headers: this.getHeaders() }
    );
  }
  updateAssignment(id: number, dto: CreateAssignmentDto): Observable<any> {
    return this.http.put<any>(
      `${this.apiUrl}/${id}`, 
      dto, 
      { headers: this.getHeaders() }
    );
  }

  getAssignments(): Observable<AssignmentResponseDto[]> {
    return this.http.get<AssignmentResponseDto[]>(this.apiUrl, { headers: this.getHeaders() });
  }


  getAssignmentById(id: number): Observable<AssignmentResponseDto> {
    return this.http.get<AssignmentResponseDto>(`${this.apiUrl}/${id}`, { headers: this.getHeaders() });
  } 

 getPendingAssignmentsForUser(evaluatorId: number): Observable<any> {
  return this.http.get<any>(`${this.apiUrl}/my-pending?evaluatorId=${evaluatorId}`);
}
}