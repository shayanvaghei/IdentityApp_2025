import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ValidationMessage } from '../../shared/components/errors/validation-message/validation-message';
import { FormInput } from '../../shared/components/form-input/form-input';
import { MyProfileService } from '../my-profile.service';
import { SharedService } from '../../shared/shared.service';
import { AccountService } from '../../account/account.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-delete-account',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ValidationMessage,
    FormInput
  ],
  templateUrl: './delete-account.html',
  styleUrl: './delete-account.scss'
})
export class DeleteAccount implements OnInit {
  form: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];

  constructor(private formBuilder: FormBuilder,
    private myProfileService: MyProfileService,
    private sharedService: SharedService,
    private accountService: AccountService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm() {
    this.form = this.formBuilder.group({
      currentUserName: ['', [Validators.required]],
      currentPassword: ['', [Validators.required]],
      confirmation: ['', [Validators.required]],
    });
  }

  save() {
    this.submitted = true;
    this.errorMessages = [];

    if (this.form.valid) {
      this.myProfileService.deleteAccount(this.form.value).subscribe({
        next: response => {
          this.sharedService.showNotification(response);
          this.accountService.setUser(null);
          this.router.navigateByUrl('/');
        }, error: error => {
          if (error) {
            if (error.errors) {
              this.errorMessages = error.errors;
            }
          }
        }
      })
    }
  }
}
