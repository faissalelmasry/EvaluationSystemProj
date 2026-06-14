// src/app/shared/models/evaluation.dto.ts

// 1. The individual answers the employee gives
export interface QuestionResponseDto {
  criterionId: number;
  score: number;
  textAnswer?: string;     // The '?' means it is optional (nullable in C#)
  selectedOption?: string; // Optional
  comment?: string;        // Optional
}

// 2. The final package sent to POST /api/evaluations/{id}/submit
export interface SubmitEvaluationDto {
  responses: QuestionResponseDto[];
}

// 3. The package sent to POST /api/evaluations/{id}/approve or /reject (For the Manager Form)
export interface SubmitReviewDto {
  reviewComment?: string;
}