import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AccountService } from '../../account/account.service';
import { UserHasRoles } from '../directives/user-has-roles';

@Component({
  selector: 'app-navbar',
  imports: [
    RouterLinkActive,
    RouterLink,
    CommonModule,
    UserHasRoles
  ],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar {
  collapsed = true;

  constructor(public accountService: AccountService) {

  }

  toggleCollapsed() {
    this.collapsed = !this.collapsed;
  }

  logout() {
    this.accountService.logout().subscribe();
  }
}
