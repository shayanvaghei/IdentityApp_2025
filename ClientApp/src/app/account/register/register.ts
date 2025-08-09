import { Component, OnInit } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, FormBuilder, FormGroup, ReactiveFormsModule, ValidatorFn, Validators } from '@angular/forms';
import { ValidationMessage } from '../../shared/components/errors/validation-message/validation-message';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AccountService } from '../account.service';
import { SharedService } from '../../shared/shared.service';
import { map, of, switchMap, timer } from 'rxjs';
import { matchValues } from '../../shared/sharedHelper';

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    ValidationMessage,
    CommonModule,
    RouterLink
  ],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register implements OnInit {
  form: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];

  constructor(private formBuilder: FormBuilder,
    private router: Router,
    private accountService: AccountService,
    private sharedService: SharedService
  ) {
    if (this.accountService.$user()) {
      this.router.navigateByUrl('/');
    }
  }

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.form = this.formBuilder.group({
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(15),
      Validators.pattern('^[a-zA-Z][a-zA-Z0-9]*$')], [this.checkNameNotTaken()]],
      email: ['', [Validators.required, Validators.pattern('^.+@[^\\.].*\\.[a-z]{2,}$')], [this.checkEmailNotTaken()]],
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(15)]],
      confirmPassword: ['', [Validators.required, matchValues('password')]]
    });

    this.form.controls['password'].valueChanges.subscribe({
      next: () => this.form.controls['confirmPassword'].updateValueAndValidity()
    });
  }

  register() {
    this.submitted = true;
    this.errorMessages = [];

    if (this.form.valid) {
      this.accountService.register(this.form.value).subscribe({
        next: response => {
          this.sharedService.showNotification(response);
          this.router.navigate(['/account/confirm-email'], {
            queryParams: {
              email: this.form.get('email')?.value
            }
          });
        },
        error: error => {
          if (error.errors) {
            this.errorMessages = error.errors;
          }
        }
      })
    }
  }

  // #region Private Methods
  private checkNameNotTaken(): AsyncValidatorFn {
    return control => {
      return timer(500).pipe(
        switchMap(_ => {
          if (!control.value) {
            return of(null);
          }

          return this.accountService.checkNameTaken(control.value).pipe(
            map((res: any) => {
              if (res && res.isTaken) {
                return { nameTaken: true };
              }

              return null;
            })
          )
        })
      )
    }
  }

  private checkEmailNotTaken(): AsyncValidatorFn {
    return control => {
      return timer(500).pipe(
        switchMap(_ => {
          if (!control.value) {
            return of(null);
          }

          return this.accountService.checkEmailTaken(control.value).pipe(
            map((res: any) => {
              if (res && res.isTaken) {
                return { emailTaken: true };
              }

              return null;
            })
          )
        })
      )
    }
  }
  // #endregion
}
