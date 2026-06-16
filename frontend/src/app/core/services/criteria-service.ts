import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { AddEvalCriterionDto } from '../models/add-eval-criterion-dto';

@Injectable({
  providedIn: 'root',
})
export class CriteriaService {
  public constructor(private client:HttpClient){}
  public AddCriterion(sectionId:number,dto:AddEvalCriterionDto)
    {
      return this.client.post(`${environment.apiUrl}/EvaluationCriteria/sectionid?sectionid=${sectionId}`,dto)
    }
    public EditCriterion(criterionId:number,dto:AddEvalCriterionDto)
    {
      return this.client.put(`${environment.apiUrl}/EvaluationCriteria/criteriaid?criteriaid=${criterionId}`,dto)
    }
    public DeleteCriterion(criterionId:number)
    {
      return this.client.delete(`${environment.apiUrl}/EvaluationCriteria/criteriaid?criteriaid=${criterionId}`)
    }
}
