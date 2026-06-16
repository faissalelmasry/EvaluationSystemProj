import { QuestionType } from "../enums/question-type";

export interface GetCriteriaDto {
    id:number,
    title: string,
    description: string,
    orderNo: number,
    questionType: QuestionType,
    maxScore: number,
    weight: number,
    isRequired: boolean    
}
