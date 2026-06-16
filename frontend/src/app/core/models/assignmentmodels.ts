export interface Assignmentmodels {}
export interface CreateAssignmentDto
 {
    templateId: number;
  evaluatorId: number;
  evaluateeId: number;
  dueDate: Date | string;
}
export interface AssignmentResponseDto {
  id: number;
  templateId: number;
  templateTitle?: string;
  evaluatorId: number;
  evaluatorName?: string;
  evaluateeId: number;
  evaluateeName?: string;
  status: string | number;
  progress?: number;
  dueDate: string;
  createdAt: string;
}