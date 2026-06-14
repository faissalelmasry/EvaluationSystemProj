import { AddCriteriaDto } from "./add-criteria-dto";

export interface AddSectionDto {
  title: string,
  description: string,
  OrderNo: number,
  Criteria: AddCriteriaDto[]
}
