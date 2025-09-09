import { inject, Inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { LoginModel } from '../shared/models/account/login_m';
import { AuthStatusModel, UserModel } from '../shared/models/account/user_m';
import { map } from 'rxjs';
import { RegisterModel } from '../shared/models/account/register_m';
import { ApiResponse } from '../shared/models/apiResponse_m';
import { ConfirmEmailModel, EmailModel } from '../shared/models/account/confirmEmail_m';
import { ResetPasswordModel } from '../shared/models/account/resetPassword_m';
import { MfaVerifyModel } from '../shared/models/account/mfaVerify_m';
import { MfaDisableModel } from '../shared/models/account/mfaDisable_m';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  apiUrl = environment.apiUrl;
  $user = signal<UserModel | null>(null);

  constructor(private http: HttpClient,
    private route: Router
  ) { }

  authStatus() {
    return this.http.get<AuthStatusModel>(this.apiUrl + 'account/auth-status');
  }

  refreshUser() {
    return this.http.get<UserModel>(this.apiUrl + 'account/refresh-appuser').pipe(
      map((user: UserModel) => {
        if (user) {
          this.setUser(user);
        }
      })
    )
  }

  login(model: LoginModel) {
    return this.http.post<UserModel>(this.apiUrl + 'account/login', model)
      .pipe(
        map((user: UserModel) => {
          if (user && user.jwt) {
            this.setUser(user);
            return '';
          } else {
            return user.mfaToken;
          }
        })
      );
  }

  mfaVerify(model: MfaVerifyModel) {
    return this.http.post<UserModel>(this.apiUrl + 'account/mfa-verify', model)
      .pipe(
        map((user: UserModel) => {
          if (user && user.jwt) {
            this.setUser(user);
          }
        })
      );
  }

  register(model: RegisterModel) {
    return this.http.post<ApiResponse<any>>(environment.apiUrl + 'account/register', model);
  }

  checkNameTaken(name: string) {
    return this.http.get(environment.apiUrl + 'account/name-taken?name=' + name);
  }

  checkEmailTaken(email: string) {
    return this.http.get(environment.apiUrl + 'account/email-taken?email=' + email);
  }

  logout() {
    return this.http.post<{}>(this.apiUrl + 'account/logout', {})
      .pipe(
        map(() => {
          this.$user.set(null);
          this.route.navigateByUrl('/');
        }));
  }

  confirmEmail(model: ConfirmEmailModel) {
    return this.http.put<ApiResponse<any>>(this.apiUrl + 'account/confirm-email', model);
  }

  resendConfirmationEmail(model: EmailModel) {
    return this.http.post<ApiResponse<any>>(this.apiUrl + 'account/resend-confirmation-email', model);
  }

  forgotUsernameOrPassword(model: EmailModel) {
    return this.http.post<ApiResponse<any>>(this.apiUrl + 'account/forgot-username-or-password', model);
  }

  resetPassword(model: ResetPasswordModel) {
    return this.http.put<ApiResponse<any>>(this.apiUrl + 'account/reset-password', model);
  }

  mfaDisableRequest(model: EmailModel) {
    return this.http.post<ApiResponse<any>>(this.apiUrl + 'account/mfa-disable-request', model);
  }

  mfaDisable(model: MfaDisableModel) {
    return this.http.put<ApiResponse<any>>(this.apiUrl + 'account/mfa-disable', model);
  }

  setUser(user: UserModel | null) {
    this.$user.set(user);
  }
}
