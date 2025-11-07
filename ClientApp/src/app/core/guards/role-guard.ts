import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../../account/account.service';
import { ToastrService } from 'ngx-toastr';

export const roleGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);
  const router = inject(Router);
  const requiredRoles: string[] = Array.isArray(route.data['role'])
    ? route.data['role']
    : [route.data['role'] || 'admin'];

  const user = accountService.$user();

  if (user && user.roles && requiredRoles.some(r => user.roles.includes(r))) {
    return true;
  } else {
    toastr.error('You are not authorized to visit this page!');
    router.navigate(['/']);
    return false;
  }
};
