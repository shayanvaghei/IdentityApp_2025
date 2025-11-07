import { Component, OnInit } from '@angular/core';
import { UserView } from '../shared/models/admin/userView_m';
import { ApiResponse } from '../shared/models/apiResponse_m';
import { AllUsersParams } from '../shared/models/pagination/allUsersParams';
import { PaginatedResult } from '../shared/models/pagination/paginatedResult';
import { SharedService } from '../shared/shared.service';
import { AdminService } from './admin.service';
import { CommonModule } from '@angular/common';
import { IaPaginationDropdownRadioFilter } from '../shared/components/pagination/ia-pagination-dropdown-radio-filter/ia-pagination-dropdown-radio-filter';
import { IaPaginationSearchInput } from '../shared/components/pagination/ia-pagination-search-input/ia-pagination-search-input';
import { IaPaginationTh } from '../shared/components/pagination/ia-pagination-th/ia-pagination-th';

import { RouterLink } from '@angular/router';
import { IaPagination } from '../shared/components/pagination/ia-pagination/ia-pagination';
import { TimeagoModule } from 'ngx-timeago';
import { NgSelectModule } from '@ng-select/ng-select';
import { FormsModule } from '@angular/forms';
import { UserAction } from '../shared/models/admin/userAction';
import { NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-admin',
  imports: [
    CommonModule,
    IaPaginationDropdownRadioFilter,
    IaPaginationSearchInput,
    IaPaginationTh,
    TimeagoModule,
    NgSelectModule,
    NgbTooltipModule,
    RouterLink,
    IaPagination,
    FormsModule
  ],
  templateUrl: './admin.html',
  styleUrl: './admin.scss'
})
export class Admin implements OnInit {
  allUsersParams = new AllUsersParams(10);
  paginatedResult: PaginatedResult<UserView> | undefined;
  apiResponse: ApiResponse<any> | undefined;
  appRoles: string[] | undefined;
  roles: string[] | undefined;

  constructor(public adminService: AdminService,
    public sharedService: SharedService
  ) { }

  ngOnInit(): void {
    this.adminService.getApplicationRoles$().subscribe({
      next: appRoles => {
        this.appRoles = appRoles;
      }
    });

    this.getAllUsers();
  }

  // #region Filters
  onClearFilters() {
    this.roles = [];
    this.adminService.resetFilter();
    this.allUsersParams = new AllUsersParams(10);
    this.getAllUsers();
  }

  onDropdownRadioFilterEntries(model: any) {
    switch (model.action) {
      case 'lockUnlock':
        this.allUsersParams.lockUnlock = model.selectedValue;
        break;
      case 'activation':
        this.allUsersParams.activation = model.selectedValue;
        break;
    }

    this.getAllUsers();
  }

  onRoleSelection() {
    if (this.roles) {
      this.allUsersParams.csvRoles = this.roles.join(",");
    }

    this.getAllUsers();
  }

  onReceivedSearchInput(value: any) {
    this.allUsersParams.searchBy = value;
    this.allUsersParams.pageNumber = 1;
    this.getAllUsers();
  }

  onSortBy(sortBy: any) {
    this.allUsersParams.sortBy = sortBy;
    this.getAllUsers();
  }
  // #endregion


  // #region Actions
  onDeleteUser(user: UserView) {
    this.sharedService.confirmBox(`Are you sure you want to delete user ${user.name}?`)
      .subscribe((result: boolean) => {
        if (result) {
          this.adminService.deleteUser(user.userId).subscribe({
            next: (response: ApiResponse<any>) => {
              this.sharedService.showNotification(response);
              this.getAllUsers();
            }
          })
        }
      });
  }

  onLockUnlockChange(user: UserView, days: number = 5) {
    let action = '';
    let message = '';

    if (user.isLocked) {
      action = 'unlock';
      days = 0;
      message = `Are you sure you want to unlock user ${user.name}?`;
    } else {
      action = 'lock';
      message = `Are you sure you want to lock user ${user.name} for the next ${days} days?`;
    }

    this.sharedService.confirmBox(message)
      .subscribe((result: boolean) => {
        if (result) {
          this.adminService.lockUnlockUser(new UserAction(user.userId, action, days)).subscribe({
            next: (response: ApiResponse<any>) => {
              this.sharedService.showNotification(response);
              this.getAllUsers();
            }
          })
        }
      });
  }

  onActivateDeactivateChange(user: UserView) {
    let action = '';
    let actionMessage = '';

    if (user.isActive) {
      action = 'inactive';
      actionMessage = 'deactivate';
    } else {
      action = 'active';
      actionMessage = 'activate';
    }

    this.sharedService.confirmBox(`Are you sure you want to ${actionMessage} user ${user.name}?`)
      .subscribe((result: boolean) => {
        if (result) {
          this.adminService.activeInactiveUser(new UserAction(user.userId, action)).subscribe({
            next: (response: ApiResponse<any>) => {
              this.sharedService.showNotification(response);
              this.getAllUsers();
            }
          })
        }
      });
  }

  onSendEmailConfirmationLink(user: UserView) {
    const message = `Are you sure you want to send a confirmation email to user ${user.name}? This will make the user inactive until they confirm their email address.`;
    this.sharedService.confirmBox(message)
      .subscribe((result: boolean) => {
        if (result) {
          this.adminService.emailConfirmationLink(user.userId).subscribe({
            next: (response: ApiResponse<any>) => {
              this.sharedService.showNotification(response);
              this.getAllUsers();
            }
          })
        }
      });
  }

  onSendForgotUsernameOrPasswordEmailLink(user: UserView) {
    const message = `Are you sure you want to send a forgot username or password link to user ${user.name}?`;
    this.sharedService.confirmBox(message)
      .subscribe((result: boolean) => {
        if (result) {
          this.adminService.forgotUsernameOrPassword(user.userId).subscribe({
            next: (response: ApiResponse<any>) => {
              this.sharedService.showNotification(response);
              this.getAllUsers();
            }
          })
        }
      });
  }
  // #endregion

  // #region Pagination
  onPageNumberChanged(pageNumber: number) {
    this.allUsersParams.pageNumber = pageNumber;
    this.getAllUsers();
  }

  onPageSizeChanged(pageSize: number) {
    this.allUsersParams.pageSize = pageSize;
    this.getAllUsers();
  }
  // #endregion

  // #region Private Methods
  private getAllUsers() {
    this.adminService.getAllUsers(this.allUsersParams).subscribe({
      next: paginatedUsers => {
        this.paginatedResult = paginatedUsers;
        console.log(this.paginatedResult);
      }
    });
  }
  // #endregion
}
