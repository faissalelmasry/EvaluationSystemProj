export interface Department {
  id: number;
  name: string;
  description?: string;
  isActive: boolean;
}

export interface DepartmentCreatePayload {
  name: string;
  description?: string;
}