import { JobTitle } from "./job-title.enum";

export interface LoginDto {
  email: string;
  password: string;
}
export interface RegisterDto {
  fullName: string;
  email: string;
  password: string;
  departmentId: number;
  jobTitle: JobTitle;
}
export interface LoginResponse {
  token: string;
}
export interface AuthResponseDto {
  token: string;
  tokenExpiresAt: string;
}
export interface AuthMessageDto {
  message: string;
}
export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
}
export interface ForgotPasswordDto {
  email: string;
}
export interface ResetPasswordDto {
  email: string;
  token: string;
  newPassword: string;
}