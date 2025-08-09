import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CodeInput } from '../../shared/components/code-input/code-input';
import { AccountService } from '../account.service';
import { SharedService } from '../../shared/shared.service';
import { ConfirmEmailModel } from '../../shared/models/account/confirmEmail_m';

@Component({
  selector: 'app-confirm-email',
  imports: [
    CommonModule,
    RouterLink,
    CodeInput
  ],
  templateUrl: './confirm-email.html',
  styleUrl: './confirm-email.scss'
})
export class ConfirmEmail {
  email: string | undefined;
  fullCode: string | undefined;

  constructor(private accountService: AccountService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private sharedService: SharedService
  ) {
    if (this.accountService.$user()) {
      this.router.navigateByUrl('/');
    } else {
      this.activatedRoute.queryParamMap.subscribe({
        next: (params: any) => {
          const email = params.get('email');
          if (email) {
            this.email = email;
          } else {
            this.router.navigateByUrl('/account/login');
          }
        }
      })
    }
  }

  fullCodeReceive(code: string) {
    this.fullCode = code.replace(/ /g, '');
  }

  submit() {
    if (this.email && this.fullCode && this.fullCode.length == 6) {
      this.accountService.confirmEmail(new ConfirmEmailModel(this.fullCode, this.email)).subscribe({
        next: response => {
          this.sharedService.showNotification(response);
          this.router.navigateByUrl('/account/login');
        }, error: error => {
          if (error && error.title.includes('Email was confirmed')) {
            this.router.navigateByUrl('/account/login');
          }
        }
      });
    }
  }
}
