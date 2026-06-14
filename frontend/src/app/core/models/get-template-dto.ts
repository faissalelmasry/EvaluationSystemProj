import { GetSectionsDto } from "./get-sections-dto";

export interface GetTemplateDto {
  id:number
  title:string
  description:string
  sections:GetSectionsDto[];
}
