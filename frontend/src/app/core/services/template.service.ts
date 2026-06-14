import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { EvaluationTemplateList } from '../models/evaluation-template-list';
import { Observable } from 'rxjs';
import { AddTemplateDto } from '../models/add-template-dto';
import { GetTemplateDto } from '../models/get-template-dto';

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
  public AddTemplate(dto:AddTemplateDto)
  {
    return this.client.post<AddTemplateDto>(`${environment.BaseUrl}/EvaluationTemplate/template`,dto);
  }
  public GetTemplateById(id:number):Observable<GetTemplateDto>
  {
    return this .client.get<GetTemplateDto>(`${environment.BaseUrl}/EvaluationTemplate/${id}`)
  }
}
