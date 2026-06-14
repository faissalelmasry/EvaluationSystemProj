import { AddSectionDto } from "./add-section-dto";

export interface AddTemplateDto {
    title: string,
  description: string,
  createdById: number,
  sections: AddSectionDto[]
}
