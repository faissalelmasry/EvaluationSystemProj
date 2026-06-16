import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Observable } from 'rxjs';
import { SubmitEvaluationDto, SubmitReviewDto } from '../models/evaluation.model';

@Injectable({
  providedIn: 'root',
})
export class EvaluationService {
  private http = inject(HttpClient);
  
  private apiUrl = `${environment.apiUrl}/Evaluations`; 
  
  submitEvaluation(assignmentId: number, data: SubmitEvaluationDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/${assignmentId}/submit`, data);
  }

  getResponses(assignmentId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${assignmentId}/responses`);
  }



  public getEvaluationForReview(assignmentId: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${assignmentId}/responses`);
  }
  public approveEvaluation(assignmentId: number, notes: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${assignmentId}/approve`, { notes });
  }

  public rejectEvaluation(assignmentId: number, notes: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${assignmentId}/reject`, { notes });
  }
  public getMyPendingAssignments(evaluatorId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/EvaluationAssignments/my-pending?evaluatorId=${evaluatorId}`);
  }
  getEvaluationResult(assignmentId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${assignmentId}/result`); 
  }
}