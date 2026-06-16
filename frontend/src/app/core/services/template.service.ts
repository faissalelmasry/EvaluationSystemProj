import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { EvaluationTemplateList } from '../models/evaluation-template-list';
import { Observable } from 'rxjs';
import { AddTemplateDto } from '../models/add-template-dto';
import { GetTemplateDto } from '../models/get-template-dto';
import { UpdateTemplateDto } from '../models/update-template-dto';

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
    return this.client.get<EvaluationTemplateList[]>(`${environment.apiUrl}/EvaluationTemplate?PageNumber=${PageNumber}&PageSize=${PageSize}&Search=${Search}`)
  }
  public AddTemplate(dto:AddTemplateDto)
  {
    return this.client.post<AddTemplateDto>(`${environment.apiUrl}/EvaluationTemplate/template`,dto);
  }
  public GetTemplateById(id:number):Observable<GetTemplateDto>
  {
    return this.client.get<GetTemplateDto>(`${environment.apiUrl}/EvaluationTemplate/id?id=${id}`)
  }
  public UpdateTemplate(id:number,dto:UpdateTemplateDto)
  {
    return this.client.put<UpdateTemplateDto>(`${environment.apiUrl}/EvaluationTemplate/id?id=${id}`,dto)
  }
  public DeleteTemplate(id:number)
  {
    return this.client.delete(`${environment.apiUrl}/EvaluationTemplate/id?id=${id}`);
  }
}
