import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

export const authRedirectGuard: CanActivateFn = () => {
  const router = inject(Router);
  const token = localStorage.getItem('jwt_token');
  console.log('authRedirectGuard fired, token:', token);

  if (!token) {
    return router.createUrlTree(['/login']);
  }

  try {
    const decoded: any = jwtDecode(token);
    const role = (
      decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 
      decoded['role'] || 
      ''
    ).toLowerCase();

    if (role === 'admin') {
      return router.createUrlTree(['/admin']);
    } else if (role === 'evaluator') {
      return router.createUrlTree(['/pending']);
    } else if (role === 'evaluatee') {
      return router.createUrlTree(['/history']);
    }
      else if (role === 'reviewer') {
      return router.createUrlTree(['/reviews']);
    }
    

    return router.createUrlTree(['/login']);
  } catch {
    return router.createUrlTree(['/login']);
  }
};