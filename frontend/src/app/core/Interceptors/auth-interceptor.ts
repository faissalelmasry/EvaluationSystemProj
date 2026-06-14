import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const token = localStorage.getItem('jwt_token');
  let authReq = req;
  if (token) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }
  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
            if (error.status === 401) {
        console.error('Unauthorized! Your session has expired.');
        localStorage.removeItem('jwt_token'); 
        router.navigate(['/login']);         
      } 
      else if (error.status === 403) {
        alert('Access Denied: You do not have permission to perform this action.');
      }
      else if (error.status === 400 || error.status === 404) {
        const backendMessage = error.error?.Message || 'An unexpected server error occurred.';
        alert(`Error: ${backendMessage}`); 
      }
      else if (error.status === 500) {
        alert('A critical server error occurred. Please contact IT.');
      }
      return throwError(() => error);
    })
  );
};