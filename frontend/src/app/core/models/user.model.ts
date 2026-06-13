import {Role} from "./role.model";
import {Department} from "./department.model";
import { JobTitle } from "./job-title.enum";
export interface User {
    id: number;
    username: string;
    fullName?: string;
    email: string;
    departmentId: number;
    roles?: string[];
    role?: string;
    jobTitle: JobTitle;
    isActive: boolean;
    createdAt: string;
    isDeleted: boolean;
    deletedAt?: string | null;
    department?: Department;
}
//dto
export interface UserCreatePayload {
  email: string;
  fullName: string;
  departmentId: number;
  jobTitle: JobTitle;
  roles: string[];
}
export interface CreateUserDto {
  email: string;
  fullName: string;
  username: string;
  password?: string; // Needed for creation
  departmentId: number;
  jobTitle: number; // Enum value
  role: string;
}

export interface UpdateUserDto {
  email: string;
  fullName: string;
  departmentId: number;
  jobTitle: number;
}