import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { ApiResponse } from '../shared/models/apiResponse_m';
import { EditMyProfileModel } from '../shared/models/myProfile/editMyProfile_m';
import { ChangePasswordModel } from '../shared/models/myProfile/changePassword_m';
import { DeleteAccountModel } from '../shared/models/myProfile/deleteAccount_m';
import { MyProfileModel } from '../shared/models/myProfile/myProfile_m';

@Injectable({
  providedIn: 'root'
})
export class MyProfileService {
  apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getMyProfile() {
    return this.http.get<MyProfileModel>(this.apiUrl + 'myProfile');
  }

  editMyProfile(model: EditMyProfileModel) {
    return this.http.put<ApiResponse<any>>(this.apiUrl + 'myProfile', model);
  }

  changePassword(model: ChangePasswordModel) {
    return this.http.put<ApiResponse<any>>(this.apiUrl + 'myProfile/change-password', model);
  }

  deleteAccount(model: DeleteAccountModel) {
    return this.http.delete<ApiResponse<any>>(this.apiUrl + 'myProfile/delete-account', {body: model});
  }
}
