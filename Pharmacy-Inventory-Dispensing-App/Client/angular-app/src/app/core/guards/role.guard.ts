import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStore } from '../auth/auth.store';
import { MessageService } from 'primeng/api';

export const roleGuard: CanActivateFn = (route) => {
  const authStore = inject(AuthStore);
  const router = inject(Router);
  const messageService = inject(MessageService, { optional: true });

  const requiredRoles = route.data?.['roles'] as string[] | undefined;
  const requiredPermissions = route.data?.['permissions'] as string[] | undefined;

  // If neither specified, allow access
  if (!requiredRoles && !requiredPermissions) {
    return true;
  }

  // Check roles
  if (requiredRoles) {
    const userRoles = authStore.userRoles();
    const hasRole = requiredRoles.some(r => userRoles.includes(r as never)) || userRoles.includes('Admin');
    if (hasRole) return true;
  }

  // Check permissions
  if (requiredPermissions) {
    const hasAny = requiredPermissions.some(perm => authStore.hasPermission(perm));
    if (hasAny) return true;
  }

  messageService?.add({
    severity: 'error',
    summary: 'Access Denied',
    detail: 'You do not have permission to access this page.',
    life: 4000
  });

  return router.createUrlTree(['/dashboard']);
};
