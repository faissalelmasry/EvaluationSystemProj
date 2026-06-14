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
  private apiUrl=environment.apiUrl;
  submitEvaluation(assignmentId: number, data: SubmitEvaluationDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/${assignmentId}/submit`, data);
  }

  // 2. GET /api/Evaluations/{assignmentId}/responses
  getResponses(assignmentId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${assignmentId}/responses`);
  }

  // 3. GET /api/Evaluations/{assignmentId}/result
  getResult(assignmentId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/${assignmentId}/result`);
  }

  // 4. POST /api/Evaluations/{assignmentId}/approve
  approveEvaluation(assignmentId: number, data: SubmitReviewDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/${assignmentId}/approve`, data);
  }

  // 5. POST /api/Evaluations/{assignmentId}/reject
  rejectEvaluation(assignmentId: number, data: SubmitReviewDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/${assignmentId}/reject`, data);
  }
}
