import { inject, Injectable } from '@angular/core';
import { AccountService } from '../account/account.service';
import { of, switchMap } from 'rxjs';
import { AuthStatusModel } from '../shared/models/account/user_m';

@Injectable({
  providedIn: 'root'
})
export class CoreService {
  private accountService = inject(AccountService);

  initializeApp() {
    return this.accountService.authStatus().pipe(
      switchMap((response: AuthStatusModel) => {
        if (response && response.isAuthenticated) {
          return this.accountService.refreshUser();
        } else {
          return of(null);
        }
      })
    )
  }
}
