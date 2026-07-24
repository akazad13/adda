import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class NotificationService {
  constructor(private readonly toastr: ToastrService) {}

  confirm(message: string, okCallback: () => any) {
    if (window.confirm(message)) {
      okCallback();
    }
  }

  success(message: string) {
    this.toastr.success(message);
  }

  error(message: string) {
    this.toastr.error(message);
  }

  warning(message: string) {
    this.toastr.warning(message);
  }

  message(message: string) {
    this.toastr.info(message);
  }
}
