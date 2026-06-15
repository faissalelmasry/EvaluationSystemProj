import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';

export const roleGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);

  const expectedRoles = route.data['roles'] as Array<string>;

  if (!expectedRoles || expectedRoles.length === 0) {
    return true;
  }
 
  const token = localStorage.getItem('jwt_token');

  if (!token) {
    console.warn('Access Denied: No token found. Redirecting to login.');
    router.navigate(['/login']);
    return false;
  }

  try {
    const decodedToken: any = jwtDecode(token);

    const roleClaim = 
      decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || 
      decodedToken['role'];

    if (roleClaim) {

      const userRoles: string[] = Array.isArray(roleClaim) ? roleClaim : [roleClaim];
      const hasPermission = userRoles.some(role => expectedRoles.includes(role));

      if (hasPermission) {
        return true; 
      }
    }
  } catch (error) {
    console.error('Error decoding token. The token might be corrupted.');
  }
  console.warn('Access Denied: You do not have the required role.');
  router.navigate(['/unauthorized']); 
  return false;
};