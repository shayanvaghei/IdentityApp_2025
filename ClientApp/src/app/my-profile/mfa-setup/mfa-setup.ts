import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormInput } from '../../shared/components/form-input/form-input';
import { ValidationMessage } from '../../shared/components/errors/validation-message/validation-message';
import { QRCodeComponent } from 'angularx-qrcode';
import { CodeInput } from '../../shared/components/code-input/code-input';
import { MfaEnableModel, QrCodeModel } from '../../shared/models/myProfile/mfa_m';
import { SharedService } from '../../shared/shared.service';
import { MyProfileService } from '../my-profile.service';

@Component({
  selector: 'app-mfa-setup',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormInput,
    ValidationMessage,
    QRCodeComponent,
    CodeInput
  ],
  templateUrl: './mfa-setup.html',
  styleUrl: './mfa-setup.scss'
})
export class MfaSetup implements OnInit {
  mfaEnabled: boolean | undefined;
  qrCode: QrCodeModel | undefined;
  form: FormGroup = new FormGroup({});
  submitted = false;
  errorMessages: string[] = [];
  completed = false;

  constructor(private formBuilder: FormBuilder,
    private sharedService: SharedService,
    private myProfileService: MyProfileService
  ) { }

  ngOnInit(): void {
    this.getStatus();
  }

  initializeForm() {
    this.submitted = false;
    this.errorMessages = [];

    if (this.mfaEnabled == false) {
      this.form = this.formBuilder.group({
        code: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]],
        currentPassword: ['', Validators.required]
      });
    } else {
      this.form = this.formBuilder.group({
        currentPassword: ['', [Validators.required]]
      });
    }

    this.form.markAsPristine();
  }

  fullCodeReceive(code: string) {
    this.form.get('code')?.setValue(code.replace(/ /g, ''));
  }

  save() {
    this.submitted = true;
    this.errorMessages = [];

    if (this.form.valid) {
      if (!this.mfaEnabled && this.qrCode && this.form) {
        const model: MfaEnableModel = {
          currentPassword: this.form.controls['currentPassword'].value,
          secret: this.qrCode.secret,
          code: this.form.controls['code'].value
        };

        this.myProfileService.mfaEnable(model).subscribe({
          next: response => {
            this.sharedService.showNotification(response);
            this.getStatus();
          }, error: error => {
            if (error && error.errors) {
              this.errorMessages = error.errors;
            }
          }
        });
      } else {
        this.myProfileService.mfaDisable(this.form.value).subscribe({
          next: response => {
            this.sharedService.showNotification(response);
            this.getStatus();
          }, error: error => {
            if (error && error.errors) {
              this.errorMessages = error.errors;
            }
          }
        });
      }
    }
  }

  // #region Private Methods
  private getStatus() {
    this.myProfileService.mfaStatus().subscribe({
      next: (response: any) => {
        this.mfaEnabled = response.isEnabled;
        if (this.mfaEnabled == false) {
          this.myProfileService.getQrCode().subscribe({
            next: qrCode => {
              this.qrCode = qrCode;
              this.initializeForm();
              this.completed = true;
            }
          })
        } else {
          this.initializeForm();
          this.completed = true;
        }
      }
    })
  }
  // #endregion

}
