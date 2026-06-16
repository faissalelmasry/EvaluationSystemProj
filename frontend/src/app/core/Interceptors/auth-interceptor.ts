import { HttpInterceptorFn, HttpErrorResponse, HttpContextToken } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

export const SKIP_AUTH = new HttpContextToken<boolean>(() => false);

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);

  if (req.context.get(SKIP_AUTH)) {
    return next(req);
  }

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
        localStorage.removeItem('jwt_token'); 
        router.navigate(['/login']);         
      } 
      else if (error.status === 403) {
        alert('Access Denied: You do not have permission to perform this action.');
      }
      else if (error.status === 400 || error.status === 404) {
        // Components handle these errors inline — no alert needed
      }
      else if (error.status === 500) {
        // alert('A critical server error occurred. Please contact IT.');
      }
      return throwError(() => error);
    })
  );
};
