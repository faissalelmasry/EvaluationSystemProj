import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment.development';
import { AuthMessageDto, AuthResponseDto, LoginDto, RegisterDto } from '../models/auth.model';


@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http=inject(HttpClient);
  private readonly apiUrl=`${environment.apiUrl}/auth`;
  register(dto:RegisterDto):Observable<AuthMessageDto>{
    return this.http.post<AuthMessageDto>(`{this.apiUrl}/register`,dto);
  }
  login(dto:LoginDto):Observable<AuthResponseDto>{
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`,dto);
  }
  logout():Observable<AuthMessageDto>{
    return this.http.post<AuthMessageDto>(`${this.apiUrl}/logout`,{},{withCredentials: true});
  }
  refreshToken():Observable<AuthResponseDto>{
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/refresh-token`,{},{withCredentials: true});
  }

}
