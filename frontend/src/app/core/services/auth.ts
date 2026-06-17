import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment.development';
import { AuthMessageDto, AuthResponseDto, ChangePasswordDto, ForgotPasswordDto, LoginDto, RegisterDto, ResetPasswordDto } from '../models/auth.model';


@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http=inject(HttpClient);
  private readonly apiUrl=`${environment.apiUrl}/Auth`;
  register(dto:RegisterDto):Observable<AuthMessageDto>{
    return this.http.post<AuthMessageDto>(`${this.apiUrl}/register`,dto);
  }
  login(dto:LoginDto):Observable<AuthResponseDto>{
  return this.http.post<AuthResponseDto>(
    `${this.apiUrl}/login`,
    dto,
    { withCredentials: true }
  );
}
  logout():Observable<AuthMessageDto>{
    return this.http.post<AuthMessageDto>(`${this.apiUrl}/logout`,{},{withCredentials: true});
  }
  refreshToken():Observable<AuthResponseDto>{
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/refresh-token`,{},{withCredentials: true});
  }
  changePassword(dto: ChangePasswordDto): Observable<AuthMessageDto> {
    return this.http.post<AuthMessageDto>(`${this.apiUrl}/change-password`, dto);
  }
  forgotPassword(dto: ForgotPasswordDto): Observable<AuthMessageDto> {
    return this.http.post<AuthMessageDto>(`${this.apiUrl}/forgot-password`, dto);
  }
  resetPassword(dto: ResetPasswordDto): Observable<AuthMessageDto> {
    return this.http.post<AuthMessageDto>(`${this.apiUrl}/reset-password`, dto);
  }
  saveToken(token: string) {
  localStorage.setItem('jwt_token', token);
}

getToken(): string | null {
  return localStorage.getItem('jwt_token');
}

isLoggedIn(): boolean {
  return !!this.getToken();
}

clearToken() {
  localStorage.removeItem('jwt_token');
}
  getUserId(): number | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      
      const userId = payload['nameid'] || 
                     payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || 
                     payload['sub'];
                     
      return userId ? Number(userId) : null;
    } catch (e) {
      console.error('Failed to decode JWT token', e);
      return null;
    }
  }
  getUserRole(): string | null {
  const token = this.getToken();
  if (!token) return null;

  try {
    const payload = JSON.parse(atob(token.split('.')[1]));

    const role = payload['role'] ||
                 payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    return role ?? null;
  } catch (e) {
    console.error('Failed to decode JWT token', e);
    return null;
  }
}
}