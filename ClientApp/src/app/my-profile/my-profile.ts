import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { EditProfile } from './edit-profile/edit-profile';
import { ChangePassword } from './change-password/change-password';
import { DeleteAccount } from './delete-account/delete-account';
import { MfaSetup } from './mfa-setup/mfa-setup';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AccountService } from '../account/account.service';
import { keyframes } from '@angular/animations';

@Component({
  selector: 'app-my-profile',
  imports: [
    CommonModule,
    EditProfile,
    ChangePassword,
    DeleteAccount,
    MfaSetup,
    RouterLink
  ],
  templateUrl: './my-profile.html',
  styleUrl: './my-profile.scss'
})
export class MyProfile implements OnInit {
  page: string = '';

  constructor(public accountService: AccountService,
    private activatedRoute: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.activatedRoute.paramMap.subscribe(params => {
      const page = params.get('page');
      if (page) {
        const pageKewords = ['edit-profile', 'change-password', 'mfa-setup', 'delete-account'];

        if (pageKewords.some(pk => page.includes(pk))) {
          this.page = page;
        } else {
          this.router.navigateByUrl('/my-profile/edit-profile');
        }
      } else {
        this.page = '/my-profile/edit-profile';
        this.router.navigateByUrl(this.page);
      }
    })
  }

  navigate(page: string) {
    this.page = page;
  }
}
