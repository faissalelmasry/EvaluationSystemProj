export interface AddEvalCriterionDto {
  title: string,
  description: string,
  questionType: string,
  maxScore: number,
  weight: number,
  isRequired: boolean,
  orderNo: number
}
