import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AccountService } from '../../account/account.service';

export const authGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  if (accountService.$user()) {
    return true;
  } else {
    router.navigate(['account/login'], { queryParams: { returnUrl: state.url } });
    return false;
  }
};
