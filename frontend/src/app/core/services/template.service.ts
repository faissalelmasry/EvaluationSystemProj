import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { EvaluationTemplateList } from '../models/evaluation-template-list';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class TemplateService {
  public constructor(private client:HttpClient)
  {

  }
  public GetTemplates(
  PageNumber: number,
  PageSize: number,
  Search: string
): Observable<EvaluationTemplateList[]>
  {
    return this.client.get<EvaluationTemplateList[]>(`${environment.BaseUrl}/EvaluationTemplate?PageNumber=${PageNumber}&PageSize=${PageSize}&Search=${Search}`)
  }
}
