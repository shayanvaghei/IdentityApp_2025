import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ValidationMessage } from '../../shared/components/errors/validation-message/validation-message';
import { Router, RouterLink } from '@angular/router';
import { AccountService } from '../account.service';
import { SharedService } from '../../shared/shared.service';
import { matchValues } from '../../shared/sharedHelper';
import { FormInput } from '../../shared/components/form-input/form-input';

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    ValidationMessage,
    RouterLink,
    FormInput
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
      Validators.pattern('^[a-zA-Z][a-zA-Z0-9]*$')]],
      email: ['', [Validators.required, Validators.pattern('^.+@[^\\.].*\\.[a-z]{2,}$')]],
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
}
