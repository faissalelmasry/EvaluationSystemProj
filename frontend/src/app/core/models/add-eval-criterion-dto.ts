import { QuestionType } from "../enums/question-type";

export interface AddEvalCriterionDto {
  title: string,
  description: string,
  questionType: QuestionType,
  maxScore: number,
  weight: number,
  isRequired: boolean,
  orderNo: number
}
