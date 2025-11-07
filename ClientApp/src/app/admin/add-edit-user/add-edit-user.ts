import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, AsyncValidatorFn, ValidatorFn, AbstractControl, ReactiveFormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { timer, switchMap, of, map, forkJoin } from 'rxjs';
import { UserAddEdit } from '../../shared/models/admin/userAddEdit';
import { SharedService } from '../../shared/shared.service';
import { AdminService } from '../admin.service';
import { ValidationMessage } from '../../shared/components/errors/validation-message/validation-message';
import { FormInput } from '../../shared/components/form-input/form-input';
import { NgSelectModule } from '@ng-select/ng-select';

@Component({
  selector: 'app-add-edit-user',
  imports: [
    ReactiveFormsModule,
    ValidationMessage,
    FormInput,
    NgSelectModule,
    RouterLink
  ],
  templateUrl: './add-edit-user.html',
  styleUrl: './add-edit-user.scss'
})
export class AddEditUser implements OnInit {
  form: FormGroup = new FormGroup({});
  user: UserAddEdit | undefined;
  submitted = false;
  userId: number;
  appRoles: string[] | undefined;
  completed = false;
  errorMessages: string[] = [];

  constructor(private formBuilder: FormBuilder,
    public adminService: AdminService,
    private router: Router,
    private route: ActivatedRoute,
    public sharedService: SharedService) {
    const userIdParam = this.route.snapshot.paramMap.get('id');
    const parsedId = Number(userIdParam);
    this.userId = isNaN(parsedId) ? 0 : parsedId;
  }

  ngOnInit(): void {
    forkJoin({
      user: this.adminService.getUser(this.userId),
      appRoles: this.adminService.getApplicationRoles$()
    }).subscribe({
      next: ({ user, appRoles }) => {
        this.user = user;
        this.appRoles = appRoles;
        this.initializeForm();
        this.completed = true;
      },
      error: (err) => {
        console.error('Error loading user or roles', err);
      }
    });
  }

  initializeForm() {
    this.form = this.formBuilder.group({
      userId: [0],
      name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(15),
      Validators.pattern('^[a-zA-Z][a-zA-Z0-9]*$')], [this.checkNameNotTaken()]],
      email: ['', [Validators.required, Validators.pattern('^.+@[^\\.].*\\.[a-z]{2,}$')], [this.checkEmailNotTaken()]],
      password: [''],
      emailConfirmed: [false, Validators.required],
      isActive: [true, Validators.required],
      roles: [[], [this.atLeastOneRequired()]]
    });

    if (this.user) {
      this.form.get('userId')?.setValue(this.user.userId);
      this.form.get('name')?.setValue(this.user.name);
      this.form.get('email')?.setValue(this.user.email);
      this.form.get('emailConfirmed')?.setValue(this.user.emailConfirmed);
      this.form.get('isActive')?.setValue(this.user.isActive);
      this.form.get('roles')?.setValue(this.user.roles);
    } else {
      this.form.get('password')?.addValidators([Validators.required, Validators.minLength(6), Validators.maxLength(15)]);
      this.form.get('password')?.updateValueAndValidity();
    }
  }

  createOrUpdateUser() {
    this.submitted = true;
    this.errorMessages = [];

    if (this.form.valid) {
      this.adminService.addEditUser(this.form.value as UserAddEdit).subscribe({
        next: (response) => {
          this.sharedService.showNotification(response);
          this.router.navigateByUrl('/admin');
        },
        error: error => {
          if (error.errors) {
            this.errorMessages = error.errors;
          }
        }
      })
    }
  }

  private checkNameNotTaken(): AsyncValidatorFn {
    return control => {
      return timer(500).pipe(
        switchMap(_ => {
          if (!control.value || (this.user && control.value && control.value.toLowerCase() === this.user.name.toLowerCase())) {
            return of(null);
          }

          return this.adminService.checkNameTaken(control.value).pipe(
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
          if (!control.value || (this.user && control.value && control.value.toLowerCase() === this.user.email.toLowerCase())) {
            return of(null);
          }

          return this.adminService.checkEmailTaken(control.value).pipe(
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

  private atLeastOneRequired(): ValidatorFn {
    return (control: AbstractControl) => {
      return Array.isArray(control.value) && control.value.length > 0
        ? null
        : { atLeastOneRequired: true };
    }
  }
}
