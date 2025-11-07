import { Component } from '@angular/core';
import { CodeInput } from '../../shared/components/code-input/code-input';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ValidationMessage } from '../../shared/components/errors/validation-message/validation-message';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AccountService } from '../account.service';
import { SharedService } from '../../shared/shared.service';
import { matchValues } from '../../shared/sharedHelper';
import { ResetPasswordModel } from '../../shared/models/account/resetPassword_m';
import { FormInput } from '../../shared/components/form-input/form-input';

@Component({
  selector: 'app-reset-password',
  imports: [
    CodeInput,
    ReactiveFormsModule,
    ValidationMessage,
    RouterLink,
    FormInput
  ],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.scss'
})
export class ResetPassword {
  email: string | undefined;
  fullCode: string | undefined;
  form: FormGroup = new FormGroup({});
  errorMessages: string[] = [];

  constructor(private accountService: AccountService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private sharedService: SharedService,
    private formBuilder: FormBuilder
  ) {
    if (this.accountService.$user()) {
      this.router.navigateByUrl('/');
    } else {
      this.activatedRoute.queryParamMap.subscribe({
        next: (params: any) => {
          const email = params.get('email');
          if (email) {
            this.email = email;
            this.initializeForm();
          } else {
            this.router.navigateByUrl('/account/login');
          }
        }
      });
    }
  }

  initializeForm() {
    this.form = this.formBuilder.group({
      newPassword: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(15)]],
      confirmNewPassword: ['', [Validators.required, matchValues('newPassword')]]
    });

    this.form.controls['newPassword'].valueChanges.subscribe({
      next: () => this.form.controls['confirmNewPassword'].updateValueAndValidity()
    });
  }

  fullCodeReceive(code: string) {
    this.fullCode = code.replace(/ /g, '');
  }

  submit() {
    this.errorMessages = [];

    if (this.form.valid && this.email && this.fullCode && this.fullCode.length == 6) {
      this.accountService.resetPassword(
        new ResetPasswordModel(this.fullCode, this.email, this.form.get('newPassword')?.value)).subscribe({
          next: response => {
            this.sharedService.showNotification(response);
            this.router.navigateByUrl('/account/login');
          }, error: error => {
            if (error.errors) {
              this.errorMessages = error.errors;
            } else {
              if (error && error.title.includes('Confirm your email first')) {
                this.router.navigateByUrl('/account/confirm-email?email=' + this.email);
              }
            }
          }
        });
    }
  }
}
