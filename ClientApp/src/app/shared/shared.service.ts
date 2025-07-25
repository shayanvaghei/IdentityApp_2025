import { Injectable } from '@angular/core';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { ApiResponse } from './models/apiResponse_m';
import { Notification } from './components/modals/notification/notification';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  constructor(private toastr: ToastrService,
    private modalService: NgbModal
  ) {}

  showNotification(apiResponse: ApiResponse<any>, backdrop: boolean = false) {
    let isSuccess = false;

    if (apiResponse.statusCode == 200 || apiResponse.statusCode == 201) {
      isSuccess = true;
    }

    if (apiResponse.showWithToastr) {
      if (isSuccess) {
        this.toastr.success(apiResponse.message, apiResponse.title);
      } else {
        this.toastr.error(apiResponse.message, apiResponse.title);
      }
    } else {
      const options: NgbModalOptions = {
        backdrop
      };

      const modalRef = this.modalService.open(Notification, options);
      modalRef.componentInstance.isSuccess = isSuccess;
      modalRef.componentInstance.title = apiResponse.title;
      modalRef.componentInstance.message = apiResponse.message;
      modalRef.componentInstance.isHtmlEnabled = apiResponse.isHtmlEnabled;
    }
  }
  
}
