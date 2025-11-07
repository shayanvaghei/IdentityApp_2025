import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { BehaviorSubject, map, Observable, of } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { PaginatedResult } from '../shared/models/pagination/paginatedResult';
import { UserView } from '../shared/models/admin/userView_m';
import { AllUsersParams } from '../shared/models/pagination/allUsersParams';
import { ApiResponse } from '../shared/models/apiResponse_m';
import { UserAction } from '../shared/models/admin/userAction';
import { UserAddEdit } from '../shared/models/admin/userAddEdit';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  apiUrl = environment.apiUrl;
  resetFilterValue = false;
  resetFilterSource = new BehaviorSubject<boolean>(this.resetFilterValue);
  resetFilter$ = this.resetFilterSource.asObservable();

  resetSortByValue = '';
  resetSortBySource = new BehaviorSubject<string>(this.resetSortByValue);
  resetSortBy$ = this.resetSortBySource.asObservable();

  appRoles: string[] = [];

  constructor(private http: HttpClient) { }

  getAllUsers(allUsersParams: AllUsersParams) {
    let params = new HttpParams();
    params = params.append('pageNumber', allUsersParams.pageNumber);
    params = params.append('pageSize', allUsersParams.pageSize);

    if (allUsersParams.sortBy) {
      params = params.append('sortBy', allUsersParams.sortBy);
    }

    if (allUsersParams.searchBy) {
      params = params.append('searchBy', allUsersParams.searchBy);
    }

    if (allUsersParams.lockUnlock) {
      params = params.append('lockUnlock', allUsersParams.lockUnlock);
    }

    if (allUsersParams.activation) {
      params = params.append('activation', allUsersParams.activation);
    }

    if (allUsersParams.csvRoles) {
      params = params.append('csvRoles', allUsersParams.csvRoles);
    }

    return this.http.get<PaginatedResult<UserView>>(this.apiUrl + 'admin/get-users', { params });
  }

  lockUnlockUser(model: UserAction) {
    return this.http.put<ApiResponse<any>>(this.apiUrl + 'admin/lock-unlock-user', model);
  }

  activeInactiveUser(model: UserAction) {
    return this.http.put<ApiResponse<any>>(this.apiUrl + 'admin/active-inactive-user', model);
  }

  deleteUser(userId: number) {
    return this.http.delete<ApiResponse<any>>(this.apiUrl + 'admin/delete-user/' + userId);
  }

  emailConfirmationLink(userId: number) {
    return this.http.put<ApiResponse<any>>(this.apiUrl + 'admin/email-confirmation-link/' + userId, {});
  }

  forgotUsernameOrPassword(userId: number) {
    return this.http.put<ApiResponse<any>>(this.apiUrl + 'admin/forgot-username-or-password/' + userId, {});
  }

  getUser(userId: number) {
    if (userId !== 0) {
      return this.http.get<UserAddEdit>(this.apiUrl + 'admin/get-user/' + userId);
    } else {
      return of(undefined);
    }
  }

  addEditUser(model: UserAddEdit) {
    return this.http.post<ApiResponse<any>>(this.apiUrl + 'admin/add-edit-user', model);
  }

  checkNameTaken(name: string) {
    return this.http.get(environment.apiUrl + 'admin/name-taken?name=' + name);
  }

  checkEmailTaken(email: string) {
    return this.http.get(environment.apiUrl + 'admin/email-taken?email=' + email);
  }

  getApplicationRoles$(): Observable<string[] | undefined> {
    if (this.appRoles.length > 0) return of(this.appRoles);

    return this.http.get<string[]>(this.apiUrl + 'admin/app-roles').pipe(
      map(appRoles => {
        this.appRoles = appRoles;
        return this.appRoles;
      }));
  }

  // #region Helper Methods
  resetSortBy(label: string) {
    this.resetSortByValue = label;
    this.resetSortBySource.next(this.resetSortByValue);
  }

  resetFilter() {
    this.resetFilterValue = !this.resetFilterValue;
    this.resetFilterSource.next(this.resetFilterValue);
  }
  // #endregion
}
