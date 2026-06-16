import { QuestionType } from "../enums/question-type";

export interface AddCriteriaDto {
    title: string,
    description: string,
    OrderNo: number,
    QuestionType: QuestionType,
    maxScore: number,
    Weight: number,
    IsRequired: boolean
}
