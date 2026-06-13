export type Role= 'Admin' | 'Evaluator' | 'Evaluatee' | 'Reviewer';
export interface AssignRoleDto {
  roleName: string;
}