import { Injectable } from '@angular/core';
import { AddEvalSectionDto } from '../models/add-eval-section-dto';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class SectionService {
  public constructor(private client:HttpClient){}
  public AddSection(templateId:number,dto:AddEvalSectionDto)
  {
    return this.client.post(`${environment.apiUrl}/EvaluationSection/templateid?templateid=${templateId}`,dto)
  }
  public EditSection(sectionId:number,dto:AddEvalSectionDto)
  {
    return this.client.put(`${environment.apiUrl}/EvaluationSection/id?id=${sectionId}`,dto)
  }
  public DeleteSection(sectionId:number)
  {
    return this.client.delete(`${environment.apiUrl}/EvaluationSection/id?id=${sectionId}`)
  }
}
