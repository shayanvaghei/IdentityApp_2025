import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AccountService } from '../account.service';
import { ValidationMessage } from "../../shared/components/errors/validation-message/validation-message";
import { FormInput } from '../../shared/components/form-input/form-input';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    ValidationMessage,
    RouterLink,
    FormInput
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login implements OnInit {
  form: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];
  returnUrl: string | null = null;
  displayResendConfirmationLink: boolean = false;

  constructor(private formBuilder: FormBuilder,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private accountService: AccountService
  ) {
    if (this.accountService.$user()) {
      this.router.navigateByUrl('/');
    } else {
      this.activatedRoute.queryParamMap.subscribe({
        next: (params: any) => {
          if (params) {
            this.returnUrl = params.get('returnUrl');
          }
        }
      })
    }
  }

  ngOnInit(): void {
    this.initializeForm();
  }


  initializeForm() {
    this.form = this.formBuilder.group({
      userName: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  login() {
    this.submitted = true;
    this.errorMessages = [];

    if (this.form.valid) {
      this.accountService.login(this.form.value).subscribe({
        next: (mfaToken: string) => {
          if (mfaToken === '') {
            if (this.returnUrl) {
              this.router.navigateByUrl(this.returnUrl);
            } else {
              this.router.navigateByUrl('/');
            }
          } else {
            this.router.navigate(['/account/mfa-verify'], { queryParams: { mfaToken } });
          }
        },
        error: error => {
          if (error.errors) {
            this.errorMessages = error.errors;
          } else {
            if (error && error.title.includes('Confirm your email first')) {
              this.displayResendConfirmationLink = true;
            }
          }
        }
      })
    }
  }
}
