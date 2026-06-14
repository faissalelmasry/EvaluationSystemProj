import { GetCriteriaDto } from "./get-criteria-dto";

export interface GetSectionsDto {
  id:number
  title:string
  description:string
  criteria:GetCriteriaDto[];
}
